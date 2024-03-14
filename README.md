## Usage
- Use _db/compose.yaml_ to setup the database server
  - `docker compose up -d` to start the MS SQL Server container
  - `docker compose run --rm liquibase update` to execute the database migrations
- Run _src/WebApi/WebApi.csproj_ to start the application server
