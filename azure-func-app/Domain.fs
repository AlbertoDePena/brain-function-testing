namespace BFT.AzureFuncApp

module Models =

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

    