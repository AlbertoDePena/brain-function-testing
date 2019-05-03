namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
module Validation =
    open System
    open System.Text.RegularExpressions
    open Models

    let validateProperty name value =
        if String.IsNullOrWhiteSpace(value) then invalidArg name (sprintf "%s is required" name)
        value.Trim()

    let validateDbOptions (opts : DatabaseOptions) = { 
            opts with 
                EndpointUrl = validateProperty "endpointUrl" opts.EndpointUrl
                AccountKey = validateProperty "accountKey" opts.AccountKey
                DatabaseId = validateProperty "databaseId" opts.DatabaseId
                CollectionId = validateProperty "collectionId" opts.CollectionId
        }

    let validateBftOptions (opts : BftOptions) = { 
            opts with 
                EndpointUrl = validateProperty "endpointUrl" opts.EndpointUrl
                Account = validateProperty "account" opts.Account
                Username = validateProperty "username" opts.Username
                Password = validateProperty "password" opts.Password
        }

    let validateEmail (Email email) =
        if Regex.IsMatch(email, @".+@.+") |> not then invalidOp "Email is not valid"       

        validateProperty "email" email |> Email

    let validateSubjectId (SubjectId subjectId) =
        validateProperty "subjectId" subjectId |> SubjectId      

    let validateTester (tester : Tester) =
        if Regex.IsMatch(tester.dob, @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\/\d{2}\/\d{4}") |> not then 
            invalidOp "Date of birth is not valid. Expected format: MMM/DD/YYYY"      

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
    open Models
    open BFT.Extensions
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client

    let getClient : GetClient =
        fun dbOptions ->
            async {    
                let dbOptions = Validation.validateDbOptions dbOptions

                let client = new DocumentClient(Uri(dbOptions.EndpointUrl), dbOptions.AccountKey)

                let database = Database(Id = dbOptions.DatabaseId)
                let databaseUri = UriFactory.CreateDatabaseUri(dbOptions.DatabaseId)

                let collection = DocumentCollection(Id = dbOptions.CollectionId)
                let options = RequestOptions(OfferThroughput = Nullable(1000))

                collection.PartitionKey.Paths.Add("/email")

                let! _ = client.CreateDatabaseIfNotExistsAsync(database) |> Async.AwaitTask
                let! _ = client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, options) |> Async.AwaitTask

                return client
            }

    let createDocument : CreateDocument =
        fun client dbOptions document ->
            async {
                let! result =
                    client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(dbOptions.DatabaseId, dbOptions.CollectionId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let replaceDocument : ReplaceDocument =
        fun client dbOptions (DocumentId documentId) document ->
            async {
                let! result =
                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(dbOptions.DatabaseId, dbOptions.CollectionId, documentId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let getTester : GetTester =
        fun client dbOptions filter ->
            async {
                let query =
                    match filter with
                    | EmailFilter email ->
                        let (Email email) = Validation.validateEmail email

                        sprintf "SELECT * FROM Tester WHERE Tester.email = '%s'" email
                    | SubjectIdFilter subjectId ->
                        let (SubjectId subjectId) = Validation.validateSubjectId subjectId

                        sprintf "SELECT * FROM Tester WHERE Tester.subjectId = '%s'" subjectId

                let! testers = 
                    client.CreateDocumentQuery<Tester>(
                        UriFactory.CreateDocumentCollectionUri(dbOptions.DatabaseId, dbOptions.CollectionId), query).ToListAsync() 
                    |> Async.AwaitTask

                return Seq.tryHead testers
            }

    let saveTestResults : SaveTestResults =
        fun createDocument replaceDocument getTester client dbOptions testResults ->
            async {  
                let! testerOption = 
                    SubjectIdFilter (SubjectId testResults.subjectId) 
                    |> getTester client dbOptions

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            testResults = existingTester.testResults @ [testResults] 
                    }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client dbOptions documentId updatedTester

                let! documentId =
                    match testerOption with
                    | None -> invalidOp (sprintf "Tester with subject ID %s not found" testResults.subjectId)
                    | Some existingTester -> replace existingTester

                return documentId
            }

    let saveTester : SaveTester =
        fun createDocument replaceDocument getTester client dbOptions tester ->
            async {
                let tester = Validation.validateTester tester

                let! testerOption = 
                    EmailFilter (Email tester.email)
                    |> getTester client dbOptions

                let create () = // TODO - generate Subject ID
                    let newTester = { tester with testResults = [] }

                    createDocument client dbOptions newTester

                let replace existingTester =
                    let updatedTester = { existingTester with firstName = tester.firstName; lastName = tester.lastName; dob = tester.dob }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client dbOptions documentId updatedTester

                let! documentId =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return documentId
            } 

    let getTestLink : GetTestLink =
        fun getTester client settings (Email email) ->         
            async {
                let bft = Validation.validateBftOptions settings.BFT

                let! testerOption = 
                    EmailFilter (Email email)
                    |> getTester client settings.DB

                let testLink =
                    match testerOption with
                    | None -> invalidOp (sprintf "Tester with email of %s not found" email)
                    | Some tester ->

                        let (month, day, year) =
                            match tester.dob.Split("/") with
                            | [|month; day; year|] -> (month, day, year)
                            | _ -> invalidOp (sprintf "Tester with email of %s does not have a valid DOB" email)

                        let testLinkRequest = {
                            request = "rtl"
                            account = bft.Account
                            username = bft.Username
                            password = bft.Password
                            subjectId = tester.subjectId
                            dobYear = year
                            dobMonth = month
                            dobDay = day
                            testConfig = "9"
                            testLang = "english_us"
                        }

                        TestLink bft.EndpointUrl

                return testLink
            }

[<RequireQualifiedAccess>]
module TesterAPI =
    open Models 

    let getTester =
        fun settings filter -> 
            async {
                use! client = Testers.getClient settings.DB 

                return! 
                    Testers.getTester 
                    client settings.DB filter   
            }  

    let getTestLink =
        fun settings email ->
            async {
                use! client = Testers.getClient settings.DB 

                return!
                    Testers.getTestLink
                    Testers.getTester
                    client settings email
            }                      

    let saveTestResults =
        fun settings testResults -> 
            async {
                use! client = Testers.getClient settings.DB 

                return! 
                    Testers.saveTestResults 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester 
                    client settings.DB testResults  
            }  

    let saveTester =
        fun settings tester -> 
            async {
                use! client = Testers.getClient settings.DB 

                return! 
                    Testers.saveTester 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester 
                    client settings.DB tester  
            }   

                       

    
