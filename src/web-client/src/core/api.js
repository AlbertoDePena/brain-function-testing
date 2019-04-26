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
      dob_year: tester.dobYear,
      dob_month: tester.dobMonth,
      dob_day: tester.dobDay,
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
}
