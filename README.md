# PIX API (APIx)

The APIx offers services to Payment Providers (PSPs) create pix keys, consult pix keys and make a transaction by pix.

## About

This API allows PSPs to create pix keys, specifying pix key and value, user CPF. PSPs can also consult about a pix key, receiving information about the account, PSP and user linked to this pix key. Finally, the PSP can  easily make a transaction by pix.

## Running for Development

1. Clone this repository.
2. Install all dependencies (it maybe necessary additional installs like .NET SDK 8.0):

```bash
dotnet restore
```

3. Configure the connection variables in appsettings.json to connect with a PostgreSQL database.
4. Install the entity framework tool globally and run the migrations to create the database.

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

5. Start the back-end in a development environment:

```bash
dotnet watch
```

6. Look and test the API documentation using Swagger. If you don't change the default port it can be acessed in [http://localhost:5045/swagger/index.html](http://localhost:5045/swagger/index.html).

## Monitoring the Application

1. Build the app image using Docker (maintain the appsettings configuration for default connection with the postgres container):

```bash
docker build . -t dotnetapi
```

2. Change path to folder Monitor and run the docker-compose file:

```bash
docker compose up
```

3. Now the app, the PostgreSQL the monitors and the Grafana are running. You can watch the app performance by acessing [http://localhost:3000](http://localhost:3000).

4. Make the configuration to acess data provided by Prometheus and choose some dashboards to organize the data about PostgreSQL container and/or app container.

## Loading Tests

1. You can run the provided scripts about loading tests changing the path to .k6

2. Install the dependencies:

```bash
npm i
```

3. Run the seed script:

```bash
npm run seed
```

4. Run the test script:

```bash
npm run test:post-keys
```