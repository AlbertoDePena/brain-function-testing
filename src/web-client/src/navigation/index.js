import { getUrlParameter, navigateTo } from '../core/router';

import conditions from '../conditions';
import temperature from '../temperature';

const initialState = () => {
  return { currentRoute: 'conditions' };
};

const actions = update => {
  return {
    updateRoute: routeParams => update({ currentRoute: routeParams.route })
  };
};

const view = (state, actions) => {
  if (state.currentRoute.endsWith('temperature')) {
    return temperature.view(state, actions, getUrlParameter('type'));
  }

  return conditions.view(state, actions);
};

const navigation = { initialState, actions, view, navigateTo };

export default navigation;
