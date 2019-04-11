import test from '../test';
import confirm from '../test/confirm';

const initialState = () => {
  return { route: '' };
};

const actions = update => {
  return {
    updateRoute: route => update({ route })
  };
};

const view = (state, actions) => {
  return state.route === '#/confirm' ?
    confirm.view() :
    test.view(state, actions);
};

const navigation = { initialState, actions, view };

export default navigation;
