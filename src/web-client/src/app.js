
import { PLATFORM } from 'aurelia-framework';

import { AuthorizeStep } from './core/authorize-step'; 

export class AppViewModel {
 
  configureRouter(config, router) {    
    config.title = 'Brain Function Testing';
    config.addAuthorizeStep(AuthorizeStep);
    config.map([
      { route: ['', 'main'], name: 'main', moduleId: PLATFORM.moduleName('pages/main/index'), title: 'Main' },
      { route: 'confirmation', name: 'confirmation', moduleId: PLATFORM.moduleName('pages/confirmation/index'), title: 'Confirmation' },
      { route: 'status', name: 'status', moduleId: PLATFORM.moduleName('pages/status/index'), title: 'Status' },
      { route: 'documentation', name: 'documentation', moduleId: PLATFORM.moduleName('pages/documentation/index'), title: 'Documentation' }
    ]);
    config.fallbackRoute('documentation');

    this.router = router;
  }  
}
