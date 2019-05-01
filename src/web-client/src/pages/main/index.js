import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';
import { Api } from '../../core/api';
import { notifyError } from '../../core/notifications';
import { tryCatch } from '../../core/common';
import { setTesterState } from '../../core/state';

@inject(Router, Api)
export class MainViewModel {

  constructor(router, api) {
    this.router = router;
    this.api = api;
  }

  async getTester(email) {
    if (!email) {
      notifyError('Email is required');
      return;
    }
    if (!email.match(/.+@.+/)) {
      notifyError('Email is invalid');
      return;
    }

    const { error, result : tester } = await tryCatch(this.api.getTester(email));
    if (error) {
      if (error.statusCode == 404) {
        setTesterState({ email });
      } else {
        notifyError(error.response);
        return;
      }
    } else {
      setTesterState(tester);
    }
    
    this.router.navigate('test-config');
  }
}
