namespace BFT.AzureFuncApp

[<RequireQualifiedAccess>]
module Async = 

  let singleton value = value |> async.Return

  let bind f x = async.Bind(x, f)

  let map f x = x |> bind (f >> singleton)

module Models =
    open Microsoft.Azure.Documents
    open Microsoft.Azure.Documents.Client

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

    type EndpointUrl = EndpointUrl of string

    type AccountKey = AccountKey of string

    type DatabaseId = DatabaseId of string

    type CollectionId = CollectionId of string

    type SubjectId = SubjectId of string

    type Email = Email of string

    type DocumentId = DocumentId of string

    type TesterFilter =
        | EmailFilter of Email
        | SubjectIdFilter of SubjectId

    type GetClient = 
        DatabaseOptions -> Async<DocumentClient>

    type GetTester = 
        IDocumentClient -> DatabaseId -> CollectionId -> TesterFilter -> Async<Tester option>

    type CreateDocument = 
        IDocumentClient -> DatabaseId -> CollectionId -> Tester -> Async<DocumentId>

    type ReplaceDocument = 
        IDocumentClient -> DatabaseId -> CollectionId -> DocumentId -> Tester -> Async<DocumentId>

    type SaveTestResults = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> DatabaseId -> CollectionId -> TestResults -> Async<DocumentId>

    type SaveTester = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> DatabaseId -> CollectionId -> Tester -> Async<DocumentId>
