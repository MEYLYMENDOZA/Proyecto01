-- ==========================================
-- CREAR TABLA: prediccion_tendencia_log
-- ==========================================
-- Esta tabla almacena el historial de análisis de tendencias SLA
-- para permitir proyecciones basadas en datos históricos

-- Verificar si la tabla ya existe y eliminarla (solo para desarrollo)
-- COMENTAR estas líneas en producción
-- IF OBJECT_ID('prediccion_tendencia_log', 'U') IS NOT NULL
-- BEGIN
--     DROP TABLE prediccion_tendencia_log;
--     PRINT 'Tabla prediccion_tendencia_log eliminada (modo desarrollo)';
-- END

-- Crear la tabla
CREATE TABLE prediccion_tendencia_log (
    id_log INT PRIMARY KEY IDENTITY(1,1),
    tipo_sla NVARCHAR(10) NOT NULL,
    id_area INT NULL,
    fecha_analisis DATETIME NOT NULL DEFAULT GETDATE(),
    mes_analisis INT NOT NULL,
    anio_analisis INT NOT NULL,
    total_solicitudes INT NOT NULL DEFAULT 0,
    cumplen_sla INT NOT NULL DEFAULT 0,
    porcentaje_cumplimiento DECIMAL(5,2) NOT NULL DEFAULT 0,
    proyeccion_mes_siguiente DECIMAL(5,2) NULL,
    tendencia_estado NVARCHAR(50) NULL,
    usuario_solicitante NVARCHAR(100) NULL,
    ip_cliente NVARCHAR(50) NULL,
    creado_en DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT CK_prediccion_mes_valido CHECK (mes_analisis BETWEEN 1 AND 12),
    CONSTRAINT CK_prediccion_anio_valido CHECK (anio_analisis >= 2000),
    CONSTRAINT CK_prediccion_porcentaje CHECK (porcentaje_cumplimiento BETWEEN 0 AND 100),
    CONSTRAINT CK_prediccion_proyeccion CHECK (proyeccion_mes_siguiente IS NULL OR proyeccion_mes_siguiente BETWEEN 0 AND 100)
);

-- Crear índices para mejorar rendimiento
CREATE INDEX IX_prediccion_tendencia_tipo_sla 
ON prediccion_tendencia_log(tipo_sla);

CREATE INDEX IX_prediccion_tendencia_fecha 
ON prediccion_tendencia_log(anio_analisis DESC, mes_analisis DESC);

CREATE INDEX IX_prediccion_tendencia_area 
ON prediccion_tendencia_log(id_area);

CREATE INDEX IX_prediccion_tendencia_compuesto 
ON prediccion_tendencia_log(tipo_sla, anio_analisis, mes_analisis, id_area);

PRINT '';
PRINT '====================================';
PRINT '? TABLA CREADA EXITOSAMENTE';
PRINT '====================================';
PRINT 'Tabla: prediccion_tendencia_log';
PRINT 'Índices: 4 índices creados para optimizar consultas';
PRINT '';
PRINT 'Puedes verificar la tabla con:';
PRINT 'SELECT * FROM prediccion_tendencia_log';
