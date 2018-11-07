namespace BFT.AzureFuncApp.Models 
{
    using Newtonsoft.Json;

    public class CnsvsReportDto
    {
        /// <summary>
        /// cnsvs_id - CNS Vital Sign Database identifier
        /// </summary>
        /// <value></value>
        [JsonProperty("cnsvs_id")]
        public string CnsvsId { get; set; }

        /// <summary>
        /// account_id - Customer account id
        /// </summary>
        /// <value></value>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// test_date - Date the assessment was given based on Patients computer (YYYY-MM-DD)
        /// </summary>
        /// <value></value>
        [JsonProperty("test_date")]
        public string TestDate { get; set; }

        /// <summary>
        /// test_time - Time assessment was given based on Patients computer (HH:MM:SS)
        /// </summary>
        /// <value></value>
        [JsonProperty("test_time")]
        public string TestTime { get; set; }

        /// <summary>
        /// timezone - Time zone based on Patients computer
        /// </summary>
        /// <value></value>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// gmt_test_date - Date the assessment was given based on server (YYYY-MM-DD)
        /// </summary>
        /// <value></value>
        [JsonProperty("gmt_test_date")]
        public string GmtTestDate { get; set; }

        /// <summary>
        /// gmt_test_time - Time assessment was given based on server (HH:MM:SS)
        /// </summary>
        /// <value></value>
        [JsonProperty("gmt_test_time")]
        public string GmtTestTime { get; set; }

        /// <summary>
        /// subject_id - Patient ID entered at test time
        /// </summary>
        /// <value></value>
        [JsonProperty("subject_id")]
        public string SubjectId { get; set; }

        /// <summary>
        /// birth_date - Patient date of birth entered at test time (YYYY-MM-DD)
        /// </summary>
        /// <value></value>
        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        /// <summary>
        /// gender - Patient Gender entered at test time
        /// </summary>
        /// <value></value>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// duration - Assessment duration in seconds
        /// </summary>
        /// <value></value>
        [JsonProperty("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Language assessment was presented in
        /// </summary>
        /// <value></value>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// report_data - XML string of domain data
        /// </summary>
        /// <value></value>
        [JsonProperty("report_data")]
        public string ReportData { get; set; }
    }
}