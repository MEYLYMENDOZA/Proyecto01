-- ==========================================
-- DATOS DE PRUEBA: prediccion_tendencia_log
-- ==========================================
-- Este script inserta datos históricos de prueba para simular
-- el análisis de tendencias de los últimos 12 meses

PRINT '====================================';
PRINT 'INSERTANDO DATOS DE PRUEBA';
PRINT '====================================';

-- Verificar que la tabla existe
IF OBJECT_ID('prediccion_tendencia_log', 'U') IS NULL
BEGIN
    PRINT '? ERROR: La tabla prediccion_tendencia_log no existe.';
    PRINT 'Ejecuta primero: CREATE_TABLE_prediccion_tendencia_log.sql';
    RETURN;
END

-- Limpiar datos de prueba anteriores (opcional)
-- DELETE FROM prediccion_tendencia_log WHERE usuario_solicitante = 'DATOS_PRUEBA';

-- Insertar datos históricos para SLA001 (últimos 12 meses)
-- Simulando una tendencia MEJORANDO (porcentaje aumenta con el tiempo)
INSERT INTO prediccion_tendencia_log 
    (tipo_sla, id_area, fecha_analisis, mes_analisis, anio_analisis, 
     total_solicitudes, cumplen_sla, porcentaje_cumplimiento, 
     proyeccion_mes_siguiente, tendencia_estado, 
     usuario_solicitante, ip_cliente, creado_en)
VALUES
    -- Año 2024
    ('SLA001', 1, '2024-01-31', 1, 2024, 100, 70, 70.00, NULL, 'ESTABLE_MEDIO', 'DATOS_PRUEBA', '127.0.0.1', '2024-01-31'),
    ('SLA001', 1, '2024-02-29', 2, 2024, 105, 75, 71.43, NULL, 'ESTABLE_MEDIO', 'DATOS_PRUEBA', '127.0.0.1', '2024-02-29'),
    ('SLA001', 1, '2024-03-31', 3, 2024, 110, 82, 74.55, NULL, 'ESTABLE_MEDIO', 'DATOS_PRUEBA', '127.0.0.1', '2024-03-31'),
    ('SLA001', 1, '2024-04-30', 4, 2024, 115, 90, 78.26, NULL, 'ESTABLE_MEDIO', 'DATOS_PRUEBA', '127.0.0.1', '2024-04-30'),
    ('SLA001', 1, '2024-05-31', 5, 2024, 120, 96, 80.00, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-05-31'),
    ('SLA001', 1, '2024-06-30', 6, 2024, 125, 103, 82.40, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-06-30'),
    ('SLA001', 1, '2024-07-31', 7, 2024, 130, 110, 84.62, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-07-31'),
    ('SLA001', 1, '2024-08-31', 8, 2024, 135, 117, 86.67, NULL, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-08-31'),
    ('SLA001', 1, '2024-09-30', 9, 2024, 140, 124, 88.57, NULL, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-09-30'),
    ('SLA001', 1, '2024-10-31', 10, 2024, 145, 131, 90.34, NULL, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-10-31'),
    ('SLA001', 1, '2024-11-30', 11, 2024, 150, 138, 92.00, 93.50, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-11-30'),
    ('SLA001', 1, '2024-12-31', 12, 2024, 155, 145, 93.55, 94.80, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-12-31');

-- Insertar datos para SLA002 (últimos 6 meses)
-- Simulando una tendencia EMPEORANDO (porcentaje disminuye con el tiempo)
INSERT INTO prediccion_tendencia_log 
    (tipo_sla, id_area, fecha_analisis, mes_analisis, anio_analisis, 
     total_solicitudes, cumplen_sla, porcentaje_cumplimiento, 
     proyeccion_mes_siguiente, tendencia_estado, 
     usuario_solicitante, ip_cliente, creado_en)
VALUES
    ('SLA002', 2, '2024-07-31', 7, 2024, 80, 72, 90.00, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-07-31'),
    ('SLA002', 2, '2024-08-31', 8, 2024, 85, 73, 85.88, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-08-31'),
    ('SLA002', 2, '2024-09-30', 9, 2024, 90, 72, 80.00, NULL, 'ESTABLE_ALTO', 'DATOS_PRUEBA', '127.0.0.1', '2024-09-30'),
    ('SLA002', 2, '2024-10-31', 10, 2024, 95, 69, 72.63, NULL, 'ESTABLE_MEDIO', 'DATOS_PRUEBA', '127.0.0.1', '2024-10-31'),
    ('SLA002', 2, '2024-11-30', 11, 2024, 100, 65, 65.00, 60.00, 'EMPEORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-11-30'),
    ('SLA002', 2, '2024-12-31', 12, 2024, 105, 60, 57.14, 52.00, 'EMPEORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2024-12-31');

-- Insertar algunos datos para 2025
INSERT INTO prediccion_tendencia_log 
    (tipo_sla, id_area, fecha_analisis, mes_analisis, anio_analisis, 
     total_solicitudes, cumplen_sla, porcentaje_cumplimiento, 
     proyeccion_mes_siguiente, tendencia_estado, 
     usuario_solicitante, ip_cliente, creado_en)
VALUES
    ('SLA001', 1, '2025-01-31', 1, 2025, 160, 150, 93.75, 94.50, 'MEJORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2025-01-31'),
    ('SLA002', 2, '2025-01-31', 1, 2025, 110, 55, 50.00, 45.00, 'EMPEORANDO', 'DATOS_PRUEBA', '127.0.0.1', '2025-01-31');

PRINT '';
PRINT '====================================';
PRINT '? DATOS INSERTADOS EXITOSAMENTE';
PRINT '====================================';

-- Mostrar resumen de datos insertados
PRINT '';
PRINT 'Resumen de datos insertados:';
SELECT 
    tipo_sla AS 'Tipo SLA',
    COUNT(*) AS 'Total Registros',
    MIN(anio_analisis) AS 'Año Inicio',
    MAX(anio_analisis) AS 'Año Fin',
    AVG(porcentaje_cumplimiento) AS 'Promedio %',
    MIN(porcentaje_cumplimiento) AS 'Min %',
    MAX(porcentaje_cumplimiento) AS 'Max %'
FROM prediccion_tendencia_log
WHERE usuario_solicitante = 'DATOS_PRUEBA'
GROUP BY tipo_sla
ORDER BY tipo_sla;

PRINT '';
PRINT '====================================';
PRINT 'VERIFICACIÓN';
PRINT '====================================';
PRINT 'Puedes consultar los datos con:';
PRINT 'SELECT * FROM prediccion_tendencia_log ORDER BY anio_analisis DESC, mes_analisis DESC';
PRINT '';
PRINT 'Ahora puedes probar el endpoint desde tu app Android:';
PRINT 'GET /api/reporte/tendencia?tipoSla=SLA001&anio=2024';
PRINT 'GET /api/reporte/tendencia?tipoSla=SLA002&anio=2024';
