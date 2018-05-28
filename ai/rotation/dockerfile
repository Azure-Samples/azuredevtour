FROM python:3-slim

RUN pip install --upgrade pip
RUN pip --no-cache-dir install \
        tensorflow \
        Flask \
        h5py==2.8.0rc1 \
        keras \
        pillow \
        applicationinsights \
        requests \
        python-dateutil \
        flask-cors


RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY model.h5 /usr/src/app
COPY inference.py /usr/src/app
COPY app.py /usr/src/app


EXPOSE 8080

CMD ["python", "app.py"]