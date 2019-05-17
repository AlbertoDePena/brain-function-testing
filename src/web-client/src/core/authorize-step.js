import { Redirect } from 'aurelia-router';
import { inject } from 'aurelia-framework';
import { Api } from '../core/api';
import { getState, setTesterState, setTestConfigState } from '../core/state';
import { notifyError } from '../core/notifications';

@inject(Api)
export class AuthorizeStep {

  constructor(api) {
    this.api = api;
  }

  run(navigationInstruction, next) {
    const email = navigationInstruction.queryParams.email,
      firstName = navigationInstruction.queryParams.fn,
      lastName = navigationInstruction.queryParams.ln,
      testConfig = navigationInstruction.queryParams.config || '9',
      routeName = navigationInstruction.config.name,
      tester = getState().tester || {},
      hasTestResults = (tester.testResults || []).length;

    function setState(tester) {
      setTesterState(tester);

      const hasTestResults = (tester.testResults || []).length;

      if (hasTestResults) {
        return next.cancel(new Redirect('status'));
      }

      return next.cancel(new Redirect('confirmation'));
    }

    function handleError(error) {
      if (error.statusCode === 0) {
        notifyError(
          'Failed to contact Brain Function Testing server. Please contact system administrator.'
        );
        return next.cancel(new Redirect('error'));
      }

      if (error.statusCode !== 404) {
        notifyError(error.response);
        return next.cancel(new Redirect('error'));
      }

      if (!firstName) {
        notifyError('Please provide first name in query parameter');
        return next.cancel(new Redirect('error'));
      }

      if (!lastName) {
        notifyError('Please provide last name in query parameter');
        return next.cancel(new Redirect('error'));
      }

      setTesterState({ email, firstName, lastName });

      return next();
    }

    switch (routeName) {
      case 'main':
        setTestConfigState(testConfig);

        if (!email) {
          notifyError('email is required in query param');
          return next.cancel(new Redirect('error'));
        }

        if (!email.match(/.+@.+/)) {
          notifyError('Email is invalid');
          return next.cancel(new Redirect('error'));
        }

        return this.api
          .getTester(email)
          .then(setState)
          .catch(handleError);

      case 'confirmation':
        if (!tester.email) {
          return next.cancel(new Redirect('main'));
        }
        if (hasTestResults) {
          return next.cancel(new Redirect('status'));
        }
        return next();

      case 'status':
        if (!tester.email) {
          return next.cancel(new Redirect('main'));
        }
        if (!hasTestResults) {
          return next.cancel(new Redirect('confirmation'));
        }
        return next();

      case 'error':
        return next();
    }
  }
}
