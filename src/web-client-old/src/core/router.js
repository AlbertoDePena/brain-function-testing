/**
 * @typedef {Object} RouteParams
 * @property {String} RouteParams.route
 * @property {String} RouteParams.query
 */

/**
 * @callback locationChangedCallback
 * @param {RouteParams} routeParams
 * @param {Event|null} event
 * @returns {void}
 */

let onLocationChangedCallback;

/**
 * @param {locationChangedCallback} locationChangedCallback
 */
function configureRouter(locationChangedCallback) {
  onLocationChangedCallback = locationChangedCallback;
  document.body.addEventListener('click', e => {
    if (
      e.defaultPrevented ||
      e.button !== 0 ||
      e.metaKey ||
      e.ctrlKey ||
      e.shiftKey
    )
      return;

    const anchor = e.composedPath().filter(n => n.tagName === 'A')[0];

    if (
      !anchor ||
      anchor.target ||
      anchor.hasAttribute('download') ||
      anchor.getAttribute('rel') === 'external'
    )
      return;

    const href = anchor.href;
    if (!href || href.indexOf('mailto:') !== -1) return;

    const location = window.location;
    const origin = location.origin || location.protocol + '//' + location.host;
    if (href.indexOf(origin) !== 0) return;

    e.preventDefault();
    if (href !== location.href) {
      window.history.pushState({}, '', href);
      onLocationChangedCallback(window.location.hash, e);
    }
  });

  window.addEventListener('popstate', e =>
    onLocationChangedCallback(window.location.hash, e)
  );
  onLocationChangedCallback(window.location.hash, null);
}

/**
 * @param {String} route
 */
function navigateTo(route) {
  const hash = `#/${route}`;
  if (window.location.hash === hash) return;
  if (!onLocationChangedCallback) return;
  window.history.pushState({}, '', hash);
  onLocationChangedCallback(window.location.hash, null);
}

/**
 * @param {String} name
 */
function getUrlParameter(name) {
  // eslint-disable-next-line no-useless-escape
  name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
  const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
  const results = regex.exec(window.location.hash);
  return results === null
    ? ''
    : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

export { configureRouter, navigateTo, getUrlParameter };
