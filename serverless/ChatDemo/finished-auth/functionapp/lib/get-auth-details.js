const https = require('https');

module.exports = function(token, context) {
  return new Promise((resolve, reject) => {
    if (!token) {
      resolve(null);
      return;
    }
    
    const options = {
      host: process.env.WEBSITE_HOSTNAME,
      path: '/.auth/me',
      headers: {
          accept: 'application/json',
          'x-zumo-auth': token
      }
    };
    https.get(options, (resp) => {
      let data = '';
      
      resp.on('data', (chunk) => {
        data += chunk;
      });
     
      resp.on('end', () => {
        resolve(JSON.parse(data));
      });
     
    }).on("error", (err) => {
      reject(err.message);
    });
  });
}