import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

import { generateTestLink } from '../core/api';

const initialState = () => {
  return {
    patient: {
      firstName: '',
      lastName: '',
      birthDate: ''
    }
  };
};

const actions = update => {
  return {
    changeFirstName: value => {
      update({ patient: PS({ firstName: value }) });
    },
    changeLastName: value => {
      update({ patient: PS({ lastName: value }) });
    },
    changeBirthDate: value => {
      update({ patient: PS({ birthDate: value }) });
    },
    generate: (firstName, lastName, birthDate) => {
      generateTestLink(firstName, lastName, birthDate);
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
          <label for="firstName">First Name</label>
          <input id="firstName" class="u-full-width" type="text" placeholder="First Name" .value=${state.patient.firstName}
            @change=${e=> actions.changeFirstName(e.target.value)} />
        </div>
        <div class="column">
          <label for="lastName">Last Name</label>
          <input id="lastName" class="u-full-width" type="text" placeholder="Last Name" .value=${state.patient.lastName}
            @change=${e=> actions.changeLastName(e.target.value)} />
        </div>
        <div class="column">
          <label for="birthDate">Birth Date</label>
          <input id="birthDate" class="u-full-width" type="date" .value=${state.patient.birthDate} @change=${e =>
      actions.changeBirthDate(e.target.value)} />
        </div>
        <div class="column">
          <button type="button" class="u-full-width button-primary" @click=${() => actions.generate(state.firstName,
            state.lastName, state.birthDate)} >Generate</button>
        </div>
      </div>
    </form>
  </div>
  `;
};

const patientGenerator = { initialState, actions, view };

export default patientGenerator;
