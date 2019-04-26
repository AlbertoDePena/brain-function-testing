import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

import { getDays, getMonths, getYears } from '../../core/common';

@inject(Router)
export class TestConfigViewModel {

  constructor(router) {
    this.router = router;
    this.scheduleTest = false;
    this.tester = {};
    this.days = getDays();
    this.months = getMonths();
    this.years = getYears();
  }

  save() {
    this.router.navigate('confirmation');
  }
}
