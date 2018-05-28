import os
import time
import tensorflow as tf
from keras.models import load_model
from inference import vectorize, predict
from flask import Flask, jsonify, request, abort
from applicationinsights import TelemetryClient
from applicationinsights.requests import WSGIApplication
from applicationinsights.exceptions import enable

app = Flask(__name__)
# global vars for easy reusability
global model, graph
# initialize these variables
model, graph = load_model('model.h5'), tf.get_default_graph()

@app.route('/', methods=['GET'])
def get_version():
    return 'Version 1'

@app.route('/api/v1.0/predict', methods=['POST'])
def vision():
    if not request.json or not 'url' in request.json:
        abort(400)

    # get url
    url = request.json['url']
    t0 = time.time()
    # vectorize image
    x = vectorize(url)
    # predict
    with graph.as_default():
        r = predict(x, model, ['cat', 'dog'])
    t1 = time.time()
    r['elapsed'] = t1 - t0

    return jsonify(r), 201

if __name__ == '__main__':
    #app.wsgi_app = ProfilerMiddleware(app.wsgi_app, restrictions=[30])
    #app.run(host='0.0.0.0', port=8080, debug = True)
    # clear when containerizing!!
    app.run(host='0.0.0.0', port=8080)
    #app.run(host='127.0.0.1', port=5000)
