let interceptedUrls = new Set();


browser.webRequest.onBeforeRequest.addListener(
  (details) => {
    if (isLikelyDownload(details.url) && !details.url.startsWith('http://localhost')) {
      interceptedUrls.add(details.url);
      sendUrl(details.url);
      return { cancel: true };
    }
    return { cancel: false };
  },
  { urls: ["<all_urls>"] },
  ["blocking"]
);

browser.downloads.onCreated.addListener((downloadItem) => {
  if (interceptedUrls.has(downloadItem.url)) {
    browser.downloads.erase({ id: downloadItem.id });
    interceptedUrls.delete(downloadItem.url);
  } else {
    browser.downloads.cancel(downloadItem.id);
    browser.downloads.erase({ id: downloadItem.id });
    sendUrl(downloadItem.url);
  }
});

function isLikelyDownload(url) {
  const downloadExtensions = ['.zip', '.pdf', '.exe', '.dmg', '.iso', '.mp3', '.mp4'];
  return downloadExtensions.some(ext => url.toLowerCase().endsWith(ext));
}


function sendUrl(url) {
  var xhr = new XMLHttpRequest();
  xhr.open('GET', 'http://localhost:5000/?url=' + encodeURIComponent(url), true);
  xhr.onload = function () {
      if (xhr.status === 200) {
          alert('URL has been sent');
      } else {
          alert('something went wrong');
      }
  };
  xhr.send();
}