import about from '../about';
import testLinkGenerator from '../test-link-generator';

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
    testLinkGenerator.view(state, actions);
};

const navigation = { initialState, actions, view };

export default navigation;
