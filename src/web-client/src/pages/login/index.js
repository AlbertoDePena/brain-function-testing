import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

@inject(Router)
export class LoginViewModel {

  constructor(router) {
    this.router = router;
    this.email = '';
  }

  login() {
    this.router.navigate('test-config');
  }
}
