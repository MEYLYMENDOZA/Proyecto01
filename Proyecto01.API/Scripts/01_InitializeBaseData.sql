-- =====================================================
-- Script de Inicialización de Datos Base
-- Base de Datos: Proyecto01
-- =====================================================

USE Proyecto01;
GO

-- =====================================================
-- 1. ROLES DEL SISTEMA
-- =====================================================
PRINT 'Insertando Roles del Sistema...';

-- Verificar si ya existen roles
IF NOT EXISTS (SELECT 1 FROM roles_sistema WHERE Codigo = 'ADMIN')
BEGIN
    INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
    VALUES ('ADMIN', 'Administrador', 'Rol con todos los permisos del sistema', 1);
    PRINT '? Rol ADMIN creado';
END
ELSE
    PRINT '- Rol ADMIN ya existe';

IF NOT EXISTS (SELECT 1 FROM roles_sistema WHERE Codigo = 'USER')
BEGIN
    INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
    VALUES ('USER', 'Usuario', 'Rol de usuario básico', 1);
    PRINT '? Rol USER creado';
END
ELSE
    PRINT '- Rol USER ya existe';

IF NOT EXISTS (SELECT 1 FROM roles_sistema WHERE Codigo = 'GESTOR')
BEGIN
    INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
    VALUES ('GESTOR', 'Gestor', 'Rol de gestor de solicitudes', 1);
    PRINT '? Rol GESTOR creado';
END
ELSE
    PRINT '- Rol GESTOR ya existe';

-- =====================================================
-- 2. ESTADOS DE USUARIO
-- =====================================================
PRINT '';
PRINT 'Insertando Estados de Usuario...';

IF NOT EXISTS (SELECT 1 FROM estado_usuario_catalogo WHERE Codigo = 'ACTIVO')
BEGIN
    INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
    VALUES ('ACTIVO', 'Usuario activo en el sistema', 1);
    PRINT '? Estado ACTIVO creado';
END
ELSE
    PRINT '- Estado ACTIVO ya existe';

IF NOT EXISTS (SELECT 1 FROM estado_usuario_catalogo WHERE Codigo = 'INACTIVO')
BEGIN
    INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
    VALUES ('INACTIVO', 'Usuario inactivo temporalmente', 1);
    PRINT '? Estado INACTIVO creado';
END
ELSE
    PRINT '- Estado INACTIVO ya existe';

IF NOT EXISTS (SELECT 1 FROM estado_usuario_catalogo WHERE Codigo = 'BLOQUEADO')
BEGIN
    INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
    VALUES ('BLOQUEADO', 'Usuario bloqueado por seguridad', 1);
    PRINT '? Estado BLOQUEADO creado';
END
ELSE
    PRINT '- Estado BLOQUEADO ya existe';

-- =====================================================
-- 3. VERIFICACIÓN DE DATOS INSERTADOS
-- =====================================================
PRINT '';
PRINT '=====================================================';
PRINT 'VERIFICACIÓN DE DATOS INSERTADOS';
PRINT '=====================================================';

PRINT '';
PRINT 'Roles del Sistema:';
SELECT id_rol_sistema AS ID, Codigo, Nombre, EsActivo 
FROM roles_sistema 
ORDER BY id_rol_sistema;

PRINT '';
PRINT 'Estados de Usuario:';
SELECT IdEstadoUsuario AS ID, Codigo, Descripcion, EsActivo 
FROM estado_usuario_catalogo 
ORDER BY IdEstadoUsuario;

PRINT '';
PRINT '=====================================================';
PRINT '? Script completado exitosamente';
PRINT '=====================================================';
PRINT '';
PRINT 'Ahora puedes crear usuarios usando:';
PRINT '  - idRolSistema: 1 (ADMIN), 2 (USER), 3 (GESTOR)';
PRINT '  - idEstadoUsuario: 1 (ACTIVO), 2 (INACTIVO), 3 (BLOQUEADO)';
GO
