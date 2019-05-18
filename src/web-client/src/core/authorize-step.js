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

    function redirectToDocumentationPage(message) {
      notifyError(message);
      return next.cancel(new Redirect('documentation'));
    }

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
        return redirectToDocumentationPage('Failed to contact BFT server. Please contact system administrator.');
      }

      if (error.statusCode !== 404) {
        return redirectToDocumentationPage(error.response);
      }

      if (!firstName) {
        return redirectToDocumentationPage('Please provide first name in query parameter');
      }

      if (!lastName) {
        return redirectToDocumentationPage('Please provide last name in query parameter');
      }

      setTesterState({ email, firstName, lastName });

      return next();
    }

    switch (routeName) {
      case 'main':
        setTestConfigState(testConfig);

        if (!email) {
          return redirectToDocumentationPage('email is required in query param');
        }

        if (!email.match(/.+@.+/)) {
          return redirectToDocumentationPage('Email is invalid');
        }

        return this.api
          .getTester(email)
          .then(setState)
          .catch(handleError);

      case 'confirmation':
        if (!tester.id) {
          return next.cancel(new Redirect('main'));
        }
        if (hasTestResults) {
          return next.cancel(new Redirect('status'));
        }
        return next();

      case 'status':
        if (!tester.id) {
          return next.cancel(new Redirect('main'));
        }
        if (!hasTestResults) {
          return next.cancel(new Redirect('confirmation'));
        }
        return next();

      case 'documentation':
        return next();
    }
  }
}
