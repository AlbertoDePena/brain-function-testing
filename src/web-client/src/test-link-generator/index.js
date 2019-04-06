import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

const initialState = () => {
  return {
    patient: {
      accountId: 'Test',
      patientId: '00004BFTTR042582',
      birthDate: ''
    }
  };
};

const actions = update => {
  return {
    changeBirthDate: value => {
      update({ patient: PS({ birthDate: value }) });
    }
  };
};

const view = (state, actions) => {
  return html`
  <div class="container">
    <form id="textLinkGeneratorForm" class="box">
      <div class="row">
        <h6 class="u-text-center">Generate Test Link</h6>
        <div class="column">
          <label for="accountId">Account ID</label>
          <input id="accountId" class="u-full-width" type="text" placeholder="Account ID" .value=${state.patient.accountId}
            readonly />
        </div>
        <div class="column">
          <label for="patientId">Patient ID</label>
          <input id="patientId" class="u-full-width" type="text" placeholder="Patient ID" .value=${state.patient.patientId}
            readonly />
        </div>
        <div class="column">
          <label for="birthDate">Birth Date</label>
          <input id="birthDate" class="u-full-width" type="date" .value=${state.patient.birthDate} @change=${e=>
          actions.changeBirthDate(e.target.value)} />
        </div>
        <div class="column">
          <button type="button" class="u-full-width button-primary">Generate</button>
        </div>
      </div>
    </form>
  </div>
  `;
};

const patientGenerator = { initialState, actions, view };

export default patientGenerator;
