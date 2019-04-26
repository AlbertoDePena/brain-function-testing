import { html } from 'lit-html';
import { cache } from 'lit-html/directives/cache';
import { PS } from 'patchinko/explicit';

import { navigateTo } from '../core/router';

const initialState = () => {
  return {
    scheduleTest: false,
    tester: {
      firstName: '',
      lastName: '',
      email: '',
      dobMonth: '',
      dobDay: '',
      dobYear: ''
    }
  };
};

const actions = update => {
  return {
    changeFirstName: firstName => {
      update({ tester: PS({ firstName }) });
    },
    changeLastName: lastName => {
      update({ tester: PS({ lastName }) });
    },
    changeEmail: email => {
      update({ tester: PS({ email }) });
    },
    changeDobMonth: dobMonth => {
      update({ tester: PS({ dobMonth }) });
    },
    changeDobDay: dobDay => {
      update({ tester: PS({ dobDay }) });
    },
    changeDobYear: dobYear => {
      update({ tester: PS({ dobYear }) });
    },
    takeTestNow: scheduleTest => {
      update({ scheduleTest });
    },
    generate: tester => {
      navigateTo('confirmation');
    }
  };
};

const months = () => {
  const dict = [
    'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
  ];

  return (dict.map(month => html`<option value=${month}>${month}</option>`));
};

const days = () => {
  return ([...Array(31).keys()].map(day => {
    let item = day + 1;
    item = item >= 10 ? item : `0${item}`;
    return html`<option value=${item}>${item}</option>`;
  }));
};

const years = () => {
  return ([...Array(100).keys()].reverse().map(year => {
    let item = year + 1920;
    return html`<option value=${item}>${item}</option>`;
  }));
};

const view = (state, actions) => {
  return html`
  <div class="view">
    <form @submit=${(e) => { e.preventDefault(); actions.generate(state.tester); }}>
      <div class="input-control">
        <label for="firstName">First Name</label>
        <input id="firstName" type="text" .value=${state.tester.firstName} @change=${(e)=>
        actions.changeFirstName(e.target.value)} />
      </div>
      <div class="input-control">
        <label for="lastName">Last Name</label>
        <input id="lastName" type="text" .value=${state.tester.lastName} @change=${(e)=>
        actions.changeLastName(e.target.value)} />
      </div>
      <div class="input-control">
        <label for="email">Email</label>
        <input id="email" type="email" .value=${state.tester.email} @change=${(e)=>
          actions.changeEmail(e.target.value)} />
      </div>
      <div class="input-control">
        <label for="birthDate">Date of Birth</label>
        <select @change=${(e) => actions.changeDobMonth(e.target.value)}>
          <option>Month</option>
          ${cache(months())}
        </select>
        <select @change=${(e) => actions.changeDobDay(e.target.value)}>
          <option>Day</option>
          ${cache(days())}
        </select>
        <select @change=${(e) => actions.changeDobYear(e.target.value)}>
          <option>Year</option>
          ${cache(years())}
        </select>
      </div>
      <div class="text-time-selection">
        <div>
          <input type="radio" name="text-time-selection" id="testNow" value="testNow" @change=${() => actions.takeTestNow(true)} />
          <label for="testNow">I'm ready to take my test now</label>
        </div>
        <div>
          <input type="radio" name="text-time-selection" id="testLater" value="testLater" @change=${() => actions.takeTestNow(false)} />
          <label for="testLater">Let's schedule my test for later</label>
        </div>
      </div>
      <div>
        <button type="submit">Submit</button>
      </div>
    </form>
  </div>
  `;
};

const testConfiguration = { initialState, actions, view };

export default testConfiguration;
