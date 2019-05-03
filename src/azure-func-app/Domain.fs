namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
module Async = 

  let singleton value = value |> async.Return

  let bind f x = async.Bind(x, f)

  let map f x = x |> bind (f >> singleton)

module Models =
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client
    open Newtonsoft.Json

    [<CLIMutable>]
    type TestLinkRequest = {
        request: string
        account: string
        username: string
        password: string
        [<JsonProperty("subject_id")>] subjectId: string
        [<JsonProperty("dob_year")>] dobYear: string
        [<JsonProperty("dob_month")>] dobMonth: string
        [<JsonProperty("dob_day")>] dobDay: string
        [<JsonProperty("test_config")>] testConfig: string
        [<JsonProperty("test_lang")>] testLang: string
    }

    [<CLIMutable>]
    type TestResults = {
        cnsvsId : string
        subjectId : string
        accountId : string
        birthDate : string
        gender : string
        testDate : string
        testTime : string
        timezone : string
        gmtTestDate : string
        gmtTestTime : string       
        duration : string
        language : string
        reportData : string
    }

    [<CLIMutable>]
    type Tester = {
        id : string
        firstName : string
        lastName : string
        email : string
        subjectId : string
        dob: string
        testStatus : string
        scheduleDate: string
        testResults : TestResults list
    }

    [<CLIMutable>]
    type DatabaseOptions = {
        EndpointUrl : string
        AccountKey : string
        DatabaseId : string
        CollectionId : string
    }

    [<CLIMutable>]
    type BftOptions = {
        Account : string
        Username : string
        Password : string
        EndpointUrl : string
    }

    [<CLIMutable>]
    type AppSettings = {
        DB : DatabaseOptions
        BFT : BftOptions
    }

    type TestLink = TestLink of string

    type SubjectId = SubjectId of string

    type Email = Email of string

    type DocumentId = DocumentId of string

    type TesterFilter =
        | EmailFilter of Email
        | SubjectIdFilter of SubjectId

    type GetClient = 
        DatabaseOptions -> Async<DocumentClient>

    type GetTester = 
        IDocumentClient -> DatabaseOptions -> TesterFilter -> Async<Tester option>

    type CreateDocument = 
        IDocumentClient -> DatabaseOptions -> Tester -> Async<DocumentId>

    type ReplaceDocument = 
        IDocumentClient -> DatabaseOptions -> DocumentId -> Tester -> Async<DocumentId>

    type SaveTestResults = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> DatabaseOptions -> TestResults -> Async<DocumentId>

    type SaveTester = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> DatabaseOptions -> Tester -> Async<DocumentId>

    type GetTestLink =
        GetTester -> IDocumentClient -> AppSettings -> Email -> Async<TestLink>        
