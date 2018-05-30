#!/anaconda/envs/py35/bin/python
import os
import time
import datetime
import argparse
import tensorflow as tf
from keras import optimizers
from keras import applications
from keras.utils import multi_gpu_model
from keras.preprocessing.image import ImageDataGenerator
from keras.models import Sequential, Model 
from keras.layers import Dropout, Dense, GlobalAveragePooling2D, Flatten
from keras import backend as k 
from keras.callbacks import ModelCheckpoint, LearningRateScheduler, TensorBoard, EarlyStopping

def info(msg, char = "#", width = 78):
    print("")
    print(char * width)
    print(char + "   %0*s" % ((-1*width)+5, msg) + char)
    print(char * width)

def check(directory):
    if not os.path.exists(directory):
        os.makedirs(directory)

def train(train_data_dir, test_data_dir, model_dir, log_dir, batch_size, epochs, learning_rate, gpus):
    check(model_dir)
    check(log_dir)

    # start the clock
    t0 = time.time()
    img_width, img_height = 299, 299

    # retrieve VGG16 for use in transfer learning
    info('Loading VGG16 for Transfer Learning')
    
    model = applications.ResNet50(weights = "imagenet", include_top=False, input_shape = (img_width, img_height, 3))

    info('Adding Custom Layers to model')
    # add custom layers
    x = model.output
    x = Flatten()(x)
    prediction = Dense(4, activation="softmax", name="prediction")(x)

    # generate new model
    adam = optimizers.Adam(lr=learning_rate)
    if gpus > 1:
        m = multi_gpu_model(Model(inputs = model.input, outputs = prediction), gpus=gpus)
    else:
        m = Model(inputs = model.input, outputs = prediction)

    
    m.compile(loss = 'categorical_crossentropy', optimizer=adam, metrics=['accuracy'])
    print('Done!')
    m.summary()

    info('Loading Train and Test Sets')
    print('Train Data:', train_data_dir)
    train = ImageDataGenerator(rescale = 1./255)

    train_generator = train.flow_from_directory(
        train_data_dir,
        target_size=(img_width, img_height),
        batch_size=batch_size,
        class_mode='categorical')

    print('Test Data:', test_data_dir)
    test  = ImageDataGenerator(rescale = 1./255)

    test_generator = test.flow_from_directory(
        test_data_dir,
        target_size=(img_width, img_height),
        batch_size=batch_size,
        class_mode='categorical')

    
    info('Training Parameters')
    print('Train Categories:', train_generator.num_classes, ', Train Count:', train_generator.samples)
    print('Test Categories:', test_generator.num_classes, ', Test Count:', test_generator.samples)
    print('Using a learning rate of', learning_rate)
    print('Epochs:', epochs)
    print('Batch Size:', batch_size)

    unique = datetime.datetime.now().strftime('%m.%d_%H.%M')
    logs = os.path.join(log_dir, unique)
    models = os.path.join(model_dir, unique)
    check(logs)
    check(models)
    
    print('Log Directory:', logs)
    print('Model Directory:', models)

    checkpoint = ModelCheckpoint(os.path.join(models, 'checkpoint.{epoch:02d}-{val_acc:.2f}.h5'), monitor='val_acc', verbose=1, save_best_only=True, save_weights_only=False, mode='auto', period=1)
    early = EarlyStopping(monitor='val_acc', min_delta=0, patience=10, verbose=1, mode='auto')
    tensorboard = TensorBoard(log_dir=logs)
    callbacks = [checkpoint, early, tensorboard]

    info('Training')

    m.fit_generator(
        train_generator,
        steps_per_epoch=train_generator.samples // batch_size,
        epochs=epochs,
        validation_data=test_generator,
        validation_steps=test_generator.samples // batch_size,
        callbacks = callbacks)

    model_file = os.path.join(models, 'model.h5')
    print('Saving final model to', model_file, '...')
    m.save(model_file)
    t1 = time.time()
    print('Done', t1-t0)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='CNN Training for Image Recognition.')
    parser.add_argument('-d', '--data', help='directory to training and test data', required=True)
    parser.add_argument('-e', '--epochs', help='number of epochs', default=10, type=int)
    parser.add_argument('-b', '--batch', help='batch size', default=24, type=int)
    parser.add_argument('-l', '--lr', help='learning rate', default=0.001, type=float)
    parser.add_argument('-g', '--gpus', help='gpus', default=1, type=int)
    parser.add_argument('-o', '--output', help='output directory', default='.')
    args = parser.parse_args()
    
    # check arguments
    train_data_dir = os.path.join(args.data, 'train')
    test_data_dir = os.path.join(args.data, 'test')

    if os.path.exists(train_data_dir) and os.path.exists(test_data_dir):
        epochs = args.epochs
        batch_size = args.batch
        model_dir = os.path.join(args.output, 'models')
        log_dir = os.path.join(args.output, 'logs')
        learning_rate = args.lr

        train(train_data_dir, test_data_dir, model_dir, log_dir, batch_size, epochs, learning_rate, args.gpus)
    else:
        print('Data directories are invalid')
