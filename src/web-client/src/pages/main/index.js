import { Router, activationStrategy } from 'aurelia-router';
import { inject } from 'aurelia-framework';

import { getDays, getMonths, getYears } from '../../core/common';
import { getState, setTesterState, setIsCurrentSession } from '../../core/state';
import { notifyError } from '../../core/notifications';
import { Api } from '../../core/api';

@inject(Router, Api)
export class MainViewModel {

  constructor(router, api) {
    this.router = router;
    this.api = api;

    this.tester = {};
    this.days = getDays();
    this.months = getMonths();
    this.years = getYears();
  }

  determineActivationStrategy() {
    return activationStrategy.replace;
  }

  activate() {
   this.tester = getState().tester || {};
  }

  saveTester() {
    if (!this.tester.email) {
      notifyError('Email is required');
      return;
    }
    if (!this.tester.email.match(/.+@.+/)) {
      notifyError('Email is invalid');
      return;
    }
    if (!this.tester.firstName) {
      notifyError('First Name is required');
      return;
    }
    if (!this.tester.lastName) {
      notifyError('Last Name is required');
      return;
    }
    if (this.tester.dobDay === 'Day') {
      notifyError('DOB Day is required');
      return;
    }
    if (this.tester.dobMonth === 'Month') {
      notifyError('DOB Month is required');
      return;
    }
    if (this.tester.dobYear === 'Year') {
      notifyError('DOB Year is required');
      return;
    }

    const that = this;

    function setState(testerId) {
      that.tester.id = testerId;
      setTesterState(that.tester);
      setIsCurrentSession(true);
      that.router.navigate('confirmation');
    }

    function handleError(error) {
      notifyError(error.response);
    }

    that.api.saveTester(that.tester).then(setState).catch(handleError);
  }
}
