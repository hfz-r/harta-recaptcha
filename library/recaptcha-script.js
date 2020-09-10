/**
 * DOM helper to update UI
 * 
 * @param {boolean} isOnline - connection state
 */
function updateBrowserState(isOnline) {
  var output = document.getElementsByClassName("g-recaptcha")[0];
  var hiddenField = document.getElementById(state);

  if (isOnline) {
    hiddenField.value = "online";
    if (output != null) output.style.display = "block";
  } else {
    hiddenField.value = "offline";
    if (output != null) output.style.display = "none";
  }
}

/**
 * Custom "offline/online" event
 * 
 * @param {string} name - event name
 * @param {any} data - event data
 */
var fireEvent = function(name, data) {
  var e = document.createEvent("Event");
  e.initEvent(name, true, true);
  e.data = data;
  window.dispatchEvent(e);
};

/**
 * XHR request to an online source.
 *
 * @param {object} options - xhr options
 * @param {requestCallback} callback - The callback that handles the response.
 *
 */
var fetch = function(options, callback) {
  var xhr = new XMLHttpRequest();

  var noResponseTimer = setTimeout(function() {
      xhr.abort();
      fireEvent("connectiontimeout", {});
      if (!!localStorage[options.url]) {
        callback(localStorage[options.url]);
        return;
      }
    },
    3000);

  xhr.onreadystatechange = function(e) {
    if (xhr.readyState !== 4) {
      return;
    }

    if (xhr.status == 200) {
      fireEvent("goodconnection", {});
      clearTimeout(noResponseTimer);

      localStorage[options.url] = xhr.responseText;
      callback(xhr.responseText);
    } else {
      fireEvent("connectionerror", {});
      if (!!localStorage[options.url]) {
        callback(localStorage[options.url]);
        return;
      }
    }
  };

  xhr.open(options.method, options.url);

  if (/^POST/i.test(options.method)) {
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
  }
  xhr.send(options.data);
};

//subscribe to browser api event
window.addEventListener('online',
  function(e) {
    fetch({
        method: 'GET',
        url: 'https://jsonplaceholder.typicode.com/todos/1'
      },
      function(e) {});
  });
window.addEventListener('offline', function(e) { updateBrowserState(false); });
//subscribe to custom created event
window.addEventListener("goodconnection", function(e) { updateBrowserState(true); });
window.addEventListener("connectionerror", function(e) { updateBrowserState(false); });