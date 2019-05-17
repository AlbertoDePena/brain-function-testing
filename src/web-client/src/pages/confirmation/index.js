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
    this.tester = {};
  }

  activate() {
    this.tester = getState().tester || {};
  }

  launchTest() {
    if (!(this.instructionsRead && this.usingComputer) || this.testLaunched) return;

    const that = this;

    function launch(linkUrl) {
      that.testLaunched = true;
      window.open(linkUrl, '_blank');
    }

    function handleError(error) {
      that.logger.error('Get Test Link', error);
      notifyError('Failed to launch test. Please contact system administrator.');
    }

    function update() {
      that.tester.testStatus = 'Incomplete';
      that.api.saveTester(that.tester)
        .then(() => that.logger.info('Test in progress...'))
        .catch(error => that.logger.error('Failed to update tester status', error));
    }

    that.api.getTestLink(that.tester.email).then(launch).then(update).catch(handleError);
  }
}
