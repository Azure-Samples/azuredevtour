# Recognizer Model
This code generates a model that detects dogs and cats. The data is pulled from Bing
using the `dataprep.ipynb` notebook.

## File Descriptions
1. `train.py` - Training code that generates the model (using Keras)
2. `inference.py` - Code that loads model and does inferencing on new images
3. `app.py` - Flask app that exposes an API for inferencing
4. `dockerfile` - Description for generating docker image of Flask app