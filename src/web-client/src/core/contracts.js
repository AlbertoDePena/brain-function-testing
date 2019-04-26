/**
 * @typedef {Object} TestLinkRequest
 * @property {String} request = rtl
 * @property {String} account = Your Account Number
 * @property {String} username = Your Username, CNSVS Online user type must be Account Admin or Test Coordinator
 * @property {String} password = Your Password
 * @property {String} subject_id = Patient Unique Identifier, must conform to the CNSVS Online Demography setting in your account
 * @property {String} dob_year = Year Patient was born, 4 digits (1999)
 * @property {String} dob_month = Month Patient was born, short textual representation, 3 characters, (Jan - Dec)
 * @property {String} dob_day = Day Patient was born, (01 - 31)
 * @property {String} test_config = Test configuration number (0-9) or a list of test codes delimited by a colon
 * @property {String} test_lang = Language to present the test in, defaults to english_us
 */

/**
 * @typedef {Object} Tester
 * @property {String} firstName 
 * @property {String} lastName 
 * @property {String} email 
 * @property {String} dobMonth
 * @property {String} dobDay
 * @property {String} dobYear
 */

 /**
  * @typedef {Object} TestOptions
  * @property {Date} scheduleDate
  */
