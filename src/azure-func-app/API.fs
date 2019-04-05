namespace BFT.AzureFuncApp

module private Testers =
    open System
    open Models
    open BFT.Extensions
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client

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

                collection.PartitionKey.Paths.Add("/subject_id")

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

                return result
            }

    let replaceDocument : ReplaceDocument =
        fun client (DatabaseId databaseId) (CollectionId collectionId) (DocumentId documentId) document ->
            async {
                let! result =
                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(databaseId, collectionId, documentId), document) 
                    |> Async.AwaitTask

                return result
            }

    let getTester : GetTester =
        fun client (DatabaseId databaseId) (CollectionId collectionId) (SubjectId subjectId) ->
            async {
                if String.IsNullOrWhiteSpace(subjectId) then raise (ArgumentNullException("subjectId"))

                let query = sprintf "SELECT * FROM Tester WHERE Tester.subject_id = '%s'" subjectId

                let! testers = 
                    client.CreateDocumentQuery<Tester>(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), query).ToListAsync() 
                    |> Async.AwaitTask

                return Seq.tryHead testers
            }

    let saveTestResults : SaveTestResults =
        fun createDocument replaceDocument getTester getClient options testResults ->
            async {     
                use! client = getClient options

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)
                let subjectId = (SubjectId testResults.subject_id)

                let! testerOption = getTester client databaseId collectionId subjectId

                let create () =
                    let newTester = {
                        id = ""; first_name = ""; last_name = ""; email = ""; 
                        subject_id = testResults.subject_id; test_results = [testResults]
                    }

                    createDocument client databaseId collectionId newTester

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            test_results = existingTester.test_results @ [testResults] 
                    }

                    let documentId = (DocumentId existingTester.id)

                    replaceDocument client databaseId collectionId documentId updatedTester

                let! _ =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return ()
            }

    let saveTester : SaveTester =
        fun createDocument replaceDocument getTester getClient options tester ->
            async {
                use! client = getClient options

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)
                let subjectId = (SubjectId tester.subject_id)

                let! testerOption = getTester client databaseId collectionId subjectId

                let create () =
                    let newTester = { 
                        tester with 
                            test_results = [] 
                    }

                    createDocument client databaseId collectionId newTester

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            first_name = tester.first_name; 
                            last_name = tester.last_name; 
                            email = tester.email 
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
    open Testers
   
    let getTester =
        fun options subjectId -> 
            async {
                use! client = getClient options 

                let databaseId = (DatabaseId options.DatabaseId)
                let collectionId = (CollectionId options.CollectionId)

                return! getTester client databaseId collectionId subjectId   
            }        

    let saveTestResults =
        fun options testResults -> 
            saveTestResults createDocument replaceDocument Testers.getTester getClient options testResults    

    let saveTester =
        fun options tester -> 
            saveTester createDocument replaceDocument Testers.getTester getClient options tester                

    
