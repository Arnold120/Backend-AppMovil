DROP DATABASE IF EXISTS backendLibreriaMovil;
GO

CREATE DATABASE backendLibreriaMovil;
GO

USE backendLibreriaMovil;
GO

CREATE TABLE Usuario (
	Usuario_ID INT PRIMARY KEY IDENTITY,
	NombreUsuario VARCHAR(100) UNIQUE,
	Contraseña VARCHAR(100),
	Salt VARBINARY(64),
	FechaRegistro DATETIME
);
select * from Usuario
select * from Rol
select * from UsuarioRol
select * from Marca
select * from Clientes

CREATE TABLE Rol (
	Rol_ID INT PRIMARY KEY IDENTITY,
	NombreRol VARCHAR(100) NOT NULL,
	Descripcion VARCHAR(100),
	Activo BIT DEFAULT 1,
	FechaRegistro DATETIME
);

CREATE TABLE UsuarioRol (
	UsuarioRol_ID INT PRIMARY KEY IDENTITY,
	Usuario_ID INT,
	Rol_ID INT,
	FechaAsignacion DATETIME,
	FOREIGN KEY (Usuario_ID) REFERENCES Usuario(Usuario_ID),
	FOREIGN KEY (Rol_ID) REFERENCES Rol(Rol_ID)
);

-- tabla Clientes
CREATE TABLE Clientes (
    Cliente_ID INT PRIMARY KEY IDENTITY,
    Nombre VARCHAR(100) NOT NULL DEFAULT 'Consumidor Final',
    Apellido VARCHAR(100) NOT NULL DEFAULT 'No especificado',
    Direccion VARCHAR(255) NULL,
    Telefono VARCHAR(50) NULL,
    Email VARCHAR(100) NULL,
    Activo BIT DEFAULT 1,
    FechaRegistro DATETIME DEFAULT GETDATE()
);

CREATE TABLE Provedores(
    Proveedor_ID INT PRIMARY KEY IDENTITY,
    NombreEmpresa VARCHAR(255),  -- la sucursal del proveedor
    Direccion VARCHAR(255),
    Telefono VARCHAR(50),
    Email VARCHAR(100),
    AceptaDevoluciones BIT DEFAULT 0, -- si=intento de reclamo, no=perdidas 
    TiempoDevolucion INT DEFAULT 15, -- Días límite para hacer devoluciones segun el proveedor
    PorcentajeCobertura DECIMAL(5,2) DEFAULT 0  -- % del costo que cubre, ej: 100 = cubre todo
);

CREATE TABLE Marca(
	Marca_ID INT PRIMARY KEY IDENTITY,
	NombreMarca VARCHAR(100),
	Activo BIT,
	FechaRegistro DATETIME
);

CREATE TABLE Categoria(
	Categoria_ID INT PRIMARY KEY IDENTITY,
	NombreCategoria VARCHAR(100),
	Descripcion VARCHAR(100),
	Activo BIT,
	FechaRegistro DATETIME
);

CREATE TABLE Productos (
	Producto_ID INT PRIMARY KEY IDENTITY,
	Marca_ID INT,
	Categoria_ID INT,
	Codigo INT,
	NombreProducto VARCHAR(100) UNIQUE,
	UnidadMedida VARCHAR(20) DEFAULT 'UNIDAD', -- UNIDAD, CAJA, PAQUETE, etc.
	CapacidadUnidad INT DEFAULT 1,
	Cantidad INT DEFAULT 0,
	Activo BIT,
	FechaRegistro DATETIME,
	FOREIGN KEY (Marca_ID) REFERENCES Marca(Marca_ID),
	FOREIGN KEY (Categoria_ID) REFERENCES Categoria(Categoria_ID)
);

