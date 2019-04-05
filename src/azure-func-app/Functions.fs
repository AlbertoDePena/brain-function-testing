namespace BFT.AzureFuncApp

open System
open Microsoft.Azure.WebJobs
open DurableFunctions.FSharp
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs.Extensions.Http
open FSharp.Control.Tasks.ContextInsensitive
open Models
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module Settings =

  let getAppSettings name =
    let value = Environment.GetEnvironmentVariable(name)
    if String.IsNullOrWhiteSpace(value) then
      invalidOp (sprintf "App setting %s not found" name)
    else value

  let getDatabaseOptions () = {
    EndpointUrl = getAppSettings "DatabaseEndpointUrl"
    AccountKey = getAppSettings "DatabaseAccountKey"
    DatabaseId = "testers"
    CollectionId = "testers"
  }

[<RequireQualifiedAccess>]
module Activities =

  let saveTestResults =

      let run testResults =
        let options = Settings.getDatabaseOptions ()

        TesterAPI.saveTestResults options testResults |> Async.StartAsTask

      Activity.defineTask "save-test-results-activity" run

[<RequireQualifiedAccess>]
module Orchestrators =

  let processTestResults input = orchestrator {
    let! _ = Activity.call Activities.saveTestResults input

    return ()
  }

module Functions =

  [<FunctionName("save-test-results-activity")>]
  let SaveTestResultsActivity([<ActivityTrigger>] input) = 
    Activities.saveTestResults.run input

  [<FunctionName("process-test-results-orchestration")>]
  let ProcessTestResultsOrchestration ([<OrchestrationTrigger>] context: DurableOrchestrationContext) = 
    Orchestrator.run (Orchestrators.processTestResults, context)

  [<FunctionName("save-test-results-http-trigger")>]
  let SaveTestResultsHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequestMessage, 
     [<OrchestrationClient>] starter: DurableOrchestrationClient, 
     log: ILogger) =
    task {
      log.LogInformation("Posting test results...")

      let! formData = req.Content.ReadAsFormDataAsync()

      if isNull formData then invalidOp "test result payload is required (Content-Type = application/x-www-form-urlencoded)"

      let testResults = {
          cnsvs_id = formData.["cnsvs_id"]
          account_id = formData.["account_id"]
          test_date = formData.["test_date"]
          test_time = formData.["test_time"]
          timezone = formData.["timezone"]
          gmt_test_date = formData.["gmt_test_date"]
          gmt_test_time = formData.["gmt_test_time"]
          subject_id = formData.["subject_id"]
          birth_date = formData.["birth_date"]
          gender = formData.["gender"]
          duration = formData.["duration"]
          language = formData.["language"]
          report_data = formData.["report_data"]
        }

      if String.IsNullOrWhiteSpace(testResults.subject_id) then invalidOp "Test results must have a subject ID"

      let! orchestrationId = starter.StartNewAsync ("process-test-results-orchestration", testResults)

      log.LogInformation(sprintf "Started orchestration with ID = '{%s}'." orchestrationId)

      return starter.CreateCheckStatusResponse(req, orchestrationId)
    }

  [<FunctionName("get-tester-http-trigger")>]
  let GetTesterHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequest, log: ILogger) =
    async {
      log.LogInformation("Getting tester data...")

      let subjectId = req.Query.["subject_id"].ToString()

      if String.IsNullOrWhiteSpace(subjectId) then
        return BadRequestObjectResult("subject_id query param is required") :> IActionResult
      else
        let options = Settings.getDatabaseOptions()

        let! testerOption = TesterAPI.getTester options (SubjectId subjectId)

        return
          match testerOption with
          | None -> NotFoundObjectResult(sprintf "Tester with subject ID '%s' not found" subjectId) :> IActionResult
          | Some tester -> OkObjectResult(tester) :> IActionResult
    } |> Async.RunSynchronously

  [<FunctionName("save-tester-http-trigger")>]
  let SaveTesterHttpTriger 
    ([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequest, log: ILogger) =
    async {
      log.LogInformation("Saving tester...")

      let! json = req.ReadAsStringAsync() |> Async.AwaitTask

      let tester = JsonConvert.DeserializeObject<Tester>(json)

      if box tester |> isNull then invalidOp "Tester payload is required"

      let options = Settings.getDatabaseOptions()

      let! _ = TesterAPI.saveTester options tester

      return NoContentResult()
    } |> Async.RunSynchronously
