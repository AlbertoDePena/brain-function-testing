import about from '../about';
import testLinkGenerator from '../test-link-generator';

const initialState = () => {
  return { currentRoute: 'about' };
};

const actions = update => {
  return {
    updateRoute: routeParams => update({ currentRoute: routeParams.route })
  };
};

const view = (state, actions) => {
  if (state.currentRoute.endsWith('about')) {
    return about.view();
  }

  return testLinkGenerator.view(state, actions);
};

const navigation = { initialState, actions, view };

export default navigation;
