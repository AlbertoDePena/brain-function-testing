import { Redirect } from 'aurelia-router';

export class AuthorizeStep {

  run(navigationInstruction, next) {
    if (navigationInstruction.getAllInstructions().some(i => i.config.settings.auth)) {
      var isLoggedIn = true;// insert magic here;
      if (!isLoggedIn) {
        return next.cancel(new Redirect('login'));
      }
    }

    return next();
  }
}

