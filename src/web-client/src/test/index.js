import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

import { generateTestLink } from '../core/api';

const initialState = () => {
  return {
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
    changeFirstName: value => {
      update({ tester: PS({ firstName: value }) });
    },
    changeLastName: value => {
      update({ tester: PS({ lastName: value }) });
    },
    changeEmail: value => {
      update({ tester: PS({ email: value }) });
    },
    changeDobMonth: value => {
      update({ tester: PS({ dobMonth: value }) });
    },
    changeDobDay: value => {
      update({ tester: PS({ dobDay: value }) });
    },   
    changeDobYear: value => {
      update({ tester: PS({ dobYear: value }) });
    },
    generate: tester => {
      generateTestLink(tester);
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
          ${months()}
        </select>
        <select @change=${(e) => actions.changeDobDay(e.target.value)}>
          <option>Day</option>
          ${days()}
        </select>
        <select @change=${(e) => actions.changeDobYear(e.target.value)}>
          <option>Year</option>
          ${years()}
        </select>
      </div>
      <div class="text-time-selection">
        <div>
          <input type="radio" name="text-time-selection" id="testNow" value="I'm ready to take my test now" />
          <label for="testNow">I'm ready to take my test now</label>
        </div>
        <div>
          <input type="radio" name="text-time-selection" id="testLater" value="Let's schedule my test for later" />
          <label for="testLater">Let's schedule my test for later</label>
        </div>
      </div>
      <div>
        <button type="submit">Submit</button>
      </div>
    </form>
  `;
};

const test = { initialState, actions, view };

export default test;
