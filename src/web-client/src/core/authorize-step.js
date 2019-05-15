import { Redirect } from 'aurelia-router';

import { getState } from '../core/state';

export class AuthorizeStep {

  run(navigationInstruction, next) {    
    const tester = getState().tester || {};
    const isNavigatingToMain = navigationInstruction.config.name === 'main';
    const isNavifatingToStatus = navigationInstruction.config.name === 'status';

    if (tester.email && isNavigatingToMain) {
      return next.cancel();
    }

    if (!tester.email && !isNavigatingToMain) {
      return next.cancel(new Redirect('main'));
    }

    if (tester.testStatus && !isNavifatingToStatus) {
      return next.cancel(new Redirect('status'));
    }

    return next();
  }
}

