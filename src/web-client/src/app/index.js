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
      <nav class="navbar">
        <ul class="navbar-list">
          <li class="navbar-item">
            <a class="navbar-link" href="generate-test-link">Generate Test Link</a>
          </li>
          <li class="navbar-item">
            <a class="navbar-link" href="about">About</a>
          </li>
        </ul>
      </nav>
      ${navigation.view(state, actions)}
      <hr />
      <div class="container">
        <div class="twelve columns">
          <pre>${JSON.stringify(state, null, 4)}</pre>
        </div>
      </div>
  `;
};

const app = { initialState, actions, view };

export default app;
