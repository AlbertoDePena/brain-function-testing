import { getUrlParameter } from './common';

let state = {};


/**
 * 
 * @param {Tester} tester 
 */
export function setTesterState(tester) {      
  state = { ...state, tester };
}

/**
 * @returns {State} state
 */
export function getState() {
  const email = getUrlParameter('email');

  if (!state.tester) {
    return {};
  }

  if (state.tester.email !== email) {
    return {};
  }
  
  return state;
}
