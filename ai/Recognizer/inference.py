import requests
from PIL import Image
from io import BytesIO
from keras.models import load_model
from numpy import array, expand_dims

def vectorize(url, size=(224, 224)):
    # load image from url
    response = requests.get(url)
    f = BytesIO(response.content)
    img = Image.open(f).resize(size, resample=Image.BICUBIC)

    # expected shape for model
    expanded = expand_dims(array(img) / 255., axis=0)
    return expanded

def predict(vector, model, labels=None):
    response = {}
    p = model.predict(vector)[0].tolist()
    for i, v in enumerate(p):
        if labels == None:
            response[i] = v
        else:
            response[labels[i]] = v
    return response

if __name__ == "__main__":
    labels = ['cat', 'dog']
    model = load_model('model.h5')

    d = vectorize('https://upload.wikimedia.org/wikipedia/commons/6/60/YellowLabradorLooking.jpg')
    dog = predict(d, model, labels)
    print(dog)

    c = vectorize('https://upload.wikimedia.org/wikipedia/commons/3/3a/Cat03.jpg')
    cat = predict(c, model, labels)
    print(cat)
