module Tests

open System
open Xunit
open NSubstitute
open BFT.AzureFuncApp
open BFT.AzureFuncApp.Models
open Microsoft.Azure.Documents

let expectedDocumentId = Guid.NewGuid().ToString() |> DocumentId

let expectedDatabaseId = DatabaseId "my-db"

let expectedCollectionId = CollectionId "my-collection"

let expectedSubjectId = SubjectId "AEIOU1234567890"

let expectedEmail = Email "jmagan@demo.com"

let dummyTester = {
    id = Guid.NewGuid().ToString()
    firstName = "Juan"
    lastName = "Magan"
    email = "jmagan@demo.com"
    subjectId = "AEIOU1234567890"
    dob = "Jan/03/1990"
    testStatus = ""
    scheduleDate = ""
    testResults = []
}

let dummyTestResults = {
    cnsvsId = "0987654321"
    subjectId = "AEIOU1234567890"
    accountId = "12345"
    birthDate = "01/03/1990"
    gender = "Male"
    testDate = "04/19/2019"
    testTime = "30"
    timezone = "CST"
    gmtTestDate = "04/19/2019"
    gmtTestTime = "CST"
    duration = "500"
    language = "english_us"
    reportData = ""
}

let getMockedTester : GetTester =
    fun _ _ _ _ -> Some dummyTester |> Async.singleton

let mockCreateDocument : CreateDocument =
    fun _ _ _ _ -> Async.singleton expectedDocumentId

let mockReplaceDocument : ReplaceDocument =
    fun _ _ _ _ _ -> Async.singleton expectedDocumentId         

let saveTester tester =
    let client = Substitute.For<IDocumentClient>()
    Testers.saveTester 
        mockCreateDocument 
        mockReplaceDocument 
        getMockedTester 
        client 
        expectedDatabaseId 
        expectedCollectionId 
        tester |> Async.RunSynchronously |> ignore

let saveTestResults testResults =
    let client = Substitute.For<IDocumentClient>()
    Testers.saveTestResults
        mockCreateDocument 
        mockReplaceDocument 
        getMockedTester                 
        client
        expectedDatabaseId 
        expectedCollectionId 
        testResults |> Async.RunSynchronously |> ignore

[<Fact>]
let ``Saving a tester without first name should fail`` () =
    let execute () = saveTester { dummyTester with firstName = "" }

    Assert.Throws<ArgumentNullException>(execute)

[<Fact>]
let ``Saving a tester without last name should fail`` () =
    let execute () = saveTester { dummyTester with lastName = "" }

    Assert.Throws<ArgumentNullException>(execute)  

[<Fact>]
let ``Saving a tester without email should fail`` () =
    let execute () = saveTester { dummyTester with email = "" }

    Assert.Throws<ArgumentNullException>(execute)    

[<Fact>]
let ``Saving a tester with invalid email should fail`` () =
    let execute () = saveTester { dummyTester with email = "blahblah" }

    Assert.Throws<InvalidOperationException>(execute)     

[<Fact>]
let ``Saving a tester without dob should fail`` () =
    let execute () = saveTester { dummyTester with dob = "" }

    Assert.Throws<ArgumentNullException>(execute)  

[<Fact>]
let ``Saving a tester with invalid dbo should fail`` () =
    let execute () = saveTester { dummyTester with email = "blahblah" }

    Assert.Throws<InvalidOperationException>(execute)   

[<Fact>]
let ``Saving a tester with valid data should succeed`` () =
    saveTester dummyTester               

[<Fact>]
let ``Saving test results without subject ID should fail`` () =
    let execute () = saveTestResults { dummyTestResults with subjectId = "" }

    Assert.Throws<ArgumentNullException>(execute)  

[<Fact>]
let ``Saving test results with valid data should succeed`` () =
    saveTestResults dummyTestResults      