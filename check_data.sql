-- Verificar datos existentes
SELECT 'PropertyTypes' AS Tabla, COUNT(*) AS Total FROM PropertyTypes
UNION ALL
SELECT 'SaleTypes', COUNT(*) FROM SaleTypes
UNION ALL
SELECT 'Improvements', COUNT(*) FROM Improvements;

-- Ver datos específicos
SELECT * FROM PropertyTypes;
SELECT * FROM SaleTypes;
SELECT * FROM Improvements;
