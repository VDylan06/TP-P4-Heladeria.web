# Cómo aplicar la migración a la base de datos

## Opción 1: Consola del Administrador de Paquetes de Visual Studio 2022

1. **Establecer proyecto de inicio:**
   - Clic derecho en el proyecto `Heladeria` en el Explorador de Soluciones
   - Selecciona "Establecer como proyecto de inicio"

2. **Abrir la Consola del Administrador de Paquetes:**
   - Menú: **Herramientas** → **Administrador de paquetes NuGet** → **Consola del Administrador de paquetes**

3. **Ejecutar el comando:**
   ```
   Update-Database -Project LibreriaDeClases -StartupProject Heladeria
   ```

## Opción 2: Terminal de Visual Studio o PowerShell

1. Abre la Terminal de Visual Studio o PowerShell
2. Navega a la carpeta raíz del proyecto (donde está la solución)
3. Ejecuta:
   ```
   cd Heladeria
   dotnet ef database update --project ..\LibreriaDeClases --context LogicaHeladeria.Data.HeladeriaDbContext
   ```

## ¿Qué hace este comando?

- **Crea las tablas** `Roles`, `Usuarios` y `RolesUsuarios` en tu base de datos SQL Server
- **Registra la migración** en el historial de migraciones
- **Mantiene tus datos existentes** (tablas `Categorias` y `Helados` no se afectan)

## Verificación

Después de ejecutar, puedes verificar en SQL Server Management Studio que las nuevas tablas existen:

```sql
USE Heladeria;
SELECT * FROM Roles;
SELECT * FROM Usuarios;
SELECT * FROM RolesUsuarios;
```

Si todo salió bien, verás:
- **Roles**: 2 registros (Usuario y Administrador) - creados automáticamente por el programa
- **Usuarios**: vacía hasta que alguien se autentique con Google
- **RolesUsuarios**: vacía hasta que se asigne un rol a un usuario

