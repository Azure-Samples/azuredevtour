module.exports = function (context, req, messages) {
    context.res.body = messages;
    context.done();
};