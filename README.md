**Для проверки задания следует смотреть файлы проектов с припиской "Service"**

The 5th and 6th labs were done differently from lector's tasks
Instead of sending data from service to service, every service stores its data to the shared database and then send a message via rabbitmq to another service
For launching all services with database and rabbitmq just exec docker-compose file