-- Tabla de Precios (estructura más clara y completa)(OPCION MAS RENTABLE)
CREATE TABLE PrecioProducto (
    Precio_ID INT PRIMARY KEY IDENTITY(1,1),
    Producto_ID INT,
    CostoCompra DECIMAL(10,2), --El precio que se compro ese producto en unidad
    PrecioVenta DECIMAL(10,2), --El precio de venta para los clientes de la tienda
    MargenGanancia DECIMAL(5,2),  -- Campo en donde se calcula el margen de ganancia por producto(precioventa - costocompra)
    PorcentajeMargen DECIMAL(5,2),  --Porcentaje de ganancia respecto al costo(margenganacia/costocompra * 100)
    Activo BIT,  
    FechaRegistro DATETIME,
    UsuarioModificacion VARCHAR(50),  -- Auditoría: quién modificó el precio
	FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);


-- Ajuste en tabla MovimientoInventario en relacion con StockInventario
CREATE TABLE MovimientoInventario (
	MovimientoInventario_ID INT PRIMARY KEY IDENTITY,
	Producto_ID INT,
	PrecioProducto_ID INT,
	TipoMovimiento VARCHAR(50) CHECK (TipoMovimiento IN ('Entrada', 'Salida', 'Ajuste')),
	Cantidad INT NOT NULL,
	Precio DECIMAL(10,2),
	Referencia_ID INT NULL, --MAS Q TODO EL ID DE QUIEN HIZO LA COMPRA, VENTA O DEVOLUCION
	TipoReferencia VARCHAR(50) NULL, -- LA TABLA EN QUE SE HIZO LOS CAMBIOS
	FechaMovimiento DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID),
	FOREIGN KEY (PrecioProducto_ID) REFERENCES PrecioProducto(Precio_ID)
);

CREATE TABLE Compras (
	Compra_ID INT PRIMARY KEY IDENTITY,
	Usuario_ID INT,       -- quien registra la compra
	Proveedor_ID INT,     -- proveedor de la compra
	CantidadTotal INT,
	MontoTotal DECIMAL(10,2),
    FechaRegistro DATETIME DEFAULT GETDATE(),
	IVATotal DECIMAL(10,2) DEFAULT 0, --IVA en general: Si se compraron varios productos y algunos traen descuento y otros no entonces aqui se suma el total en general de esos IVAS
    SubTotal DECIMAL(10,2) NOT NULL, -- Suma antes de IVA (CantidadTotal * Monto total)
    Total DECIMAL(10,2),    -- Total final ([CantidadTotal * Monto total] + IVA)
	FOREIGN KEY (Proveedor_ID) REFERENCES Provedores(Proveedor_ID),
	FOREIGN KEY (Usuario_ID) REFERENCES Usuario(Usuario_ID)
);

CREATE TABLE Detalles_Compra (
	DetalleCompra_ID INT PRIMARY KEY IDENTITY,
	Compra_ID INT,
	Producto_ID INT,
	CantidadUnitaria INT,
	PrecioUnitario DECIMAL(12,2),
	MontoUnitario DECIMAL(10,2),
	IVA DECIMAL(12,2) DEFAULT 0, --Iva por producto
	Total DECIMAL(12,2),
	FOREIGN KEY (Compra_ID) REFERENCES Compras(Compra_ID),
	FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);

