import { Redirect } from 'aurelia-router';
import { getState } from '../core/state';

export class AuthorizeStep {

  run(navigationInstruction, next) {
    const tester = getState().tester || {};
    const hasTestResults = (tester.testResults || []).length;

    const isMainNav = navigationInstruction.config.name === 'main';
    const isConfirmationNav = navigationInstruction.config.name === 'confirmation';
    const isStatusNav = navigationInstruction.config.name === 'status';

    if (tester.email) {
      if (!isStatusNav && hasTestResults) {
        return next.cancel(new Redirect('status'));
      }
  
      if (!isConfirmationNav && !hasTestResults) {
        return next.cancel(new Redirect('confirmation'));
      }
    }
    else if (!isMainNav) {
      return next.cancel(new Redirect('main'));
    }

    return next();
  }
}

