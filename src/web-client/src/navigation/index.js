import about from '../about';
import test from '../test';

const initialState = () => {
  return { route: '' };
};

const actions = update => {
  return {
    updateRoute: routeParams => update({ route: routeParams.route })
  };
};

const view = (state, actions) => {
  return state.route.endsWith('about') ?
    about.view() :
    test.view(state, actions);
};

const navigation = { initialState, actions, view };

export default navigation;
