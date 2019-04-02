namespace BFT.AzureFuncApp

open Microsoft.Azure.WebJobs
open DurableFunctions.FSharp
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs.Extensions.Http
open FSharp.Control.Tasks.ContextInsensitive
open Models

[<RequireQualifiedAccess>]
module Activities =

  let sayHello = 
    Activity.define "SayHelloActivity" (sprintf "Hello typed %s!")

[<RequireQualifiedAccess>]
module Orchestrators =

  let processTestResults = orchestrator {
    let! hello1 = Activity.call Activities.sayHello "Tokyo"
    let! hello2 = Activity.call Activities.sayHello "Seattle"
    let! hello3 = Activity.call Activities.sayHello "London"

    return [hello1; hello2; hello3]
  }

module Functions =

  [<FunctionName("SayHelloActivity")>]
  let SayHelloActivity([<ActivityTrigger>] name) = 
    Activities.sayHello.run name

  [<FunctionName("process-test-results-orchestration")>]
  let ProcessTestResultsOrchestration ([<OrchestrationTrigger>] context: DurableOrchestrationContext) = 
    Orchestrator.run (Orchestrators.processTestResults, context)

  [<FunctionName("post-test-results-http-trigger")>]
  let PostTestResultsHttpTriger 
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

      let! orchestrationId = starter.StartNewAsync ("process-test-results-orchestration", testResults)

      log.LogInformation(sprintf "Started orchestration with ID = '{%s}'." orchestrationId)

      return starter.CreateCheckStatusResponse(req, orchestrationId)
    }
