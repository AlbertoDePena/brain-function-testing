import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

import { getDays, getMonths, getYears } from '../../core/common';
import { getState, setTesterState, setScheduleTestState } from '../../core/state';
import { notifyError } from '../../core/notifications';
import { Api } from '../../core/api';

@inject(Router, Api)
export class TestConfigViewModel {
  
  constructor(router, api) {
    this.router = router;
    this.api = api;

    this.scheduleTest = false;
    this.tester = {};
    this.days = getDays();
    this.months = getMonths();
    this.years = getYears();
  }

  activate() {
    this.tester = getState().tester;
  }

  saveTester() {
    if (!this.tester.email) {
      notifyError('Email is required');
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
      setScheduleTestState(that.scheduleTest);

      that.router.navigate('confirmation');
    }

    function handleError(error) {
      notifyError(error.response);
    }

    that.api.saveTester(that.tester).then(setState).catch(handleError);
  }
}
