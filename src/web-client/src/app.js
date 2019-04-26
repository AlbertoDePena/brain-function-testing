
import { PLATFORM } from 'aurelia-framework';

import { AuthorizeStep } from './core/authorize-step'; 

export class AppViewModel {
 
  configureRouter(config, router) {    
    config.title = 'Brain Function Testing';
    config.addAuthorizeStep(AuthorizeStep);
    config.map([
      { route: ['', 'login'], name: 'login', moduleId: PLATFORM.moduleName('pages/login/index') },
      { route: 'test-config', name: 'test-config', moduleId: PLATFORM.moduleName('pages/test-config/index') },
      { route: 'confirmation', name: 'confirmation', moduleId: PLATFORM.moduleName('pages/confirmation/index') }
    ]);

    this.router = router;
  }  
}
