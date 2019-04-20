namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
module Testers =
    open System
    open System.Text.RegularExpressions
    open Models
    open BFT.Extensions
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client

    let EmailRegex = @".+@.+"
    let DOBRegex = @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\/\d{2}\/\d{4}"

    let getClient : GetClient =
        fun options ->
            async {    
                if String.IsNullOrWhiteSpace(options.EndpointUrl) then raise (ArgumentNullException("endpointUrl"))
                if String.IsNullOrWhiteSpace(options.AccountKey) then raise (ArgumentNullException("accountKey"))
                if String.IsNullOrWhiteSpace(options.DatabaseId) then raise (ArgumentNullException("databaseId"))
                if String.IsNullOrWhiteSpace(options.CollectionId) then raise (ArgumentNullException("collectionId"))

                let client = new DocumentClient(Uri(options.EndpointUrl), options.AccountKey)

                let database = Database(Id = options.DatabaseId)
                let databaseUri = UriFactory.CreateDatabaseUri(options.DatabaseId)

                let collection = DocumentCollection(Id = options.CollectionId)
                let options = RequestOptions(OfferThroughput = Nullable(1000))

                collection.PartitionKey.Paths.Add("/email")

                let! _ = client.CreateDatabaseIfNotExistsAsync(database) |> Async.AwaitTask
                let! _ = client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, options) |> Async.AwaitTask

                return client
            }

    let createDocument : CreateDocument =
        fun client (DatabaseId databaseId) (CollectionId collectionId) document ->
            async {
                let! result =
                    client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let replaceDocument : ReplaceDocument =
        fun client (DatabaseId databaseId) (CollectionId collectionId) (DocumentId documentId) document ->
            async {
                let! result =
                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(databaseId, collectionId, documentId), document) 
                    |> Async.AwaitTask

                return (DocumentId result.Resource.Id)
            }

    let getTester : GetTester =
        fun client (DatabaseId databaseId) (CollectionId collectionId) filter ->
            async {
                let query =
                    match filter with
                    | EmailFilter (Email email) ->
                        if String.IsNullOrWhiteSpace(email) then raise (ArgumentNullException("email"))

                        sprintf "SELECT * FROM Tester WHERE Tester.email = '%s'" email
                    | SubjectIdFilter (SubjectId subjectId) ->
                        if String.IsNullOrWhiteSpace(subjectId) then raise (ArgumentNullException("subjectId"))

                        sprintf "SELECT * FROM Tester WHERE Tester.subjectId = '%s'" subjectId

                let! testers = 
                    client.CreateDocumentQuery<Tester>(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), query).ToListAsync() 
                    |> Async.AwaitTask

                return Seq.tryHead testers
            }

    let saveTestResults : SaveTestResults =
        fun createDocument replaceDocument getTester client databaseId collectionId testResults ->
            async {  
                if String.IsNullOrWhiteSpace(testResults.subject_id) then raise (ArgumentNullException("subject_id"))
   
                let subjectId = (SubjectId testResults.subject_id) |> SubjectIdFilter

                let! testerOption = getTester client databaseId collectionId subjectId

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            testResults = existingTester.testResults @ [testResults] 
                    }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client databaseId collectionId documentId updatedTester

                let! _ =
                    match testerOption with
                    | None -> invalidOp (sprintf "Tester with subject ID %s not found" testResults.subject_id)
                    | Some existingTester -> replace existingTester

                return ()
            }

    let saveTester : SaveTester =
        fun createDocument replaceDocument getTester client databaseId collectionId tester ->
            async {
                if String.IsNullOrWhiteSpace(tester.firstName) then raise (ArgumentNullException("firstName"))
                if String.IsNullOrWhiteSpace(tester.lastName) then raise (ArgumentNullException("lastName"))
                if String.IsNullOrWhiteSpace(tester.email) then raise (ArgumentNullException("email"))
                if String.IsNullOrWhiteSpace(tester.dob) then raise (ArgumentNullException("dob"))

                if Regex.IsMatch(tester.email, EmailRegex) |> not then invalidOp "Email is not valid"
                if Regex.IsMatch(tester.dob, DOBRegex) |> not then invalidOp "Date of birth is not valid. Expected format: Jan/03/1985"

                let email = (Email tester.email) |> EmailFilter

                let! testerOption = getTester client databaseId collectionId email

                let create () = // TODO - generate Subject ID
                    let newTester = { 
                        tester with 
                            testResults = [] 
                    }

                    createDocument client databaseId collectionId newTester

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            firstName = tester.firstName; 
                            lastName = tester.lastName; 
                            dob = tester.dob 
                    }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client databaseId collectionId documentId updatedTester

                let! _ =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return ()
            }          

[<RequireQualifiedAccess>]
module TesterAPI =
    open Models
   
    let getTester =
        fun options subjectId -> 
            async {
                use! client = Testers.getClient options 

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)

                return! Testers.getTester client databaseId collectionId subjectId   
            }        

    let saveTestResults =
        fun options testResults -> 
            async {
                use! client = Testers.getClient options 

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)

                return! 
                    Testers.saveTestResults 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester client databaseId collectionId testResults  
            }  

    let saveTester =
        fun options tester -> 
            async {
                use! client = Testers.getClient options 

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)

                return! 
                    Testers.saveTester 
                    Testers.createDocument 
                    Testers.replaceDocument 
                    Testers.getTester client databaseId collectionId tester  
            }              

    
