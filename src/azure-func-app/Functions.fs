namespace BFT.AzureFuncApp

open System
open Microsoft.Azure.WebJobs
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs.Extensions.Http
open Models
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module Settings =

  let private getAppSettings name =
    let value = Environment.GetEnvironmentVariable(name)
    if String.IsNullOrWhiteSpace(value) then
      invalidOp (sprintf "App setting %s not found" name)
    else value

  let get () = {
    DbEndpointUrl = getAppSettings "DB.EndpointUrl"
    DbAccountKey = getAppSettings "DB.AccountKey"
    BftAccount = getAppSettings "BFT.Account"
    BftUsername = getAppSettings "BFT.Username"
    BftPassword = getAppSettings "BFT.Password"
    BftEndpointUrl = getAppSettings "BFT.EndpointUrl"
  }

module Functions =

  [<FunctionName("save-test-results-http-trigger")>]
  let SaveTestResultsHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequestMessage, log: ILogger) =
    async {
      log.LogInformation("Posting test results...")

      let! formData = req.Content.ReadAsFormDataAsync() |> Async.AwaitTask

      if isNull formData then invalidOp "test result payload is required (Content-Type = application/x-www-form-urlencoded)"

      let testResults = {
          cnsvsId = formData.["cnsvs_id"]
          accountId = formData.["account_id"]
          testDate = formData.["test_date"]
          testTime = formData.["test_time"]
          timezone = formData.["timezone"]
          gmtTestDate = formData.["gmt_test_date"]
          gmtTestTime = formData.["gmt_test_time"]
          subjectId = formData.["subject_id"]
          birthDate = formData.["birth_date"]
          gender = formData.["gender"]
          duration = formData.["duration"]
          language = formData.["language"]
          domainData = formData.["domain_data"]
          reportData = formData.["report_data"]
        }

      if String.IsNullOrWhiteSpace(testResults.subjectId) then invalidOp "Test results must have a subject ID"

      let settings = Settings.get ()

      let! (DocumentId documentId) = TesterAPI.saveTestResults settings testResults

      return OkObjectResult(documentId)
    } |> Async.StartAsTask

  [<FunctionName("get-tester-http-trigger")>]
  let GetTesterHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequest, log: ILogger) =
    async {
      log.LogInformation("Getting tester data...")

      let email = req.Query.["email"].ToString()

      if String.IsNullOrWhiteSpace(email) then
        return BadRequestObjectResult("email query param is required") :> IActionResult
      else
        let settings = Settings.get()

        let! testerOption = TesterAPI.getTester settings (Email email)

        return
          match testerOption with
          | None -> NotFoundObjectResult(sprintf "Tester with email '%s' not found" email) :> IActionResult
          | Some tester -> OkObjectResult(tester) :> IActionResult
    } |> Async.StartAsTask

  [<FunctionName("get-test-link-http-trigger")>]
  let GetTestLinkHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequest, log: ILogger) =
    async {
      log.LogInformation("Getting test link...")

      let email = req.Query.["email"].ToString()
      let testConfig = req.Query.["config"].ToString()

      if String.IsNullOrWhiteSpace(email) then
        return BadRequestObjectResult("email query param is required") :> IActionResult
      elif String.IsNullOrWhiteSpace(testConfig) then
        return BadRequestObjectResult("test configuration query param is required") :> IActionResult
      else
        let settings = Settings.get()
        
        let! (TestLinkJson json) = TesterAPI.getTestLink settings (Email email) (TestConfig testConfig)

        return OkObjectResult(json) :> IActionResult
    } |> Async.StartAsTask    

  [<FunctionName("save-tester-http-trigger")>]
  let SaveTesterHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequest, log: ILogger) =
    async {
      log.LogInformation("Saving tester...")

      let! json = req.ReadAsStringAsync() |> Async.AwaitTask

      let tester = JsonConvert.DeserializeObject<Tester>(json)

      if box tester |> isNull then invalidOp "Tester payload is required"

      let settings = Settings.get()

      let! (DocumentId documentId) = TesterAPI.saveTester settings tester

      return OkObjectResult(documentId)
    } |> Async.StartAsTask
