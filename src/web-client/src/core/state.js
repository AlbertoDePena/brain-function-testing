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
 * @param {String} testConfig 
 */
export function setTestConfigState(testConfig) {
  state = { ...state, testConfig };
}

/**
 * 
 * @param {Boolean} isCurrentSession 
 */
export function setIsCurrentSession(isCurrentSession) {
  state = { ...state, isCurrentSession };
}

/**
 * @returns {State} state
 */
export function getState() {
  return state;
}
