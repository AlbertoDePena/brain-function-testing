import { HttpClient } from 'aurelia-http-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Api {

  constructor(httpClient) {
    this.httpClient = httpClient.configure(opts => {
      // opts.withBaseUrl('http://aurelia.io');
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
   * build payload
   * @param {Tester} tester 
   * @returns {TestLinkRequest} test link request
   */
  buildPayload(tester) {
    let payload = {
      request: 'rtl',
      account: '',
      username: '',
      password: '',
      subject_id: '',
      dob_day: tester.dobDay,
      dob_month: tester.dobMonth,
      dob_year: tester.dobYear,
      test_config: '9',
      test_lang: 'english_us'
    };

    return payload;
  }

  /**
    * 
    * @param {Tester} tester
    */
  getTestLink(tester) {
    const payload = this.buildPayload(tester);

    return this.httpClient.post('https://sync.cnsvs.com/sync.php', {
      method: 'POST',
      mode: 'no-cors',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: JSON.stringify(payload)
    }).then(res => {
      console.log('Request complete! response:', res);
    }).catch(error => console.error(error));
  }

  /**
   * 
   * @param {String} email 
   * @returns {Tester} tester
   */
  getTester(email) {
    return this.httpClient.get(`http://localhost:7071/api/get-tester-http-trigger?email=${email}`)
      .then(result => JSON.parse(result.response))
      .then(tester => {
        const [ dobMonth, dobDay, dobYear ] = tester.dob.split('/');
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
    return this.httpClient.post('http://localhost:7071/api/save-tester-http-trigger', tester).then(result => result.response);
  }
}
