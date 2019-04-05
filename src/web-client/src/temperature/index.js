import { html } from 'lit-html';
import { PS, S } from 'patchinko/explicit';

const convert = (value, to) => {
  return Math.round(to === 'C' ? ((value - 32) / 9) * 5 : (value * 9) / 5 + 32);
};

const initialState = label => {
  return {
    label,
    value: 22,
    units: 'C'
  };
};

const actions = update => {
  return {
    increment: (id, amount) => {
      update({ [id]: PS({ value: S(x => x + amount) }) });
    },
    changeUnits: id => {
      update({
        [id]: S(state => {
          var value = state.value;
          var newUnits = state.units === 'C' ? 'F' : 'C';
          var newValue = convert(value, newUnits);
          state.value = newValue;
          state.units = newUnits;
          return state;
        })
      });
    }
  };
};

const view = (state, actions, id) => {
  return html`
    <div>
      <h6>
        ${state[id].label} Temperature: ${state[id].value} &deg;
        ${state[id].units}
      </h6>
      <div>
        <button @click=${() => actions.increment(id, 1)}>
          Increment
        </button>
        <button @click=${() => actions.increment(id, -1)}>
          Decrement
        </button>
        <button @click=${() => actions.changeUnits(id)}>
          Change Units
        </button>
      </div>
    </div>
  `;
};

const temperature = { initialState, actions, view };

export default temperature;
