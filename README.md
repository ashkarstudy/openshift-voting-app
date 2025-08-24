# Voting Application

A distributed voting application running on OpenShift with the following components:

- Vote frontend (Python Flask) - Port 8080
- Result frontend (Node.js) - Port 8080  
- Worker service (.NET Core)
- Redis (in-memory database)
- PostgreSQL (persistent database)

## Quick Start

1. Build and push images to your registry
2. Update image references in deployment YAML files
3. Deploy to OpenShift: `oc apply -f k8s-specifications/`
4. Access the vote and result frontends via the created routes

## Architecture

[Include your architecture diagram here]

## Development

Use `docker-compose up` for local development.
