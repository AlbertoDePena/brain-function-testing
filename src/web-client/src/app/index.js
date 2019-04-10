import '../styles.css';

import { html } from 'lit-html';
import { P } from 'patchinko/explicit';

import test from '../test';
import navigation from '../navigation';

const initialState = () =>
  P(
    {},
    navigation.initialState(),
    test.initialState()
  );

const actions = update =>
  P(
    {},
    navigation.actions(update),
    test.actions(update)
  );

const view = (state, actions) => {
  return html`
      <div class="image-container">
        <img src="http://127.0.0.1:5500/src/web-client/dist/bft.png">
      </div>
      ${navigation.view(state, actions)}
      <div class="debug">
        <pre>${JSON.stringify(state, null, 4)}</pre>
      </div>
  `;
};

const app = { initialState, actions, view };

export default app;
