import { inject, LogManager } from 'aurelia-framework';

import { Api } from '../../core/api';
import { notifyError } from '../../core/notifications';
import { getState } from '../../core/state';

@inject(Api)
export class ConfirmationViewModel {

  constructor(api) {
    this.api = api;
    this.logger = LogManager.getLogger('ConfirmationViewModel');
    this.instructionsRead = false;
    this.usingComputer = false;
    this.testLaunched = false;
    this.isCurrentSession = false;
    this.tester = {};
  }

  activate() {
    const state = getState();
    this.tester = state.tester || {};
    this.isCurrentSession = state.isCurrentSession || false;
  }

  launchTest() {
    if (!(this.instructionsRead && this.usingComputer) || this.testLaunched) return;

    const that = this;

    function launch(link) {
      that.testLaunched = true;
      that.logger.info('Get Test Link', link);
      window.open(link.URL, '_blank');
    }

    function handleError(error) {
      that.logger.error('Get Test Link', error);
      notifyError('Failed to launch test. Please contact system administrator.');
    }

    const testConfig = getState().testConfig;

    that.api.getTestLink(that.tester.email, testConfig).then(launch).catch(handleError);
  }
}
