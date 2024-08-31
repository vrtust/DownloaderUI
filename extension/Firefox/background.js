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
    const downloadExtensions = [
        // Compressed and archived files
        '.zip', '.rar', '.7z', '.tar', '.gz',

        // Document and text files
        '.pdf', '.doc', '.docx', '.txt', '.rtf', '.odt',

        // Executable files
        '.exe', '.msi', '.app', '.dmg',

        // Image files
        '.jpg', '.jpeg', '.png', '.gif', '.bmp', '.tiff', '.svg',

        // Audio files
        '.mp3', '.wav', '.aac', '.flac', '.ogg',

        // Video files
        '.mp4', '.avi', '.mov', '.wmv', '.flv', '.mkv',

        // Web files
        '.html', '.htm', '.css', '.js',

        // Spreadsheet and presentation files
        '.xls', '.xlsx', '.ppt', '.pptx', '.csv',

        // Programming and script files
        '.py', '.java', '.cpp', '.c', '.cs', '.php', '.rb',

        // System and configuration files
        '.ini', '.cfg', '.dll', '.sys',

        // Database files
        '.db', '.sql', '.sqlite',

        // Font files
        '.ttf', '.otf', '.woff',

        // Virtual machine and disk image files
        '.iso', '.vhd', '.vmdk',

        // E-book files
        '.epub', '.mobi', '.azw',

        // 3D and CAD files
        '.obj', '.stl', '.dxf',

        // Other
        '.torrent', '.bak', '.tmp'
    ];

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