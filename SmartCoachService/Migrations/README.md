# Migrations

EF Core migrations are generated locally / in CI once the connection string points
to a real PostgreSQL instance (they cannot be generated offline in this environment).

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -o Migrations
dotnet ef database update
```
