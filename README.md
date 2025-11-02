 Sistema de Roles y Usuarios - Heladería Web

Este sistema implementa un control de acceso basado en roles para la aplicación web de heladería. Utiliza autenticación con Google OAuth y gestiona dos tipos de roles: Usuario (lectura) y Administrador (control total).

 Permisos por Rol

- Usuario: Ver helados y categorías (solo lectura)
- Administrador: Ver + Crear/Editar/Eliminar helados y categorías

 Estructura de Base de Datos

Se utilizan tres tablas principales:

1. `Roles` - Almacena los roles disponibles en el sistema. Se crean por defecto y solo una vez 2 roles:
   - Usuario: Rol asignado a todos por defecto, permite solo lectura
   - Administrador: Permite crear, editar y eliminar, se asigna solo desde la base de datos

2. `Usuarios` - Almacena información de los usuarios autenticados con Google (su email).

3. `RolesUsuarios` - Tabla de relación muchos-a-muchos entre usuarios y roles.
   - Si `FechaBaja` tiene un valor, el rol está inactivo
   - Si `FechaBaja` es `NULL`, el rol está activo




 Inicialización Automática de Roles

Al iniciar la aplicación (en `Program.cs`), el sistema verifica si existen roles en la base de datos. Si no existen, crea automáticamente:

- Rol "Usuario" (IdRol = 1)
- Rol "Administrador" (IdRol = 2)

 Registro Automático de Usuarios

Cuando un usuario se autentica con Google:

1. Se utiliza el Middleware `RegistroUsuarioMiddleware.cs` que detecta que hay un usuario autenticado
2. Obtiene el email del usuario desde los claims de Google
3. Busca si el usuario ya existe en la tabla `Usuarios`
4. Si no existe:
   - Crea un nuevo registro en `Usuarios` con el email
   - Busca el rol "Usuario"F
   - Crea un registro en `RolesUsuarios` asignando el rol Usuario por defecto
   - `FechaBaja` queda en `NULL` (rol activo)

Resultado: Todos los usuarios nuevos automáticamente tienen el rol "Usuario" activo.

 Verificación de Permisos

El servicio `RolService.cs` contiene el método `EsAdministrador()` que:

1. Recibe el email del usuario autenticado
2. Busca el usuario en la base de datos
3. Verifica si tiene algún registro en `RolesUsuarios` donde:
   - El rol sea "Administrador"
   - Y `FechaBaja` sea `NULL` (rol activo)
4. Retorna `true` si es administrador, `false` si no




 Gestión de Roles de Administrador

IMPORTANTE: Solo se puede asignar o quitar directamente desde la base de datos. No hay interfaz web para esto.

 Asignar Rol de Administrador a un Usuario

```sql
-- Paso 1: Obtener el IdRol de Administrador
DECLARE @IdRolAdmin INT;
SELECT @IdRolAdmin = IdRol FROM Roles WHERE Descripcion = 'Administrador';

-- Paso 2: Obtener el IdUsuario del email
DECLARE @IdUsuario INT;
SELECT @IdUsuario = IdUsuario FROM Usuarios WHERE Email = 'email@ejemplo.com';

-- recomendado: Verificar si ya existe un registro (para evitar duplicados)
IF NOT EXISTS (
    SELECT 1 FROM RolesUsuarios 
    WHERE IdUsuario = @IdUsuario 
    AND IdRol = @IdRolAdmin 
    AND FechaBaja IS NULL
)
BEGIN
    -- Paso 4: Insertar el rol de administrador
    INSERT INTO RolesUsuarios (IdUsuario, IdRol, FechaBaja)
    VALUES (@IdUsuario, @IdRolAdmin, NULL);
END
```

 Desactivar Rol de Administrador (Baja Lógica)

```sql
-- Desactivar rol de Administrador de un usuario
-- Reemplaza 'email@ejemplo.com' con el email real del usuario

UPDATE RolesUsuarios
SET FechaBaja = GETDATE()
WHERE IdUsuario = (
    SELECT IdUsuario FROM Usuarios WHERE Email = 'email@ejemplo.com'
)
AND IdRol = (
    SELECT IdRol FROM Roles WHERE Descripcion = 'Administrador'
)
AND FechaBaja IS NULL; -- Solo si está activo
```

 Reactivar Rol de Administrador

Si el usuario tenía el rol pero fue desactivado, puedes reactivarlo:

```sql
UPDATE RolesUsuarios
SET FechaBaja = NULL
WHERE IdUsuario = (
    SELECT IdUsuario FROM Usuarios WHERE Email = 'email@ejemplo.com'
)
AND IdRol = (
    SELECT IdRol FROM Roles WHERE Descripcion = 'Administrador'
)
AND FechaBaja IS NOT NULL;
```

