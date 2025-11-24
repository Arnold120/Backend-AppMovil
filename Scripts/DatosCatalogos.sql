INSERT INTO Marcas (NombreMarca, Activo, FechaRegistro) VALUES
('Norma', 1, GETDATE()),
('Kores', 1, GETDATE()),
('Faber-Castell', 1, GETDATE()),
('Staedtler', 1, GETDATE()),
('Paper Mate', 1, GETDATE()),
('Bic', 1, GETDATE());

INSERT INTO Categorias (NombreCategoria, Descripcion, Activo, FechaRegistro) VALUES
('Cuadernos', 'Cuadernos universitarios y escolares', 1, GETDATE()),
('Lápices y Lapiceros', 'Artículos para escritura', 1, GETDATE()),
('Arte y Dibujo', 'Artículos para dibujo y arte', 1, GETDATE()),
('Oficina', 'Accesorios para oficina', 1, GETDATE()),
('Papel', 'Resmas, hojas y cartulinas', 1, GETDATE()),
('Adhesivos', 'Gomas, silicones y pegamentos', 1, GETDATE());

INSERT INTO Proveedores (NombreEmpresa, Direccion, Telefono, Email, AceptaDevoluciones, TiempoDevolucion, PorcentajeCobertura)
VALUES
('Distribuidora El Escolar', 'Managua, Nicaragua', '8888-1111', 'contacto@escolar.com', 1, 30, 100),
('Office Depot Mayorista', 'Carretera Masaya', '2250-9988', 'ventas@officedepotni.com', 1, 15, 80),
('Suministros Creativos', 'León, Nicaragua', '8670-4455', 'info@screativos.com', 0, 0, 0),
('Distribuidora Faber Centroamérica', 'Managua', '2231-7788', 'servicio@faber-ca.com', 1, 20, 100),
('Papelería Continental', 'Granada, Nicaragua', '7644-2211', 'ventas@continental.com', 1, 7, 50);

INSERT INTO Clientes (Nombre, Apellido, Direccion, Telefono, Email, Activo)
VALUES
('Carlos', 'Méndez', 'Bo. Monseñor Lezcano, Managua', '8888-2211', 'cmendez@gmail.com', 1),
('Ana', 'Gutiérrez', 'Bo. San Judas, Managua', '8675-1122', 'ana.gtz@hotmail.com', 1),
('Luis', 'Pérez', 'Ciudad Sandino', '7855-4422', 'l.perez@yahoo.com', 1),
('Escuela San Martín', NULL, 'Carretera Norte', '2244-5566', 'ventas@sanmartin.edu.ni', 1),
('OfiMart', NULL, 'Metrocentro Managua', '2233-1100', 'compras@ofimart.com', 1);

INSERT INTO Productos (Marca_ID, Categoria_ID, Codigo, NombreProducto, UnidadMedida, CapacidadUnidad, Cantidad, Activo, FechaRegistro)
VALUES
(1, 1, 1001, 'Cuaderno Universitario Norma 100 Hojas', 'UNIDAD', 1, 50, 1, GETDATE()),
(6, 2, 1002, 'Lapicero Azul Bic Cristal', 'UNIDAD', 1, 500, 1, GETDATE()),
(3, 3, 1003, 'Lápiz Grafito Faber-Castell 2B', 'UNIDAD', 1, 200, 1, GETDATE()),
(4, 3, 1004, 'Borrador Staedtler Dust-Free', 'UNIDAD', 1, 150, 1, GETDATE()),
(2, 6, 1005, 'Goma Kores 40g', 'UNIDAD', 1, 120, 1, GETDATE()),
(5, 4, 1006, 'Marcador Permanente Paper Mate Negro', 'UNIDAD', 1, 80, 1, GETDATE()),
(1, 5, 1007, 'Resma Papel Carta Norma 500 hojas', 'PAQUETE', 500, 30, 1, GETDATE());


