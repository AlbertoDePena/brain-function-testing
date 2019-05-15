import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

import { Api } from '../../core/api';
import { notifyError } from '../../core/notifications';
import { setTesterState } from '../../core/state';

@inject(Router, Api)
export class MainViewModel {
 
  constructor(router, api) {
    this.router = router;
    this.api = api;
  }

  getTester(email) {
    if (!email) {
      notifyError('Email is required');
      return;
    }
    if (!email.match(/.+@.+/)) {
      notifyError('Email is invalid');
      return;
    }

    const that = this;

    function setState(tester) {
      setTesterState(tester);

      that.router.navigate('test-config');
    }

    function handleError(error) {
      if (error.statusCode === 0) {
        notifyError('Failed to contact Brain Function Testing server. Please contact system administrator.');
        return;
      }

      if (error.statusCode !== 404) {
        notifyError(error.response);
        return;
      }

      setTesterState({ email });

      that.router.navigate('test-config');
    }

    this.api.getTester(email).then(setState).catch(handleError);
  }
}
