namespace BFT.AzureFuncApp

open System
open System.Linq
open System.Threading.Tasks
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open FSharp.Control.Tasks.ContextInsensitive

module Models =

    [<CLIMutable>]
    type TestResults = {
        cnsvs_id : string
        subject_id : string
        account_id : string
        birth_date : string
        gender : string
        test_date : string
        test_time : string
        timezone : string
        gmt_test_date : string
        gmt_test_time : string       
        duration : string
        language : string
        report_data : string
    }

    [<CLIMutable>]
    type Tester = {
        id : string
        first_name : string
        last_name : string
        email : string
        subject_id : string
        test_results : TestResults list
    }

    [<CLIMutable>]
    type DatabaseOptions = {
        EndpointUrl : string
        AccountKey : string
        DatabaseId : string
        CollectionId : string
    }

    type EndpointUrl = EndpointUrl of string

    type AccountKey = AccountKey of string

    type DatabaseId = DatabaseId of string

    type CollectionId = CollectionId of string

    type SubjectId = SubjectId of string

    type GetClient = EndpointUrl -> AccountKey -> DocumentClient

    type EnsureCreated = GetClient -> DatabaseOptions -> Task<DocumentClient>

    type GetTester = EnsureCreated -> GetClient -> DatabaseOptions -> SubjectId -> Task<Tester option>

    type InsertTestResults = GetTester -> EnsureCreated -> GetClient -> DatabaseOptions -> TestResults -> Task<unit>

[<RequireQualifiedAccess>]
module Service =
    open Models
    open BFT.Extensions

    let getClient : GetClient =
        fun (EndpointUrl endpointUrl) (AccountKey accountKey) ->
            if String.IsNullOrWhiteSpace(endpointUrl) then raise (ArgumentNullException("endpointUrl"))
            if String.IsNullOrWhiteSpace(accountKey) then raise (ArgumentNullException("accountKey"))

            let uri = Uri(endpointUrl)
            new DocumentClient(uri, accountKey)

    let ensureCreated : EnsureCreated =
        fun getClient options ->
            task {    
                let client = getClient (EndpointUrl options.EndpointUrl) (AccountKey options.AccountKey)

                if String.IsNullOrWhiteSpace(options.DatabaseId) then raise (ArgumentNullException("databaseId"))
                if String.IsNullOrWhiteSpace(options.CollectionId) then raise (ArgumentNullException("collectionId"))

                let database = Database(Id = options.DatabaseId)
                let databaseUri = UriFactory.CreateDatabaseUri(options.DatabaseId)

                let collection = DocumentCollection(Id = options.CollectionId)
                let options = RequestOptions(OfferThroughput = Nullable(1000))

                collection.PartitionKey.Paths.Add("/subject_id")

                let! _ = client.CreateDatabaseIfNotExistsAsync(database)
                let! _ = client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, options)

                return client
            }

    let getTester : GetTester =
        fun ensureCreated getClient options (SubjectId subjectId) ->
            task {
                if String.IsNullOrWhiteSpace(subjectId) then raise (ArgumentNullException("subjectId"))

                use! client = ensureCreated getClient options

                let collectionUri = UriFactory.CreateDocumentCollectionUri(options.DatabaseId, options.CollectionId)

                let query = sprintf "SELECT * FROM Tester WHERE Tester.subject_id = '%s'" subjectId

                let! testers = client.CreateDocumentQuery<Tester>(collectionUri, query).ToListAsync()

                let result =
                    let tester = testers.FirstOrDefault()
                    if box tester |> isNull then
                        None
                    else Some tester

                return result
            }

    let insertTestResults : InsertTestResults =
        fun getTester ensureCreated getClient options testResults ->
            task {                
                let! testerOption = getTester ensureCreated getClient options (SubjectId testResults.subject_id)

                use! client = ensureCreated getClient options

                let create () =
                    let newTester = {
                        id = ""; first_name = ""; last_name = ""; email = ""; 
                        subject_id = testResults.subject_id; test_results = [testResults]
                    }

                    let collectionUri = UriFactory.CreateDocumentCollectionUri(options.DatabaseId, options.CollectionId)

                    client.CreateDocumentAsync(collectionUri, newTester)

                let update tester =
                    let updatedTester = { tester with test_results = tester.test_results @ [testResults] }

                    let documentUri = UriFactory.CreateDocumentUri(options.DatabaseId, options.CollectionId, tester.subject_id)

                    client.ReplaceDocumentAsync(documentUri, updatedTester)

                let! _ =
                    match testerOption with
                    | None -> create ()
                    | Some tester -> update tester

                return ()
            }

    