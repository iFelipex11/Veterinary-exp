# Veterinary

Aplicacion de ejemplo con `Blazor WebAssembly`, `ASP.NET Core`, `Entity Framework Core`, `Identity` y `JWT`.

## Proyectos

- `Veterinary.API`: backend con EF Core, Identity, JWT, Swagger y seed.
- `Veterinary.WEB`: frontend Blazor WebAssembly.
- `Veterinary.Shared`: entidades y DTOs compartidos.

## Requisitos

- `.NET 10 SDK`
- `SQL Server` local

## Configuracion

Actualiza en [`Veterinary.API/appsettings.Development.json`](C:/Users/Whiterose/.codex/worktrees/f030/Veterinary-exp/Veterinary.API/appsettings.Development.json):

- `ConnectionStrings:DefaultConnection`
- `jwtKey`
- `frontendUrl`
- bloque `mail` para confirmacion y recuperacion por correo

## Ejecucion

1. Ejecuta `Veterinary.API`
2. Ejecuta `Veterinary.WEB`

El seed crea un usuario administrador base:

- correo: `oap@yopmail.com`
- contrasena: `123456`

## Funcionalidades actuales

- registro y login con JWT
- confirmacion de correo
- reenvio de confirmacion
- recuperacion y reseteo de contrasena
- edicion de perfil y cambio de contrasena
- carga de foto de usuario
- CRUD inicial de propietarios
