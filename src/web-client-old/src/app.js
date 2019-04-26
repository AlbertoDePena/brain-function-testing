import './content/styles.css';

import { html } from 'lit-html';
import { P } from 'patchinko/explicit';

import login from './pages/login';
import testConfiguration from './pages/test-configuration';
import confirmation from './pages/confirmation';

const initialState = () =>
  P(
    {},
    { route: '' },
    login.initialState(),
    testConfiguration.initialState(),
    confirmation.initialState()
  );

const actions = update =>
  P(
    {},
    { updateRoute: route => update({ route }) },
    login.actions(update),
    testConfiguration.actions(update),
    confirmation.actions(update)   
  );

const page = (state, actions) => {
  switch (state.route) {
    case '#/test-configuration':
      return testConfiguration.view(state, actions);

    case '#/confirmation':
      return confirmation.view(state, actions);

    default:
      return login.view(state, actions);
  }
};

const view = (state, actions) => {
  return html`
      <div class="image-container">
        <img src="bft.png">
      </div>
      <main>
        ${page(state, actions)}
      </main>
      <div class="debug">
        <pre>${JSON.stringify(state, null, 4)}</pre>
      </div>
  `;
};

const app = { initialState, actions, view };

export default app;
