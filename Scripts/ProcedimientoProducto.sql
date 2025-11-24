CREATE VIEW vw_CatalogoProductos AS
SELECT 
    P.Producto_ID,
    P.NombreProducto,
    P.Codigo,
    M.NombreMarca AS Marca,
    C.NombreCategoria AS Categoria,
    P.UnidadMedida,
    P.CapacidadUnidad,
    P.Cantidad AS StockActual,
    PP.PrecioVenta,
    PP.CostoCompra,
    PP.MargenGanancia,
    PP.PorcentajeMargen,
    CASE 
        WHEN P.Cantidad > 0 THEN 'Disponible'
        ELSE 'Agotado'
    END AS EstadoStock,
    P.Activo AS ProductoActivo,
    PP.Activo AS PrecioActivo,
    P.FechaRegistro AS FechaRegistroProducto,
    PP.FechaRegistro AS FechaRegistroPrecio
FROM Productos P
INNER JOIN PrecioProducto PP ON P.Producto_ID = PP.Producto_ID
INNER JOIN Marcas M ON P.Marca_ID = M.Marca_ID
INNER JOIN Categorias C ON P.Categoria_ID = C.Categoria_ID
WHERE P.Activo = 1 AND PP.Activo = 1;