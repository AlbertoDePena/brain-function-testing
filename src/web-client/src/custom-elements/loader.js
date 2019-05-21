import { inlineView, DOM, inject, customElement } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';

@inject(DOM.Element, EventAggregator)
@inlineView('<template><div class="loader hide-loader"></div></template>')
@customElement('bft-loader')
export class LoaderCustomElement {

  constructor(element, eventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
    this.subscription = undefined;
  }

  attached() {
    this.subscription = this.eventAggregator.subscribe('loading-event', isLoading => {
      const element = this.element.querySelector('.loader');
      if (!element) return;

      if (isLoading) {
        element.classList.add('show-loader');
        element.classList.remove('hide-loader');
      } else {
        element.classList.add('hide-loader');
        element.classList.remove('show-loader');
      }
    });
  }

  detached() {
    if (this.subscription) this.subscription.dispose();
  }
}
