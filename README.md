# My-Kitchen-Back-End

### Docker commands

#### Build service image
1. Make sure the build context is the src folder of the project. There are two ways of doing this
    1. Running the build command from the src folder.
    2. Specify the build context relative to the current directory.
    ```bash
    docker build -t {namespace}/mykitchen-{serviceName}-service -f {dockerFilePath} {buildDirectory}
    ```
2. The `{namespace}` is replaced with either a user's or organization's name if no namespace is specified, `library` is used.

5. The `{serviceName}` is replaced with the name of the service.

5. The `{dockerFilePath}` is replaced with the path to the docker file relative to the current directory.

6. The `{buildDirectory}` is replaced with the path to the src folder relative to the current directory.

#### Debug image building

1. Set `DOCKER_BUILDKIT` variable to 0.
- Windows: 
    - Command line: `set DOCKER_BUILDKIT=0`
    - PowerShell: `set DOCKER_BUILDKIT=0`
- Linux or macOS: `DOCKER_BUILDKIT=0 docker build ...`
2. Using the image layer id start a shell session.
```bash
docker run --rm -it {imageLayerId} sh
```

#### Start service image
- Using Docker Compose (recommended)
1. Navigate to the root of the microservice.
2. Run the following command
```bash
docker compose up --build --detach
```
3. Note that the docker compose file may includes the dependency services also, like databases, development tools,
message brokers and others . If you want to start only the microservice you can add the `--no-deps` flag.

- Using Docker CLI
1. Make sure you have built the service image first.
2. Run the following command
```bash
docker run -d \
  --name={containerName} \
  --env-file {envFilePath} \
  -p {hostPort}:{containerPort}/{protocol} \
  -v {userSecretsDirectory}:/root/.microsoft/usersecrets `#optional` \
  --restart unless-stopped \
  {serviceImage}
```
4. Replace `{containerName}` with the name of the container.
5. Replace `{envFilePath}` with the path to the env file holding the application secrets and configuration.
6. To expose container ports and bind them to the host replace `{hostPort}`, `{containerPort}` and `{protocol}` accordingly.
7. Optionally you can bind mount the user secrets on the host machine to the container by specifying the
`-v {userSecretsDirectory}:/root/.microsoft/usersecrets` and replacing `{userSecretsDirectory}` with the directory
where user secrets are stored on the host machine. Usually `%APPDATA%/Microsoft/UserSecrets` on Windows
and `~/.microsoft/usersecrets` on Linux.
8. Replace `{serviceImage}` with the image of the service.
Example:

```bash
docker run -d \
  --name=mykitchen-identity-rest-service \
  --env-file ./development.env \
  -p 5000:5000/tcp \
  -v %APPDATA%/Microsoft/UserSecrets:/root/.microsoft/usersecrets \
  --restart unless-stopped \
  sirmov/mykitchen-identity-rest-service:latest
```


### Kubernetes

Command to install ingress nginx controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0-beta.0/deploy/static/provider/cloud/deploy.yaml

Change add to host file 127.0.0.1 sirmov.com

