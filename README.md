# Voting Application

A distributed voting application running on OpenShift. It consists of multiple components to handle voting, results processing, and background tasks.

## Components

- **Vote Frontend** (Python Flask) - Port `8080`  
- **Result Frontend** (Node.js) - Port `8080`  
- **Worker Service** (.NET Core)  
- **Redis** (in-memory database)  
- **PostgreSQL** (persistent database with dynamic storage)  

## Prerequisites

- OpenShift cluster access with `oc` CLI configured.  
- Docker/Podman registry to push your application images.  
- Dynamic storage provisioner available for PostgreSQL.  

## Create Service Account for PostgreSQL Deployment
- oc create sa postgres-sa -n voting-app
- oc adm policy add-scc-to-user anyuid -z postgres-sa -n voting-app

## Quick Start

1. **Build and push images** to your registry.  
2. **Update image references** in the deployment YAML files.  
3. **Deploy the application** on OpenShift:

```bash
oc apply -f k8s-specifications/

