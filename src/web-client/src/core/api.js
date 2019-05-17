import { HttpClient } from 'aurelia-http-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Api {
  
  constructor(httpClient) {
    this.httpClient = httpClient.configure(opts => {
      opts.withBaseUrl('http://localhost:7071/api/');
      opts.withInterceptor({
        request(message) {
          return message;
        },
        requestError(error) {
          throw error;
        },
        response(message) {
          return message;
        },
        responseError(error) {
          throw error;
        }
      });
    });
  }

  /**
    * 
    * @param {String} email
    * @param {String} testConfig
    * @returns {TestLinkResult} test link result
    */
  getTestLink(email, testConfig) {
    return this.httpClient.get(`get-test-link-http-trigger?email=${email}&config=${testConfig}`)
      .then(result => JSON.parse(result.response));
  }

  /**
   * 
   * @param {String} email 
   * @returns {Tester} tester
   */
  getTester(email) {
    return this.httpClient.get(`get-tester-http-trigger?email=${email}`)
      .then(result => JSON.parse(result.response))
      .then(tester => {
        const [dobMonth, dobDay, dobYear] = tester.dob.split('/');
        return { ...tester, dobMonth, dobDay, dobYear };
      });
  }

  /**
   * 
   * @param {Tester} tester 
   * @returns {String} tester ID
   */
  saveTester(tester) {
    const { dobMonth, dobDay, dobYear } = tester;
    tester.dob = `${dobMonth}/${dobDay}/${dobYear}`;
    return this.httpClient.post('save-tester-http-trigger', tester).then(result => result.response);
  }
}
