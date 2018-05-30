# Rotation Model
This code generates a model that detects improper image rotation. It uses images from the
COCO dataset and arbitrarily rotates them using the `cocoprep.py` and dumps into a data
folder.

## File Descriptions
1. `cocoprep.py` - Prepares COCO images with arbitrary rotations for training
2. `train.py` - Training code that generates the model (using Keras)
3. `inference.py` - Code that loads model and does inferencing on new images
4. `app.py` - Flask app that exposes an API for inferencing
5. `dockerfile` - Description for generating docker image of Flask app