CREATE TABLE Ventas (
	Venta_ID INT PRIMARY KEY IDENTITY,
	Usuario_ID INT,
	Cliente_ID INT,
	FechaVenta DATETIME,
	CantidadTotal INT,
	MontoRecibido DECIMAL(12,2),
	MontoDevuelto DECIMAL(12,2),
	SubTotal DECIMAL(12,2),
	Descuento DECIMAL(12,2) DEFAULT 0,
	IVA DECIMAL(12,2) DEFAULT 0,
	Total DECIMAL(12,2),
	Estado VARCHAR(20) DEFAULT 'Activo' CHECK (Estado IN ('Activo','Anulado')), --En caso q la venta sea anulada
	FOREIGN KEY (Usuario_ID) REFERENCES Usuario(Usuario_ID),
	FOREIGN KEY (Cliente_ID) REFERENCES Clientes(Cliente_ID)



select * from Ventas
	-- Detalles de Ventas (por producto)
CREATE TABLE Detalles_Ventas (
	DetalleVenta_ID INT PRIMARY KEY IDENTITY,
	Venta_ID INT,
	Producto_ID INT,
	Cantidad INT,
	PrecioUnitario DECIMAL(12,2),
	SubTotal DECIMAL(12,2),
	IVA DECIMAL(12,2),
	Total DECIMAL(12,2),
	TipoComprobante VARCHAR(20) NOT NULL CHECK (TipoComprobante IN ('Factura','Ticket')), --Esto de podria quitar
	FOREIGN KEY (Venta_ID) REFERENCES Ventas(Venta_ID),
	FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);

select * from Detalles_Ventas

CREATE TABLE Factura (
    Factura_ID INT PRIMARY KEY IDENTITY,
    Venta_ID INT NOT NULL,  -- id de la venta qyeu genero esta factura
    Cliente_ID INT NULL,  -- id del cliente aunque puede ser nulo
    NumeroFactura VARCHAR(50) UNIQUE NOT NULL,
    Serie VARCHAR(10) NULL, 
    Correlativo VARCHAR(20) NULL, 
    FechaFactura DATETIME,
    FechaVencimiento DATETIME NULL, 
    SubTotal DECIMAL(12,2) NOT NULL,  -- la suma antes de los impuestos y descuentos
    IVA DECIMAL(12,2) DEFAULT 0,  --impuestos (15% en nicaragua)
    Descuento DECIMAL(12,2) DEFAULT 0, -- aplicado de manera general
    TotalFactura DECIMAL(12,2) NOT NULL, -- el total a pagar o el subtotal + iva - descuento
    Moneda VARCHAR(10) DEFAULT 'Córdoba', -- 
    MetodoPago VARCHAR(50) DEFAULT 'Efectivo',
    TipoPago VARCHAR(50) DEFAULT 'Contado', -- (Contado/Crédito) o sea pago inmediato o si la libreria fia a plazos
    Estado VARCHAR(20) DEFAULT 'Emitida' CHECK (Estado IN ('Emitida', 'Anulada', 'Pagada', 'Pendiente')),
    FOREIGN KEY (Venta_ID) REFERENCES Ventas(Venta_ID), -- una factura perteneces a una venta
    FOREIGN KEY (Cliente_ID) REFERENCES Clientes(Cliente_ID) -- hace referencia a la columna id cliente en la tabla cliente
);

CREATE TABLE Detalles_Factura (
    DetalleFactura_ID INT PRIMARY KEY IDENTITY,
    Factura_ID INT NOT NULL, -- id de la factura padre
    Venta_ID INT NOT NULL, -- el detalle de la factura especifico
    DetalleVenta_ID INT NOT NULL, -- relacion directa con el item de venta
    Producto_ID INT NOT NULL, -- el id del prodcuto facturado
    Cantidad INT NOT NULL, -- cantidad de el producto especifico de la venta realizada
    PrecioUnitario DECIMAL(12,2) NOT NULL, 
    Subtotal DECIMAL(12,2) NOT NULL,  -- cantidad + preciounitario antes de los impuestos o iva
    IVA DECIMAL(12,2) DEFAULT 0, 
    Descuento DECIMAL(12,2) DEFAULT 0, 
    Total DECIMAL(12,2) NOT NULL,  -- subtotal + iva -descuento por producto especifico
    FOREIGN KEY (Factura_ID) REFERENCES Factura(Factura_ID), 
    FOREIGN KEY (Venta_ID) REFERENCES Ventas(Venta_ID),
    FOREIGN KEY (DetalleVenta_ID) REFERENCES Detalles_Ventas(DetalleVenta_ID),
    FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);

CREATE TABLE Devoluciones (
	Devolucion_ID INT PRIMARY KEY IDENTITY,
	Venta_ID INT NOT NULL, -- id de la venta que se hara la devolucion
	FechaDevolucion DATETIME,
	CantidadDevuelta INT NOT NULL, -- la cantidad de articulos que se devolvieron en general
	SubTotalDevuelto DECIMAL(12,2) NOT NULL, -- suma de subtotales de productos devueltos
	TotalDevuelto DECIMAL(12,2), -- total a devolver o subtotal + iva
	Motivo VARCHAR(255), 
	TipoDevolucion VARCHAR(20) DEFAULT 'Parcial' CHECK (TipoDevolucion IN ('Parcial','Total')), -- parcial es algunos articulos y total toda la venta
	Activo BIT,
	FOREIGN KEY (Venta_ID) REFERENCES Ventas(Venta_ID) -- la devolucion es de una venta especifica
);

-- Detalles de Devoluciones
CREATE TABLE Detalles_Devoluciones (
    DetalleDevolucion_ID INT PRIMARY KEY IDENTITY,
    Devolucion_ID INT NOT NULL,  -- id de la devolucion padre
    DetalleVenta_ID INT NOT NULL,  --saber exactamente que articulo de la venta
    Producto_ID INT NOT NULL, 
    Cantidad INT NOT NULL,  -- cantidad de ese producto especifico que se devuelve
    PrecioUnitario DECIMAL(12,2) NOT NULL,  -- precio al que se vendio
    IVADevuelto DECIMAL(12,2),  
    SubtotalDevuelto DECIMAL(12,2) NOT NULL,  -- Cantidad * PrecioUnitario
    EstadoProducto VARCHAR(50) CHECK (EstadoProducto IN ('Bueno','Defectuoso')), --por si se puede revender o no
    FOREIGN KEY (Devolucion_ID) REFERENCES Devoluciones(Devolucion_ID),
    FOREIGN KEY (DetalleVenta_ID) REFERENCES Detalles_Ventas(DetalleVenta_ID), --relacion directa con el articulo de venta
    FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);

CREATE TABLE Bitacora (
	Bitacora_ID INT IDENTITY PRIMARY KEY,
	Usuario_ID INT,
	Tabla VARCHAR(100) NOT NULL,
	UsuarioNombre VARCHAR(100),
	Accion VARCHAR(15) NOT NULL CHECK (Accion IN ('Insert', 'Update', 'Delete', 'Login', 'Logout')),
	Descripcion VARCHAR(500) NOT NULL,
	DatosAnteriores VARCHAR(MAX),
	DatosNuevos VARCHAR(MAX),
	Fecha DATETIME DEFAULT GETDATE(),
	IPAddress VARCHAR(50),
	Aplicacion VARCHAR(150),
	FOREIGN KEY (Usuario_ID) REFERENCES Usuario(Usuario_ID)
);

--tabla de perdidas en caso de que los productos sean defectuosos en la devolucion

CREATE TABLE PerdidasInventario (
    Perdida_ID INT PRIMARY KEY IDENTITY,
    Producto_ID INT NOT NULL, -- id del produto que se perdio
    Cantidad INT NOT NULL, -- la cantidad de unidades que se perdio
    Motivo VARCHAR(100) CHECK (Motivo IN ('Defectuoso', 'Vencimiento', 'Dañado')), --aunque sea defectuoso si paso el tiempo limite del reembolso del proveedor es una perdida
    FechaRegistro DATETIME DEFAULT GETDATE(),  -- fecha que se registra la perdida
    Usuario_ID INT,
    FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID),
    FOREIGN KEY (Usuario_ID) REFERENCES Usuario(Usuario_ID)
);

