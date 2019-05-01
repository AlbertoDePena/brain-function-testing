import { Redirect } from 'aurelia-router';

import { getState } from '../core/state';

export class AuthorizeStep {

  run(navigationInstruction, next) {    
    const tester = getState().tester || {};
    const isNavigatingToMain = navigationInstruction.config.name === 'main';

    if (tester.email && isNavigatingToMain) {
      return next.cancel();
    }

    if (!tester.email && !isNavigatingToMain) {
      return next.cancel(new Redirect('main'));
    }

    return next();
  }
}

