window.addEventListener('load',
  function() {
    function getBrowserState(event) {
      updateBrowserState(state);
      console.log("Event: " + event.type + "; Status: " + (navigator.onLine ? "online" : "offline"));
    }

    window.addEventListener('online', getBrowserState);
    window.addEventListener('offline', getBrowserState);
  });

function updateBrowserState(hiddenFieldId) {
  var output = document.getElementsByClassName("g-recaptcha")[0];
  var hiddenField = document.getElementById(hiddenFieldId);

  if (!navigator.onLine) {
    hiddenField.value = "offline";
    if (output != null) output.style.display = "none";
  } else {
    hiddenField.value = "online";
    if (output != null) output.style.display = "block";
  }
}