--tabla de reclamos al proveedor como estrategia para minimizar perdidas en la libreria

CREATE TABLE ReclamosProveedor (
    Reclamo_ID INT PRIMARY KEY IDENTITY,
    Proveedor_ID INT NOT NULL,
    Producto_ID INT NOT NULL,  --id del producto defectuoso
    Cantidad INT NOT NULL,  --la cantidad de unidades defectuosas
    Motivo VARCHAR(100),
    Estado VARCHAR(20) DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado')),
    MontoReclamado DECIMAL(12,2), -- monto total que se esta reclamando
    MontoRecuperado DECIMAL(12,2) DEFAULT 0, --lo que devolvio el proveedor
    FechaReclamo DATETIME DEFAULT GETDATE(),
    FechaRespuesta DATETIME NULL,
    FOREIGN KEY (Proveedor_ID) REFERENCES Provedores(Proveedor_ID),
    FOREIGN KEY (Producto_ID) REFERENCES Productos(Producto_ID)
);

INSERT INTO Provedores (NombreEmpresa, Direccion, Telefono, Email)
VALUES 
('Papelería Central', 'Av. Insurgentes 123, CDMX', '555-123-4567', 'contacto@papeleriacentral.com'),
('Distribuidora Escolar S.A.', 'Calle Educación 45, Guadalajara', '333-789-6543', 'ventas@distribuidoraescolar.com'),
('Útiles y Más', 'Boulevard del Estudiante 789, Monterrey', '818-456-1122', 'info@utilesymas.mx'),
('Proveedora del Norte', 'Calle Reforma 88, Chihuahua', '614-333-9988', 'norte@proveedora.com'),
('Suministros Académicos', 'Av. Universidad 432, Puebla', '222-444-7788', 'contacto@suministrosac.com');

