const getAuthDetails = require('../lib/get-auth-details');

module.exports = function (context, req) {
  var zumoToken = req.headers['x-zumo-auth'];
  getAuthDetails(zumoToken, context).then(authDetails => {
    const username = authDetails && authDetails[0].user_id;
  
    if (username) {
      req.body.sender = username;
      req.body.avatarUrl = `https://avatars.io/twitter/${username}/small`
    } else {
      req.body.avatarUrl = 'https://avatars.io/static/default_48.jpg';
    }
  
    context.bindings.signalRMessages = [{
      "target": "newMessage",
      "arguments": [req.body]
    }];
    context.bindings.cosmosDBMessage = req.body;
    context.done();
  }).catch(err => {
    context.done(err);
  });
};