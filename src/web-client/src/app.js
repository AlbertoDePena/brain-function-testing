
import { PLATFORM } from 'aurelia-framework';

import { AuthorizeStep } from './core/authorize-step'; 

export class AppViewModel {
 
  configureRouter(config, router) {    
    config.title = 'Brain Function Testing';
    config.addAuthorizeStep(AuthorizeStep);
    config.map([
      { route: ['', 'main'], name: 'main', moduleId: PLATFORM.moduleName('pages/main/index') },
      { route: 'confirmation', name: 'confirmation', moduleId: PLATFORM.moduleName('pages/confirmation/index') },
      { route: 'status', name: 'status', moduleId: PLATFORM.moduleName('pages/status/index') },
      { route: 'error', name: 'error', moduleId: PLATFORM.moduleName('pages/error/index') }
    ]);
    config.fallbackRoute('error');

    this.router = router;
  }  
}