INSERT INTO Categoria (NombreCategoria, Descripcion)
VALUES 
('Cuadernos', 'Cuadernos escolares de diferentes tamaños y diseños'),
('Escritura', 'Artículos de escritura como lápices, plumas, marcadores'),
('Papelería General', 'Hojas, carpetas, sobres y otros insumos'),
('Arte y Dibujo', 'Materiales de arte como colores, crayones, pinceles'),
('Oficina', 'Artículos para oficina como engrapadoras, perforadoras, clips');

INSERT INTO Marca (NombreMarca)
VALUES 
('Norma'),
('Bic'),
('Pelikan'),
('Faber-Castell'),
('Kores');

INSERT INTO Productos (Marca_ID, Categoria_ID, NombreProducto)
VALUES 
(1, 1, 'Cuaderno Profesional Norma 100 hojas'),
(2, 2, 'Bolígrafo Bic Azul'),
(3, 2, 'Marcador Permanente Pelikan Negro'),
(4, 4, 'Colores de Madera Faber-Castell 12 piezas'),
(5, 3, 'Corrector Líquido Kores'),
(2, 2, 'Lápiz delgado Bic HB2'),
(3, 3, 'Carpeta tamaño carta con broche Pelikan'),
(1, 1, 'Cuaderno Norma Rayado 200 hojas'),
(4, 4, 'Crayones Faber-Castell 24 colores'),
(5, 5, 'Grapadora de oficina Kores metálica');

INSERT INTO Clientes (Nombre, Apellido)
VALUES ('Juan', 'Pérez');

INSERT INTO Clientes (Nombre, Apellido)
VALUES ('María', 'Gómez');

INSERT INTO Clientes (Nombre, Apellido)
VALUES ('Carlos', 'López');

INSERT INTO Clientes (Nombre, Apellido)
VALUES ('Ana', 'Martínez');

INSERT INTO Clientes (Nombre, Apellido)
VALUES ('Luis', 'Sanchez');


-- Eliminar los detalles de las ventas (para evitar conflicto de clave foránea)
DELETE FROM Detalles_Ventas;

-- Eliminar las ventas
DELETE FROM Ventas;

select * from Ventas;
select * from Detalles_Ventas;

select * from Productos

-- 1. Insertar la primera venta
INSERT INTO Ventas (
    Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto, 
    SubTotal, Descuento, IVA, Total
)
VALUES 
(1, 1, GETDATE(), 3, 600.00, 75.00, 500.00, 50.00, 75.00, 525.00);

DECLARE @Venta_ID1 INT;
SET @Venta_ID1 = SCOPE_IDENTITY();

