/**
 * @callback offlineChangedCallback
 * @param {Boolean} isOffline
 * @returns {void}
 */

/**
 * @param {offlineChangedCallback} offlineChangedCallback
 */
function installOfflineWatcher(offlineChangedCallback) {
  window.addEventListener('online', () => offlineChangedCallback(false));
  window.addEventListener('offline', () => offlineChangedCallback(true));

  offlineChangedCallback(navigator.onLine === false);
}

export { installOfflineWatcher };
