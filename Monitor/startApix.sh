#!/bin/bash

until nc -z rabbitmq 5672; do
    echo "Aguardando o RabbitMQ iniciar..."
    sleep 2
done

dotnet APIx.dll
