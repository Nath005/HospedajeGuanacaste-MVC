
USE master;
GO

IF DB_ID(N'HospedajeGuanacasteDatos') IS NULL
BEGIN
    CREATE DATABASE HospedajeGuanacasteDatos;
END
GO

USE HospedajeGuanacasteDatos;
GO


IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes(
        Identificacion      NVARCHAR(50) NOT NULL,
        TipoIdentificacion  NVARCHAR(30) NOT NULL,
        Nombre              NVARCHAR(50) NOT NULL,
        PrimerApellido      NVARCHAR(75) NOT NULL,
        SegundoApellido     NVARCHAR(75) NOT NULL,
        FechaNacimiento     DATE NOT NULL,

        CONSTRAINT PK_Clientes PRIMARY KEY (Identificacion),
        CONSTRAINT CK_Clientes_Tipo CHECK (TipoIdentificacion IN ('Cedula', 'DIMEX', 'Pasaporte')),
        CONSTRAINT CK_Clientes_Nombre CHECK (LEN(Nombre) BETWEEN 3 AND 50),
        CONSTRAINT CK_Clientes_PrimerApellido CHECK (LEN(PrimerApellido) BETWEEN 3 AND 75),
        CONSTRAINT CK_Clientes_SegundoApellido CHECK (LEN(SegundoApellido) BETWEEN 3 AND 75)
    );
END
GO


IF OBJECT_ID(N'dbo.Empleados', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Empleados(
        Identificacion      NVARCHAR(50) NOT NULL,
        TipoIdentificacion  NVARCHAR(30) NOT NULL,
        Nombre              NVARCHAR(50) NOT NULL,
        PrimerApellido      NVARCHAR(75) NOT NULL,
        SegundoApellido     NVARCHAR(75) NOT NULL,
        FechaNacimiento     DATE NOT NULL,
        SalarioMensual      DECIMAL(12,2) NOT NULL,
        FechaIngreso        DATE NOT NULL,
        Categoria           NVARCHAR(50) NOT NULL,
        Provincia           NVARCHAR(50) NOT NULL,
        Canton              NVARCHAR(50) NOT NULL,
        Distrito            NVARCHAR(50) NOT NULL,
        DireccionExacta     NVARCHAR(150) NOT NULL,

        CONSTRAINT PK_Empleados PRIMARY KEY (Identificacion),
        CONSTRAINT CK_Empleados_Tipo CHECK (TipoIdentificacion IN ('Cedula', 'DIMEX')),
        CONSTRAINT CK_Empleados_Nombre CHECK (LEN(Nombre) BETWEEN 3 AND 50),
        CONSTRAINT CK_Empleados_PrimerApellido CHECK (LEN(PrimerApellido) BETWEEN 3 AND 75),
        CONSTRAINT CK_Empleados_SegundoApellido CHECK (LEN(SegundoApellido) BETWEEN 3 AND 75),
        CONSTRAINT CK_Empleados_SalarioMensual CHECK (SalarioMensual >= 0 AND SalarioMensual <= 5000000),
        CONSTRAINT CK_Empleados_Direccion CHECK (LEN(DireccionExacta) BETWEEN 1 AND 150)
    );
END
GO

IF OBJECT_ID(N'dbo.Habitaciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Habitaciones(
        NumHabitacion           INT NOT NULL,
        TipoHabitacion          NVARCHAR(50) NOT NULL,
        TarifaNoche             DECIMAL(12,2) NOT NULL,
        TvSatelital             BIT NOT NULL,
        PendienteMantenimiento  NVARCHAR(500) NULL,

        CONSTRAINT PK_Habitaciones PRIMARY KEY (NumHabitacion),
        CONSTRAINT CK_Habitaciones_Numero CHECK (NumHabitacion BETWEEN 1 AND 500),
        CONSTRAINT CK_Habitaciones_Tipo CHECK (TipoHabitacion IN ('Start Junior', 'Start Vista al Mar', 'Master Start')),
        CONSTRAINT CK_Habitaciones_TarifaNoche CHECK (TarifaNoche BETWEEN 50 AND 800)
    );
END
GO

IF OBJECT_ID(N'dbo.Reservaciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reservaciones(
        CodReservacion          NVARCHAR(20) NOT NULL,
        IdentificacionCliente   NVARCHAR(50) NOT NULL,
        NumHabitacion           INT NOT NULL,
        FechaReservacion        DATE NOT NULL,
        FechaIngreso            DATE NOT NULL,
        FechaSalida             DATE NOT NULL,
        TarifaReservacion       DECIMAL(12,2) NOT NULL,
        SolicitudesEspeciales   NVARCHAR(300) NULL,
        PorcentajeDescuento     DECIMAL(5,2) NOT NULL,
        MontoTotal              DECIMAL(12,2) NOT NULL,
        Estado                  NVARCHAR(30) NOT NULL,

        CONSTRAINT PK_Reservaciones PRIMARY KEY (CodReservacion),
        CONSTRAINT FK_Reservaciones_Clientes FOREIGN KEY (IdentificacionCliente) REFERENCES dbo.Clientes(Identificacion),
        CONSTRAINT FK_Reservaciones_Habitaciones FOREIGN KEY (NumHabitacion) REFERENCES dbo.Habitaciones(NumHabitacion),
        CONSTRAINT CK_Reservaciones_Fechas CHECK (FechaSalida > FechaIngreso),
        CONSTRAINT CK_Reservaciones_Tarifa CHECK (TarifaReservacion >= 0),
        CONSTRAINT CK_Reservaciones_Descuento CHECK (PorcentajeDescuento >= 0 AND PorcentajeDescuento <= 100),
        CONSTRAINT CK_Reservaciones_MontoTotal CHECK (MontoTotal >= 0),
        CONSTRAINT CK_Reservaciones_Estado CHECK (Estado IN ('Reservada', 'Cancelada', 'En proceso', 'Finalizada'))
    );

    CREATE INDEX IX_Reservaciones_Habitacion_Fechas ON dbo.Reservaciones (NumHabitacion, FechaIngreso, FechaSalida);
    CREATE INDEX IX_Reservaciones_Cliente ON dbo.Reservaciones (IdentificacionCliente);
    CREATE INDEX IX_Reservaciones_FechaIngreso ON dbo.Reservaciones (FechaIngreso);
END
GO
