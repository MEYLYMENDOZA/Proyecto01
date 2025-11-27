-- ==========================================
-- SCRIPT DE DIAGNÓSTICO - REPORTE TENDENCIA
-- ==========================================

-- 1. Verificar códigos SLA disponibles
PRINT '====================================';
PRINT '1. CÓDIGOS SLA DISPONIBLES';
PRINT '====================================';
SELECT 
    id_sla AS IdSla,
    codigo_sla AS CodigoSla,
    descripcion AS Descripcion,
    dias_umbral AS DiasUmbral,
    es_activo AS EsActivo
FROM config_sla
ORDER BY codigo_sla;

-- 2. Verificar solicitudes por año
PRINT '';
PRINT '====================================';
PRINT '2. SOLICITUDES POR AÑO';
PRINT '====================================';
SELECT 
    YEAR(fecha_solicitud) AS Anio,
    COUNT(*) AS TotalSolicitudes
FROM solicitud
WHERE fecha_solicitud IS NOT NULL
GROUP BY YEAR(fecha_solicitud)
ORDER BY Anio DESC;

-- 3. Verificar solicitudes por SLA y año
PRINT '';
PRINT '====================================';
PRINT '3. SOLICITUDES POR SLA Y AÑO';
PRINT '====================================';
SELECT 
    cs.codigo_sla AS CodigoSla,
    YEAR(s.fecha_solicitud) AS Anio,
    COUNT(*) AS TotalSolicitudes
FROM solicitud s
INNER JOIN config_sla cs ON s.id_sla = cs.id_sla
WHERE s.fecha_solicitud IS NOT NULL
GROUP BY cs.codigo_sla, YEAR(s.fecha_solicitud)
ORDER BY cs.codigo_sla, Anio DESC;

-- 4. Verificar solicitudes para SLA002 en 2023 (el caso del error)
PRINT '';
PRINT '====================================';
PRINT '4. DETALLE SOLICITUDES SLA002 - 2023';
PRINT '====================================';
SELECT 
    s.id_solicitud AS IdSolicitud,
    cs.codigo_sla AS CodigoSla,
    s.fecha_solicitud AS FechaSolicitud,
    s.num_dias_sla AS NumDiasSla,
    cs.dias_umbral AS DiasUmbral,
    CASE 
        WHEN s.num_dias_sla <= cs.dias_umbral THEN 'CUMPLE'
        ELSE 'NO CUMPLE'
    END AS EstadoSLA
FROM solicitud s
INNER JOIN config_sla cs ON s.id_sla = cs.id_sla
WHERE cs.codigo_sla = 'SLA002'
    AND YEAR(s.fecha_solicitud) = 2023
ORDER BY s.fecha_solicitud;

-- 5. Verificar si existe configuración para SLA002
PRINT '';
PRINT '====================================';
PRINT '5. CONFIGURACIÓN SLA002';
PRINT '====================================';
SELECT 
    id_sla AS IdSla,
    codigo_sla AS CodigoSla,
    descripcion AS Descripcion,
    dias_umbral AS DiasUmbral,
    es_activo AS EsActivo
FROM config_sla
WHERE UPPER(codigo_sla) = 'SLA002';

-- 6. Verificar tabla de logs de tendencia
PRINT '';
PRINT '====================================';
PRINT '6. LOGS DE TENDENCIA EXISTENTES';
PRINT '====================================';
IF OBJECT_ID('prediccion_tendencia_log', 'U') IS NOT NULL
BEGIN
    SELECT 
        id_log AS IdLog,
        tipo_sla AS TipoSla,
        anio_analisis AS Anio,
        mes_analisis AS Mes,
        total_solicitudes AS TotalSolicitudes,
        porcentaje_cumplimiento AS PorcentajeCumplimiento,
        tendencia_estado AS EstadoTendencia,
        fecha_analisis AS FechaAnalisis
    FROM prediccion_tendencia_log
    ORDER BY anio_analisis DESC, mes_analisis DESC;
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: La tabla prediccion_tendencia_log no existe.';
    PRINT 'Ejecuta el script de creación de la tabla.';
END

-- 7. Verificar áreas disponibles
PRINT '';
PRINT '====================================';
PRINT '7. ÁREAS CON SOLICITUDES';
PRINT '====================================';
SELECT DISTINCT
    s.id_area AS IdArea,
    COUNT(*) AS TotalSolicitudes
FROM solicitud s
WHERE s.id_area IS NOT NULL
GROUP BY s.id_area
ORDER BY s.id_area;

-- ==========================================
-- RESULTADO ESPERADO
-- ==========================================
PRINT '';
PRINT '====================================';
PRINT 'DIAGNÓSTICO COMPLETADO';
PRINT '====================================';
PRINT 'Revisa los resultados anteriores para:';
PRINT '1. Verificar que existe SLA002 activo';
PRINT '2. Confirmar que hay solicitudes para 2023';
PRINT '3. Verificar que las solicitudes tienen id_sla correcto';
PRINT '4. Confirmar que existe la tabla prediccion_tendencia_log';
