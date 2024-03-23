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

3. Configure the connection variables in appsettings.json to connect with a PostgreSQL database, RabbitMQ message broker and Redis memory cache.
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
1. Acess [mock repository](https://github.com/GDMeira/PSP-Mock), [payment consumer](https://github.com/GDMeira/PaymentQueueConsumer) and [concilliation consumer](https://github.com/GDMeira/ConcilliationQueueConsumer) to get the code for use and test APIx endpoints. Unfortunaly the consumer docker images isn't connecting with RabbitMQ, but it works running locally.

2. Build the app image using Docker (maintain the appsettings configuration for default connection with other containers and services):

```bash
docker build . -t dotnetapi
```

3. Change path to folder Monitor and run the docker-compose file:

```bash
docker compose up
```

4. Now the app, the PostgreSQL, the RabbitMQ, the Redis, the monitors and the Grafana are running. You can watch the app performance by acessing [http://localhost:3000](http://localhost:3000).

5. Make the configuration to acess data provided by Prometheus (http://prometheus:9090) and choose some dashboards to organize the data about PostgreSQL container and/or app container.

6. You can acess the [RabbitMQ Manager](http://http://localhost:15672) using the default user 'guest' and password 'guest'.

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
npm run test:get-keys
```

5. To run the post payments load test clone the mock and consumer repositories, generate the psp-mock docker image from the mock folder and run some consumers on terminal.

6. After that seed the pix keys and accounts:

```bash
npm run seed:pix-key
```

7. And run the test script:

```bash
npm run test:post-payments
```

8. You can adjust the test load by changing the options object:

```javascript
export const options = {
    scenarios: {
        contacts: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '30s', target: 50 }, // target is the number of virtual users (VU)
                { duration: '20s', target: 25 },
                { duration: '10s', target: 0 },
            ],
            gracefulRampDown: '0s',
        },
    }
}

9. To test the post concilliation endpoint run:

```bash
npm run seed:payment
```

10. A .ndjson file will be generated in /.k6/seed. Use some platform (like google drive) to upload the file and generated an download link. You can change some data at the file to see the results in report. Use the download link to make a request in /Concilliation using swagger:

```json
{
  "file": "https://drive.google.com/uc?export=download&id=1rPU132RV9BuJamLId-Yiy7YcZYiCzLxk",
  "postBack": "http://psp-mock:5039/concilliation/pix", 
  "date": "2024-03-22"
}
```

11. Run ```docker logs psp-mock``` in a terminal to see the generated report. It uses to spend 3 minutes per report.