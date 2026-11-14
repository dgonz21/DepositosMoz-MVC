# Depósitos Moz - Etapa 3

## Requisitos
- .NET SDK 10
- SQL Server Express
- Visual Studio Code o Visual Studio
- Git

## Configuración de base de datos

1. Clonar el repositorio.
2. Instalar SQL Server Express.
3. Verificar que exista la instancia .\SQLEXPRESS.
4. Comprobar la cadena de conexión en:
   DepositosMoz.Web/appsettings.json

La base de datos se llama DepositosMozDB.

## Restaurar dependencias

dotnet restore

## Crear la base de datos

dotnet tool install --global dotnet-ef --version 10.0.0

Si dotnet-ef ya está instalado, omitir ese comando.

dotnet ef database update --project .\DepositosMoz.Web --startup-project .\DepositosMoz.Web

Las migraciones crean las tablas y relaciones necesarias.

## Ejecutar aplicación

dotnet run --project .\DepositosMoz.Web

## Ejecutar pruebas

dotnet test .\DepositosMoz.Tests\DepositosMoz.Tests.csproj

## Componentes implementados

- Entity Framework Core
- SQL Server
- DbContext
- Relaciones entre entidades
- Migraciones
- Repositorios genéricos
- Interfaces de repositorios
- Consultas LINQ
- Inyección de dependencias

## Importante

La base de datos local no se almacena en GitHub.
Cada desarrollador debe ejecutar las migraciones en su SQL Server.

La integración de los controladores MVC con los repositorios
y las consultas corresponde al trabajo conjunto del equipo.
