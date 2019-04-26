import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

import { navigateTo } from '../core/router';

const initialState = () => {
  return {
    email: ''
  };
};

const actions = update => {
  return {
    changeEmail: email => {
      update({ tester: PS({ email }) });
    },
    login: () => {
      navigateTo('test-configuration');
    }
  };
};

const view = (state, actions) => {
  return html`
    <div class="view">
      <form @submit=${(e) => { e.preventDefault(); actions.login(); }}>
        
        <div class="input-control">
          <label for="email">Email</label>
          <input id="email" type="email" .value=${state.email} @change=${(e)=>
            actions.changeEmail(e.target.value)} />
        </div>
        
        <div>
          <button type="submit">Next</button>
        </div>
      </form>
    </div>`;
};

const login = { initialState, actions, view };

export default login;
