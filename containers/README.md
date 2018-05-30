# Scaling with containers

> We previously deployed a Node web application (see [`../webapp`](../webapp/README.md)) to Azure App Service.
> 
> This web app consists of a **Frontend** serving a static single page application and a **Backend** connecting to Azure KeyVault, CosmosDB to serve an API.

In this tutorial we will cover:
- how to containerize the web application with Docker,
- how to publish the container image to Azure Container Registry
- how to quickly run the container in Azure, accessible on the internet,
- and finally, we will introduce the concept of independently scaling our Frontend and Backend microservices with the help of Kubernetes (AKS: Azure Kubernetes Service).


## Containerizing the web application

### Prerequisites

- [Docker CE](https://docs.docker.com/install/) installed

Before we can start building containers for our app we will need to copy the [`webapp`](../webapp) folder into every folder with a Dockerfile.

> Before copying the folder:

```
.
├── README.md
├── container-instances
│   └── Dockerfile
├── kubernetes
│   ├── backend
│   │   ├── Dockerfile
│   │   └── backend.yaml
│   └── frontend
│       ├── Dockerfile
│       ├── frontend.yaml
│       └── startup.sh
└── simple-container
    └── Dockerfile
```

> After copying the folder:

```
.
├── README.md
├── container-instances
│   ├── Dockerfile
│   └── webapp
├── kubernetes
│   ├── backend
│   │   ├── Dockerfile
│   │   ├── backend.yaml
│   │   └── webapp
│   └── frontend
│       ├── Dockerfile
│       ├── frontend.yaml
│       ├── startup.sh
│       └── webapp
└── simple-container
    ├── Dockerfile
    └── webapp
```

### Containerizing the Node Development Server

> For your convenience in following this tutorial include all necessary connection secrets in an `.env` file within the `webapp` directory.
> You should never publish containers with secrets built into them or run these in a shared environment.

To build a Docker container image called `phototour-dev`, navigate to the `simple-container` directory and run:

```bash
docker build -t phototour-dev .
```

Once the container has been built, you can run the container with the following command to make the web app available at http://localhost:8080/.

```bash
docker run -it -p8080:80 phototour-dev
```
This command maps the local port 8080 to port 80 within the container and displays all container logs.

### Building the Production Image

> For your convenience in following this tutorial include all necessary connection secrets in an `.env` file within the `webapp` directory.
> You should never publish containers with secrets built into them or run these in a shared environment.

The `container-instances` contains an optimized Dockerfile based on the `alpine` linux distribution. This represents a production build of our Node application.

To build this Docker container image and name it `phototour`, navigate to the `container-instances` directory and run:

```bash
docker build -t phototour .
```

Once the container has been built, you can run the container with the following command to make the web app available at http://localhost:8080/.

```bash
docker run -it -p8080:80 phototour
```
This command maps the local port 8080 to port 80 within the container and displays all container logs.

## Publishing the Docker image to Azure Container Registry

### Prerequisites

- [Docker CE](https://docs.docker.com/install/) installed
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) installed
- [Azure Container Registry](https://docs.microsoft.com/azure/container-registry/container-registry-get-started-azure-cli) created

### Publishing our Production Image to Azure Container Registry

```bash
REGISTRY_NAME=devtourregistry  # Your registry name
```

We log into our ACR instance, tag our image with a prefix for our ACR instance, and finally push the image.

```bash
# Automatically log Docker into your ACR
az acr login --name $REGISTRY_NAME
docker tag phototour $REGISTRY_NAME.azurecr.io/phototour
docker push $REGISTRY_NAME.azurecr.io/phototour
```

## Running a container quickly using Azure Container Instance

We can easily run a container already published to ACR. This is especially easy from the Azure Portal.

Follow [these instructions](https://docs.microsoft.com/azure/container-instances/container-instances-using-azure-container-registry) for creating a container with a public IP address listening to port 80 using the Azure portal.

> ACI can quickly deploy containers from any registry.

## Independently scaling our microservices using Kubernetes

### Prerequisites

- AKS provisioned
- Kubernetes `kubectl` CLI installed and configured to access the AKS cluster
- Give AKS access to your ACR instances

Instructions:
- [How to create an AKS cluster](https://docs.microsoft.com/azure/aks/kubernetes-walkthrough)
- Make sure to enable the [HTTP Application Routing Preview](https://docs.microsoft.com/azure/aks/http-application-routing) upon creation
- [Granting AKS access to ACR](https://docs.microsoft.com/azure/container-registry/container-registry-auth-aks)

### Separating the Frontend and Backend

Build two containers:
- `phototour-frontend` from folder `kubernetes/frontend`
- `phototour-backend` from folder `kubernetes/backend`

Tag these images:
```bash
docker tag phototour-frontend $REGISTRY_NAME.azurecr.io/phototour-frontend
docker tag phototour-backend $REGISTRY_NAME.azurecr.io/phototour-backened
```

Publish the images to ACR:
```bash
docker push $REGISTRY_NAME.azurecr.io/phototour-frontend
docker push $REGISTRY_NAME.azurecr.io/phototour-backened
```

### Deploying our Frontend and Backend to Kubernetes

In `kubernetes/backend/backend.yaml` and `kubernetes/frontend/frontend.yaml`:

- replace `<YOUR-REGISTRY>` with the name of your ACR registry
- replace `<YOUR-DNS-NAME>` with the domain name of your AKS HTTP Application Routing DNS zone (see [HTTP Application Routing Preview](https://docs.microsoft.com/azure/aks/http-application-routing)).

Create a Kubernetes secret `phototour-secret` for our connection strings to be kept secret:
```bash
kubectl create secret generic phototour-secret 
--from-literal=KEYVAULT_CLIENT_ID=XXXXX
--from-literal=KEYVAULT_CLIENT_SECRET=XXXXX 
--from-literal=KEYVAULT_URI=XXXXX
--from-literal=APPINSIGHTS_INSTRUMENTATIONKEY=XXXXX
```

Now we can deploy our Frontend and Backend services. From the `kubernetes` folder:

```bash
kubectl apply -f backend/backend.yaml
kubectl apply -f frontend/frontend.yaml
```

In a little while the DNS names `phototour.<YOUR-DNS-NAME>` and `phototour-api.<YOUR-DNS-NAME>` will resolve to the Frontend and Backend powered by our containers.

### Scaling

We can easily scale our application backend from 1 to 30 containers by running

```bash
kubectl scale deployments/phototour-api --replicas=30
```

### Caveats 
Our application deployed in Kubernetes does not support SSL via this method. You should deploy your own Ingress controller bound to a static IP address, add your own SSL certificates to the Ingress, and create a DNS record corresponding to your SSL certificate and Ingress controller public IP.