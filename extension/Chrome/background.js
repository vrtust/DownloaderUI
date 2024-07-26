let interceptedUrls = new Set();

chrome.webRequest.onBeforeRequest.addListener(
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

chrome.downloads.onCreated.addListener((downloadItem) => {
  if (interceptedUrls.has(downloadItem.url)) {
    chrome.downloads.erase({ id: downloadItem.id });
    interceptedUrls.delete(downloadItem.url);
  } else {
    chrome.downloads.cancel(downloadItem.id);
    chrome.downloads.erase({ id: downloadItem.id });
    sendUrl(downloadItem.url);
  }
});

function isLikelyDownload(url) {
  const downloadExtensions = ['.zip', '.pdf', '.exe', '.dmg', '.iso', '.mp3', '.mp4'];
  return downloadExtensions.some(ext => url.toLowerCase().endsWith(ext));
}

function sendUrl(url) {
  fetch('http://localhost:5000/?url=' + encodeURIComponent(url), {
    method: 'GET'
  })
  .then(response => {
    if (response.ok) {
      console.log('URL has been sent');
    } else {
      console.log('Something went wrong');
    }
  })
  .catch(error => {
    console.log('Error:', error);
    console.log('Something went wrong');
  });
}