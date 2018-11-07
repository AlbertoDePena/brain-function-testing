namespace BFT.AzureFuncApp
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;
    using Models;
    using Core;

    public static class PostCnsvsData
    {
        [FunctionName("post-cnsvs-data-http-trigger")]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req, ILogger log)
        {
            try
            {
                log.LogInformation("Posting CNSVS data...");

                var formData = await req.Content.ReadAsFormDataAsync().ConfigureAwait(false);

                if (formData == null)
                {
                    throw new InvalidOperationException($"'{nameof(formData)}' is required (Content-Type = application/x-www-form-urlencoded)");
                }

                var dto = new CnsvsReportDto()
                {
                    CnsvsId = formData["cnsvs_id"],
                    AccountId = formData["account_id"],
                    TestDate = formData["test_date"],
                    TestTime = formData["test_time"],
                    Timezone = formData["timezone"],
                    GmtTestDate = formData["gmt_test_date"],
                    GmtTestTime = formData["gmt_test_time"],
                    SubjectId = formData["subject_id"],
                    BirthDate = formData["birth_date"],
                    Gender = formData["gender"],
                    Duration = formData["duration"],
                    Language = formData["language"],
                    ReportData = formData["report_data"]
                };

                log.LogInformation($"cnsvs_id: {dto.CnsvsId}");
                log.LogInformation($"account_id: {dto.AccountId}");
                log.LogInformation($"test_date: {dto.TestDate}");
                log.LogInformation($"test_time: {dto.TestTime}");
                log.LogInformation($"timezone: {dto.Timezone}");
                log.LogInformation($"gmt_test_date: {dto.GmtTestDate}");
                log.LogInformation($"gmt_test_time: {dto.GmtTestTime}");
                log.LogInformation($"subject_id: {dto.SubjectId}");
                log.LogInformation($"birth_date: {dto.BirthDate}");
                log.LogInformation($"gender: {dto.Gender}");
                log.LogInformation($"duration: {dto.Duration}");
                log.LogInformation($"language: {dto.Language}");
                log.LogInformation($"report_data: {dto.ReportData}");

                using (var service =
                    new DatabaseService(
                        SettingsProvider.GetDatabaseEndpointUrl(), SettingsProvider.GetDatabaseAccountKey(), SettingsProvider.GetDatabaseId()))
                {
                    var id = await service.InsertAsync(Collections.CnsvsReport, dto);

                    log.LogInformation($"CNSVS report inserted - ID: {id}");
                }

                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                log.LogError(e, "Opps something went wrong!");

                return req.CreateResponse(HttpStatusCode.BadRequest, e.ToDetails());
            }
        }
    }
}