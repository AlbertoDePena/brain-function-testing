import { PLATFORM } from 'aurelia-framework';

export function configure(config) {
  config.globalResources([
    PLATFORM.moduleName('./filterBy'),
    PLATFORM.moduleName('./sortBy'),
    PLATFORM.moduleName('./groupBy')
  ]);
}
