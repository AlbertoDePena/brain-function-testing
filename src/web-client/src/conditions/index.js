import { html } from 'lit-html';
import { PS } from 'patchinko/explicit';

const initialState = () => {
  return {
    conditions: {
      precipitations: false,
      sky: 'Sunny'
    }
  };
};

const actions = update => {
  return {
    togglePrecipitations: value => {
      update({ conditions: PS({ precipitations: value }) });
    },
    changeSky: value => {
      update({ conditions: PS({ sky: value }) });
    }
  };
};

const skyOption = ({ state, actions, value, label }) => {
  return html`
    <label>
      <input
        type="radio"
        id=${value}
        name="sky"
        value=${value}
        .checked=${state.conditions.sky === value}
        @change=${evt => actions.changeSky(evt.target.value)}
      />
      ${label}
    </label>
  `;
};

const view = (state, actions) => {
  return html`
    <div>
      <label>
        <input
          type="checkbox"
          .checked=${state.conditions.precipitations}
          @change=${evt => actions.togglePrecipitations(evt.target.checked)}
        />
        Precipitations
      </label>
      <div>
        ${skyOption({ state, actions, value: 'SUNNY', label: 'Sunny' })}
        ${skyOption({ state, actions, value: 'CLOUDY', label: 'Cloudy' })}
        ${skyOption({
          state,
          actions,
          value: 'MIX',
          label: 'Mix of sun/clouds'
        })}
      </div>
    </div>
  `;
};

const conditions = { initialState, actions, view };

export default conditions;
