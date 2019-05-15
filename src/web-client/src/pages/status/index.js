import { getState } from '../../core/state';

export class StatusViewModel {

  constructor() {
    this.tester = {};
  }

  activate() {
    this.tester = getState().tester;
  }
}
