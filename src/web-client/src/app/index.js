import '../styles.css';

import { html } from 'lit-html';

import conditions from '../conditions';
import temperature from '../temperature';
import navigation from '../navigation';

const initialState = () =>
  Object.assign(
    {},
    navigation.initialState(),
    conditions.initialState(),
    { air: temperature.initialState('Air') },
    { water: temperature.initialState('Water') }
  );

const actions = update =>
  Object.assign(
    {},
    navigation.actions(update),
    conditions.actions(update),
    temperature.actions(update)
  );

const view = (state, actions) => {
  return html`
    <div class="container">
      <nav class="navbar">
        <div class="container">
          <ul class="navbar-list">
            <li class="navbar-item">
              <a class="navbar-link" href="conditions">Conditions</a>
            </li>
            <li class="navbar-item">
              <a class="navbar-link" href="temperature?type=air"
                >Air Temperature</a
              >
            </li>
            <li class="navbar-item">
              <a class="navbar-link" href="temperature?type=water"
                >Water Temperature</a
              >
            </li>
          </ul>
        </div>
      </nav>
      <hr />
      <div>
        <button @click=${() => navigation.navigateTo('conditions')}>
          Conditions
        </button>
        <button @click=${() => navigation.navigateTo('temperature?type=air')}>
          Air Temperature
        </button>
        <button @click=${() => navigation.navigateTo('temperature?type=water')}>
          Water Temperature
        </button>
      </div>
      <hr />
      ${navigation.view(state, actions)}
      <hr />
      <pre>${JSON.stringify(state, null, 4)}</pre>
    </div>
  `;
};

const app = { initialState, actions, view };

export default app;