-- Insertar los detalles de la primera venta
INSERT INTO Detalles_Ventas (Venta_ID, Producto_ID, Cantidad, PrecioUnitario, SubTotal, IVA, Total, TipoComprobante)
VALUES 
(@Venta_ID1, 1, 2, 100.00, 200.00, 30.00, 230.00, 'Factura'),
(@Venta_ID1, 2, 1, 50.00, 50.00, 7.50, 57.50, 'Factura');

-- 2. Insertar la segunda venta
INSERT INTO Ventas (
    Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto, 
    SubTotal, Descuento, IVA, Total
)
VALUES 
(1, 2, GETDATE(), 5, 1000.00, 150.00, 850.00, 100.00, 127.50, 977.50);

DECLARE @Venta_ID2 INT;
SET @Venta_ID2 = SCOPE_IDENTITY();

-- Insertar los detalles de la segunda venta
INSERT INTO Detalles_Ventas (Venta_ID, Producto_ID, Cantidad, PrecioUnitario, SubTotal, IVA, Total, TipoComprobante)
VALUES 
(@Venta_ID2, 3, 3, 120.00, 360.00, 54.00, 414.00, 'Factura'),
(@Venta_ID2, 4, 2, 80.00, 160.00, 24.00, 184.00, 'Factura');

-- 3. Insertar la tercera venta
INSERT INTO Ventas (
    Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto, 
    SubTotal, Descuento, IVA, Total
)
VALUES 
(1, 3, GETDATE(), 4, 800.00, 50.00, 750.00, 70.00, 105.00, 825.00);

DECLARE @Venta_ID3 INT;
SET @Venta_ID3 = SCOPE_IDENTITY();

-- Insertar los detalles de la tercera venta
INSERT INTO Detalles_Ventas (Venta_ID, Producto_ID, Cantidad, PrecioUnitario, SubTotal, IVA, Total, TipoComprobante)
VALUES 
(@Venta_ID3, 5, 1, 200.00, 200.00, 30.00, 230.00, 'Factura'),
(@Venta_ID3, 6, 2, 120.00, 240.00, 36.00, 276.00, 'Factura');

-- 4. Insertar la cuarta venta
INSERT INTO Ventas (
    Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto, 
    SubTotal, Descuento, IVA, Total
)
VALUES 
(1, 4, GETDATE(), 6, 1200.00, 100.00, 1100.00, 120.00, 165.00, 1265.00);

DECLARE @Venta_ID4 INT;
SET @Venta_ID4 = SCOPE_IDENTITY();

-- Insertar los detalles de la cuarta venta
INSERT INTO Detalles_Ventas (Venta_ID, Producto_ID, Cantidad, PrecioUnitario, SubTotal, IVA, Total, TipoComprobante)
VALUES 
(@Venta_ID4, 7, 4, 150.00, 600.00, 90.00, 690.00, 'Factura'),
(@Venta_ID4, 8, 2, 300.00, 600.00, 90.00, 690.00, 'Factura');

-- 5. Insertar la quinta venta
INSERT INTO Ventas (
    Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto, 
    SubTotal, Descuento, IVA, Total
)
VALUES 
(1, 5, GETDATE(), 7, 1400.00, 50.00, 1350.00, 150.00, 202.50, 1552.50);

DECLARE @Venta_ID5 INT;
SET @Venta_ID5 = SCOPE_IDENTITY();

-- Insertar los detalles de la quinta venta
INSERT INTO Detalles_Ventas (Venta_ID, Producto_ID, Cantidad, PrecioUnitario, SubTotal, IVA, Total, TipoComprobante)
VALUES 
(@Venta_ID5, 9, 5, 100.00, 500.00, 75.00, 575.00, 'Factura'),
(@Venta_ID5, 10, 2, 75.00, 150.00, 22.50, 172.50, 'Factura');


select * from Devoluciones
select * from Detalles_Ventas
select * from Ventas