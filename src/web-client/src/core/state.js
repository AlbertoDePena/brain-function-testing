let state = {};

/**
 * 
 * @param {Tester} tester 
 */
export function setTesterState(tester) {      
  state = { ...state, tester };
}

/**
 * 
 * @param {Boolean} scheduleTest 
 */
export function setScheduleTestState(scheduleTest) {
  state = { ...state, scheduleTest };
}

/**
 * @returns {State} state
 */
export function getState() {
  return state;
}
