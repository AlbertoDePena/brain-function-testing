import '../styles.css';

import { html } from 'lit-html';

import testLinkGenerator from '../test-link-generator';
import navigation from '../navigation';

const initialState = () =>
  Object.assign(
    {},
    navigation.initialState(),
    testLinkGenerator.initialState()
  );

const actions = update =>
  Object.assign(
    {},
    navigation.actions(update),
    testLinkGenerator.actions(update)
  );

const view = (state, actions) => {
  return html`
    <div class="container">
      <nav class="navbar">
        <div class="container">
          <ul class="navbar-list">
            <li class="navbar-item">
              <a class="navbar-link" href="generate-test-link">Generate Test Link</a>
            </li>
            <li class="navbar-item">
              <a class="navbar-link" href="about">About</a>
            </li>
          </ul>
        </div>
      </nav>
      <div class="page">
        ${navigation.view(state, actions)}
      </div>
      <hr />
      <pre>${JSON.stringify(state, null, 4)}</pre>
    </div>
  `;
};

const app = { initialState, actions, view };

export default app;
