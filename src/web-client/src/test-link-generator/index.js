import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

const initialState = () => {
  return {
    testLinkRequest: {
      accountId: 'Test',
      userName: '',
      password: '',
      patientId: '00004BFTTR042582',
      dobYear: '',
      dobMonth: '',
      dobDay: '',
      testConfiguration: '',
      testLanguage: 'english_us'
    }
  };
};

const actions = update => {
  return {
    togglePrecipitations: value => {
      update({ conditions: PS({ precipitations: value }) });
    },
    changeSky: value => {
      update({ conditions: PS({ sky: value }) });
    }
  };
};

const view = (state, actions) => {
  return html`
  <div class="container">
    <form>
      <div class="row">
        <h6 class="u-text-center">Generate Test Link</h6>
        <div class="one-third column">
          <label for="accountId">Account ID</label>
          <input id="accountId" class="u-full-width" type="text" placeholder="Account ID" .value=${state.testLinkRequest.accountId} />
        </div>
        <div class="one-third column">
          <label for="userName">User Name</label>
          <input id="userName" class="u-full-width" type="text" placeholder="User Name" />
        </div>
        <div class="one-third column">
          <label for="password">Password</label>
          <input id="password" class="u-full-width" type="password" placeholder="Password" />
        </div>
        <div class="one-third column">
          <label for="patientId">Patient ID</label>
          <input id="patientId" class="u-full-width" type="text" placeholder="Patient ID" .value=${state.testLinkRequest.patientId} readonly />
        </div>
        <div class="one-third column">
          <label for="dobYear">DOB Year</label>
          <input id="dobYear" class="u-full-width" type="text" placeholder="DOB Year" />
        </div>
        <div class="one-third column">
          <label for="dobMonth">DOB Month</label>
          <input id="dobMonth" class="u-full-width" type="text" placeholder="DOB Month" />
        </div>
        <div class="one-third column">
          <label for="dobDay">DOB Day</label>
          <input id="dobDay" class="u-full-width" type="text" placeholder="DOB Day" />
        </div>
        <div class="one-third column">
          <label for="testConfiguration">Test Configuration</label>
          <input id="testConfiguration" class="u-full-width" type="text" placeholder="Test Configuration" />
        </div>
        <div class="one-third column">
          <label for="testLanguage">Test Language</label>
          <input id="testLanguage" class="u-full-width" type="text" placeholder="Test Language" .value=${state.testLinkRequest.testLanguage}
            readonly />
        </div>
        <div class="twelve columns">
          <button type="button" class="button-primary">Generate</button>
        </div>
      </div>
    </form>
  </div>
  `;
};

const testLinkGenerator = { initialState, actions, view };

export default testLinkGenerator;
