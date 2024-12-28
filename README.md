# My-Kitchen-Back-End

### Docker commands

#### Build service image
1. Make sure the build context is the root of the project. There are two ways of doing this
    1. Running the build command from the root of the project.
    2. Specifying the build context with the `--build-context` option.
1. From the root folder of the project run the following command in the terminal:

    ```bash
    docker build -t {namespace}/mykitchen-{serviceName}-service -f src/Microservices/{ServiceName}/Dockerfile .
    ```
2. The `namespace` is replaced with either a user's or organization's name if no namespace is specified, `library` is used.

3. The `serviceName` is replaced with the name of the service.

4. The `ServiceName` is replaced with the name of root folder of the service.

#### Start service
1. Navigate to service root folder. Example:
``
Example: cd src/Microservices/Identity

docker build -t sirmov/mykitchen-identity-service -f src/Microservices/Identity/Dockerfile .

docker run --name mykitchen-identity-service -e ASPNETCORE_ENVIRONMENT=Development -e RedisCacheOptions::Configuration=localhost:6379 -p 8080:80 -d sirmov/mykitchen-identity-service -v %APPDATA%/Microsoft/UserSecrets:/root/.microsoft/usersecrets

cd src/Microservices/Identity
docker compose up --build


docker run --rm -it d9dddbbc7be9 sh

### Kubernetes

Command to install ingress nginx controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0-beta.0/deploy/static/provider/cloud/deploy.yaml

Change add to host file 127.0.0.1 sirmov.com

