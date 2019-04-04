namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
module Async = 

  let singleton value = value |> async.Return

  let bind f x = async.Bind(x, f)

  let map f x = x |> bind (f >> singleton)

module Models =
    open Microsoft.Azure.Documents.Client

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

    type GetClient = DatabaseOptions -> Async<DocumentClient>

    type GetTester =  DocumentClient -> DatabaseOptions -> SubjectId -> Async<Tester option>

    type UpsertTestResults = GetTester -> GetClient -> DatabaseOptions -> TestResults -> Async<unit>

    type UpsertTester = GetTester -> GetClient -> DatabaseOptions -> Tester -> Async<unit>
