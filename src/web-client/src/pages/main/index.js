import { Router, activationStrategy } from 'aurelia-router';
import { inject } from 'aurelia-framework';

import { getDays, getMonths, getYears } from '../../core/common';
import { setTesterState } from '../../core/state';
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

  activate(params) {
    const email = params.email,
      firstName = params.fn,
      lastName = params.ln,
      testConfig = params.config;

    if (!email) {
      return setTimeout(() => {
        notifyError('Please provide email in query parameter');
      });
    }

    const that = this;

    function setState(tester) {
      setTesterState(tester);

      const hasTestResults = (tester.testResults || []).length;

      if (hasTestResults) {
        that.router.navigate('status');
      } else {
        that.router.navigate('confirmation');
      }
    }

    function handleError(error) {
      if (error.statusCode === 0) {
        return setTimeout(() => {
          notifyError('Failed to contact Brain Function Testing server. Please contact system administrator.');
        });
      }

      if (error.statusCode !== 404) {
        return setTimeout(() => {
          notifyError(error.response);
        });
      }

      if (!firstName) {
        return setTimeout(() => {
          notifyError('Please provide first name in query parameter');
        });
      }

      if (!lastName) {
        return setTimeout(() => {
          notifyError('Please provide last name in query parameter');
        });
      }

      that.tester = { firstName, lastName, email, testConfig };
    }

    return this.api.getTester(email).then(setState).catch(handleError);
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
      that.router.navigate('confirmation');
    }

    function handleError(error) {
      notifyError(error.response);
    }

    that.api.saveTester(that.tester).then(setState).catch(handleError);
  }
}
