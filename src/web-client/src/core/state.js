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
 * @returns {State} state
 */
export function getState() {
  return state;
}
