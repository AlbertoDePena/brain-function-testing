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
    type RemoteLink = {
        STATUS_CODE : string
        MESSAGE : string
        URL : string
    }

    [<CLIMutable>]
    type LinkResult = {
        REMOTE_LINK : RemoteLink
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
        domainData : string
        reportData : string
    }

    [<CLIMutable>]
    type Tester = {
        id : string
        firstName : string
        lastName : string
        email : string
        dob: string
        testResults : TestResults list
    }

    [<CLIMutable>]
    type AppSettings = {
        DbEndpointUrl : string
        DbAccountKey : string
        BftAccount : string
        BftUsername : string
        BftPassword : string
        BftEndpointUrl : string
    }

    type TestLinkJson = TestLinkJson of string

    type TestConfig = TestConfig of string

    type Email = Email of string

    type DocumentId = DocumentId of string

    type GetClient = 
        AppSettings -> Async<DocumentClient>

    type GetTester = 
        IDocumentClient -> Email -> Async<Tester option>

    type CreateDocument = 
        IDocumentClient -> Tester -> Async<DocumentId>

    type ReplaceDocument = 
        IDocumentClient -> DocumentId -> Tester -> Async<DocumentId>

    type SaveTestResults = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> TestResults -> Async<DocumentId>

    type SaveTester = 
        CreateDocument -> ReplaceDocument -> GetTester -> IDocumentClient -> Tester -> Async<DocumentId>

    type GetTestLink =
        GetTester -> IDocumentClient -> AppSettings -> Email -> TestConfig -> Async<TestLinkJson>        
