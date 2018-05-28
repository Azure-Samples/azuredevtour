#!/anaconda/envs/py35/bin/python
import os
import math
import shutil
import argparse
import numpy as np
from PIL import Image
import matplotlib.pyplot as plt

def largest_rotated_rect(w, h, angle):
    """
    Given a rectangle of size wxh that has been rotated by 'angle' (in
    radians), computes the width and height of the largest possible
    axis-aligned rectangle within the rotated rectangle.
    Original JS code by 'Andri' and Magnus Hoff from Stack Overflow
    Converted to Python by Aaron Snoswell
    Source: http://stackoverflow.com/questions/16702966/rotate-image-and-crop-out-black-borders
    """

    quadrant = int(math.floor(angle / (math.pi / 2))) & 3
    sign_alpha = angle if ((quadrant & 1) == 0) else math.pi - angle
    alpha = (sign_alpha % math.pi + math.pi) % math.pi

    bb_w = w * math.cos(alpha) + h * math.sin(alpha)
    bb_h = w * math.sin(alpha) + h * math.cos(alpha)

    gamma = math.atan2(bb_w, bb_w) if (w < h) else math.atan2(bb_w, bb_w)

    delta = math.pi - alpha - gamma

    length = h if (w < h) else w

    d = length * math.cos(alpha)
    a = d * math.sin(alpha) / math.sin(delta)

    y = a * math.cos(gamma)
    x = y * math.tan(gamma)

    return (
        math.ceil(bb_w - 2 * x),
        math.ceil(bb_h - 2 * y)
    )

def rotate(image, angle, crop=True):
    img = image.convert('RGBA').rotate(angle, expand=True)
    if crop: # use largest useful space
        largest = largest_rotated_rect(image.size[0], image.size[1], angle)
        left = (img.size[0]/2) - (largest[0]/2)
        upper = (img.size[1]/2) - (largest[1]/2)
        right = left + largest[0]
        lower = upper + largest[1]
        img = img.crop((left, upper, right, lower))
    else: # white background
        fff = Image.new('RGBA', img.size, (255,)*4)
        img = Image.composite(img, fff, img)
    return img.convert('RGB')

def info(msg, char = "#", width = 75):
    print("")
    print(char * width)
    print(char + "   %0*s" % ((-1*width)+5, msg) + char)
    print(char * width)

def check(directory, clear=False):
    if not os.path.exists(directory):
        os.makedirs(directory)
    elif clear:
        shutil.rmtree(directory)
        os.makedirs(directory)
    
    return directory

def process(files, basedir, directory, aincr):
    info('Processing {0} ({1})'.format(directory, len(files)))
    target = os.path.join(basedir, directory)
    check(target, clear=True)

    for idx, f in enumerate(files):
        angle = (idx*aincr) % 360 
        file = os.path.join(basedir, f)
        toDir = os.path.join(target, str(angle))
        check(toDir)
        tofile = os.path.join(toDir, f)
        print('Rotating', file, 'saving to', tofile)
        try:
            rot = rotate(Image.open(file), angle)
            rot.save(tofile)
        except SystemError as e:
            print(type(e), e.args, e)

def crawl(directory, holdout, aincr):
    info('Using {0}'.format(directory))

    files = [name for name in os.listdir(directory) 
             if os.path.isfile(os.path.join(directory, name)) and 
             '.jpg' in name]

    count = len(files)
    valid = int(count * holdout * .5)
    test = valid
    train = count - valid - test

    print('{:<12} {:>10}'.format('Total:', count))
    print('-----------------------')
    print('{:<12} {:>10}'.format('Train:', train))
    print('{:<12} {:>10}'.format('Validation:', valid))
    print('{:<12} {:>10}'.format('Test:', test))

    process(files[:train], directory,'train', aincr)
    process(files[train:train+valid], directory,'validation', aincr)
    process(files[train+valid:], directory,'test',aincr)

    print('Done!')


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Create arbitrarily rotated images from COCO dataset')
    parser.add_argument('-d', '--directory', help='source directory', required=True)
    parser.add_argument('-t', '--test', help='test/validation holdout', default=0.2, type=float)
    parser.add_argument('-a', '--angle', help='angle increments', default=10, type=int)
    args = parser.parse_args()

    crawl(args.directory, args.test, args.angle)