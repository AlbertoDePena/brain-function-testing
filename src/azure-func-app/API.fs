namespace BFT.AzureFuncApp
open System.Collections.Generic

[<RequireQualifiedAccess>]
module Validation =
    open System
    open System.Text.RegularExpressions
    open Models

    let validateProperty name value =
        if String.IsNullOrWhiteSpace(value) then invalidArg name (sprintf "%s is required" name)
        value.Trim()

    let validateAppSettings (settings : AppSettings) = { 
            settings with 
                DbEndpointUrl = validateProperty "DB Endpoint URL" settings.DbEndpointUrl
                DbAccountKey = validateProperty "DB Account Key" settings.DbAccountKey
                BftEndpointUrl = validateProperty "BFT Endpoint URL" settings.BftEndpointUrl
                BftAccount = validateProperty "BFT Account" settings.BftAccount
                BftUsername = validateProperty "BFT Username" settings.BftUsername
                BftPassword = validateProperty "BFT Password" settings.BftPassword
        }

    let validateEmail (Email email) =
        if Regex.IsMatch(email, @".+@.+") |> not then invalidArg "email" "Email is not valid"       

        validateProperty "email" email |> Email

    let validateTester (tester : Tester) =
        if Regex.IsMatch(tester.dob, @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\/\d{2}\/\d{4}") |> not then 
            invalidArg "dob" "Date of birth is not valid. Expected format: MMM/DD/YYYY"      

        {
            tester with
                firstName = validateProperty "firstName" tester.firstName
                lastName = validateProperty "lastName" tester.lastName
                email = Email tester.email |> validateEmail |> (fun (Email email) -> email)
                dob = validateProperty "dob" tester.dob
        }        

[<RequireQualifiedAccess>]
module Testers =
    open System
    open System.Net.Http
    open Models
    open BFT.Extensions
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client

    let DatabaseId = "testers"

    let CollectionId = "testers"

    let getClient : GetClient =
        fun settings ->
            async {    
                let settings = Validation.validateAppSettings settings
                let client = new DocumentClient(Uri(settings.DbEndpointUrl), settings.DbAccountKey)
                let collection = DocumentCollection(Id = CollectionId)
                
                collection.PartitionKey.Paths.Add("/email")
                
                let options = RequestOptions(OfferThroughput = Nullable(1000))
                let databaseUri = UriFactory.CreateDatabaseUri(DatabaseId)

                let! _ = 
                    client.CreateDatabaseIfNotExistsAsync(Database(Id = DatabaseId)) 
                    |> Async.AwaitTask
                let! _ = 
                    client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, options) 
                    |> Async.AwaitTask

                return client
            }

    let createDocument : CreateDocument =
        fun client document ->
            async {
                let! result =
                    client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let replaceDocument : ReplaceDocument =
        fun client (DocumentId documentId) document ->
            async {
                let! result =
                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(DatabaseId, CollectionId, documentId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let getTester : GetTester =
        fun client email ->
            async {
                let query = 
                    let (Email email) = Validation.validateEmail email
                    sprintf "SELECT * FROM Tester WHERE Tester.email = '%s'" email 
                
                let! testers =
                    client.CreateDocumentQuery<Tester>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), query).ToListAsync() 
                        |> Async.AwaitTask

                return Seq.tryHead testers
            }

    let saveTestResults : SaveTestResults =
        fun createDocument replaceDocument getTester client testResults ->
            async {  
                let! testerOption = 
                    (Email testResults.subjectId) 
                    |> getTester client

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            testResults = existingTester.testResults @ [testResults] 
                    }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client documentId updatedTester

                let! documentId =
                    match testerOption with
                    | None -> invalidOp (sprintf "Tester with subject ID %s not found" testResults.subjectId)
                    | Some existingTester -> replace existingTester

                return documentId
            }

    let saveTester : SaveTester =
        fun createDocument replaceDocument getTester client tester ->
            async {
                let tester = Validation.validateTester tester

                let! testerOption = 
                    (Email tester.email)
                    |> getTester client

                let create () =
                    // TODO: generate subject ID
                    // 00004BFTTR042582
                    // Where 00004 is a sequential counter
                    // BFT is a the character code letting us know which practice generated the patient
                    // TR is first and last initial
                    // 042582 is the DOB

                    let newTester = { tester with testResults = [] }

                    createDocument client newTester

                let replace existingTester =
                    let updatedTester = { existingTester with firstName = tester.firstName; lastName = tester.lastName; dob = tester.dob }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client documentId updatedTester

                let! documentId =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return documentId
            } 

    let getTestLink : GetTestLink =
        fun getTester client settings (Email email) ->         
            async {
                
                let generateLink tester =
                    async {
                        let settings = Validation.validateAppSettings settings

                        let (month, day, year) =
                            match tester.dob.Split("/") with
                            | [|month; day; year|] -> (month, day, year)
                            | _ -> invalidOp (sprintf "Tester with email of %s does not have a valid DOB" email)

                        let payload = 
                            [| 
                                ("request", "rtl")
                                ("account", settings.BftAccount)
                                ("username", settings.BftUsername)
                                ("password", settings.BftPassword)
                                ("subject_id", tester.email)
                                ("dob_year", year)
                                ("dob_month", month)
                                ("dob_day", day)
                                ("test_config", "9")
                                ("test_lang", "english_us")
                            |] |> Map.ofArray

                        use httpClient = new HttpClient()

                        let! response = 
                            httpClient.PostAsync(settings.BftEndpointUrl, new FormUrlEncodedContent(payload)) 
                            |> Async.AwaitTask

                        if not response.IsSuccessStatusCode then invalidOp response.ReasonPhrase

                        let! data = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                        return TestLink data
                    }

                let! testerOption = 
                    (Email email)
                    |> getTester client

                let! testLink =
                    match testerOption with
                    | None -> invalidOp (sprintf "Tester with email of %s not found" email)
                    | Some tester -> generateLink tester
                     
                return testLink
            }

[<RequireQualifiedAccess>]
module TesterAPI =
    open Models 

    let getTester =
        fun settings filter -> 
            async {
                use! client = Testers.getClient settings 

                return! Testers.getTester client filter   
            }  

    let getTestLink =
        fun settings email ->
            async {
                use! client = Testers.getClient settings 

                return!
                    Testers.getTestLink
                    Testers.getTester
                    client settings email
            }                      

    let saveTestResults =
        fun settings testResults -> 
            async {
                use! client = Testers.getClient settings 

                return! 
                    Testers.saveTestResults 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester 
                    client testResults  
            }  

    let saveTester =
        fun settings tester -> 
            async {
                use! client = Testers.getClient settings 

                return! 
                    Testers.saveTester 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester 
                    client tester  
            }   

                       

    
