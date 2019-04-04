namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
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

    let getTester : GetTester =
        fun client options (SubjectId subjectId) ->
            async {
                if String.IsNullOrWhiteSpace(subjectId) then raise (ArgumentNullException("subjectId"))

                let query = sprintf "SELECT * FROM Tester WHERE Tester.subject_id = '%s'" subjectId

                let! testers = 
                    client.CreateDocumentQuery<Tester>(
                        UriFactory.CreateDocumentCollectionUri(options.DatabaseId, options.CollectionId), query).ToListAsync() 
                    |> Async.AwaitTask

                return Seq.tryHead testers
            }

    let upsertTestResults : UpsertTestResults =
        fun getTester getClient options testResults ->
            async {     
                use! client = getClient options

                let! testerOption = getTester client options (SubjectId testResults.subject_id)

                let create () =
                    let newTester = {
                        id = ""; first_name = ""; last_name = ""; email = ""; 
                        subject_id = testResults.subject_id; test_results = [testResults]
                    }

                    client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(options.DatabaseId, options.CollectionId), newTester) 
                    |> Async.AwaitTask

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            test_results = existingTester.test_results @ [testResults] 
                    }

                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(options.DatabaseId, options.CollectionId, existingTester.id), updatedTester) 
                    |> Async.AwaitTask

                let! _ =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return ()
            }

    let upsertTester : UpsertTester =
        fun getTester getClient options tester ->
            async {
                use! client = getClient options

                let! testerOption = getTester client options (SubjectId tester.subject_id)

                let create () =
                    let newTester = { 
                        tester with 
                            test_results = [] 
                    }

                    client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(
                            options.DatabaseId, options.CollectionId), newTester) 
                    |> Async.AwaitTask

                let replace existingTester =
                    let updatedTester = { 
                        existingTester with 
                            first_name = tester.first_name; 
                            last_name = tester.last_name; 
                            email = tester.email 
                    }

                    client.ReplaceDocumentAsync(
                        UriFactory.CreateDocumentUri(
                            options.DatabaseId, options.CollectionId, existingTester.id), updatedTester) 
                    |> Async.AwaitTask

                let! _ =
                    match testerOption with
                    | None -> create ()
                    | Some existingTester -> replace existingTester

                return ()
            }          

[<RequireQualifiedAccess>]
module TesterAPI =
   
    let getTester =
        fun options subjectId -> 
            async {
                use! client = Testers.getClient options 

                return! Testers.getTester client options subjectId   
            }        

    let upsertTestResults =
        fun options testResults -> 
            Testers.upsertTestResults Testers.getTester Testers.getClient options testResults    

    let upsertTester =
        fun options tester -> 
            Testers.upsertTester Testers.getTester Testers.getClient options tester                

    
