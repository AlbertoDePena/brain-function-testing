import '../styles.css';

import { html } from 'lit-html';
import { P } from 'patchinko/explicit';

import testLinkGenerator from '../test-link-generator';
import navigation from '../navigation';

const initialState = () =>
  P(
    {},
    navigation.initialState(),
    testLinkGenerator.initialState()
  );

const actions = update =>
  P(
    {},
    navigation.actions(update),
    testLinkGenerator.actions(update)
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
