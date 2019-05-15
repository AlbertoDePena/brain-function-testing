/**
 * @typedef {Object} REMOTE_LINK
 * @property {String} STATUS_CODE 
 * @property {String} MESSAGE
 * @property {String} URL
 */

 /**
 * @typedef {Object} TestLinkResult
 * @property {REMOTE_LINK} REMOTE_LINK 
 */

/**
 * @typedef {Object} Tester
 * @property {String} id
 * @property {String} subjectId
 * @property {String} firstName 
 * @property {String} lastName 
 * @property {String} email 
 * @property {String} dob
 * @property {String} dobDay
 * @property {String} dobMonth
 * @property {String} dobYear
 * @property {String} testStatus
 * @property {Date} testScheduleDate
 */

 /**
  * @typedef {Object} TestOptions
  * @property {Date} scheduleDate
  */

/**
 * @typedef {Object} State
 * @property {Tester} tester
 * @property {Boolean} scheduleTest
 */
