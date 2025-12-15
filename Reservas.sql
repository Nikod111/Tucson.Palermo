CREATE DATABASE [Tucson];
GO

USE [Tucson]
GO

/****** Objeto: Table [dbo].[Reservas] Fecha del script: 13/12/2025 14:51:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reservas] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]           NVARCHAR (100) NOT NULL,
    [Email]            NVARCHAR (150) NOT NULL,
    [FechaHoraInicio]  DATETIME2 (7)  NOT NULL,
    [FechaHoraFin]     DATETIME2 (7)  NOT NULL,
    [CantidadPersonas] INT            NOT NULL,
    [Estado]           NVARCHAR (20)  NOT NULL,
    [CodigoVIP]        NVARCHAR (6)   NULL,
    [MesaPreferida]    INT            NULL,
    [EdadCumpleañero]  INT            NULL,
    [RequiereTorta]    BIT            NULL,
    [Discriminator]    NVARCHAR (100) DEFAULT ('Reserva') NOT NULL
);

-- 5 Reservas normales
INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, Discriminator)
VALUES ('Juan Pérez', 'juan@mail.com', '2025-12-20 19:00:00', '2025-12-20 21:00:00', 2, 'Pendiente', 'Reserva');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, Discriminator)
VALUES ('María López', 'maria@mail.com', '2025-12-21 20:00:00', '2025-12-21 22:00:00', 4, 'Confirmada', 'Reserva');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, Discriminator)
VALUES ('Carlos Gómez', 'carlos@mail.com', '2025-12-22 19:30:00', '2025-12-22 21:30:00', 1, 'Pendiente', 'Reserva');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, Discriminator)
VALUES ('Lucía Fernández', 'lucia@mail.com', '2025-12-23 20:00:00', '2025-12-23 22:00:00', 3, 'Confirmada', 'Reserva');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, Discriminator)
VALUES ('Pedro Martínez', 'pedro@mail.com', '2025-12-24 21:00:00', '2025-12-24 23:00:00', 2, 'Pendiente', 'Reserva');

-- 5 Reservas VIP
INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, CodigoVIP, MesaPreferida, Discriminator)
VALUES ('Ana Torres', 'ana@mail.com', '2025-12-20 12:00:00', '2025-12-20 14:00:00', 2, 'Confirmada', 'VIP001', 5, 'ReservaVIP');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, CodigoVIP, MesaPreferida, Discriminator)
VALUES ('Diego Ramírez', 'diego@mail.com', '2025-12-21 13:00:00', '2025-12-21 15:00:00', 4, 'Pendiente', 'VIP002', 3, 'ReservaVIP');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, CodigoVIP, MesaPreferida, Discriminator)
VALUES ('Sofía Herrera', 'sofia@mail.com', '2025-12-22 12:30:00', '2025-12-22 14:30:00', 1, 'Confirmada', 'VIP003', 7, 'ReservaVIP');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, CodigoVIP, MesaPreferida, Discriminator)
VALUES ('Martín Díaz', 'martin@mail.com', '2025-12-23 13:00:00', '2025-12-23 15:00:00', 3, 'Pendiente', 'VIP004', 2, 'ReservaVIP');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, CodigoVIP, MesaPreferida, Discriminator)
VALUES ('Valentina Suárez', 'valen@mail.com', '2025-12-24 12:00:00', '2025-12-24 14:00:00', 2, 'Confirmada', 'VIP005', 4, 'ReservaVIP');

-- 10 Reservas Cumpleañero
INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Mateo Rojas', 'mateo@mail.com', '2025-12-20 18:00:00', '2025-12-20 22:00:00', 8, 'Confirmada', null, 10, 1, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Camila Vega', 'camila@mail.com', '2025-12-21 17:00:00', '2025-12-21 21:00:00', 5, 'Pendiente', null, 7, 0, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Tomás Navarro', 'tomas@mail.com', '2025-12-22 18:30:00', '2025-12-22 22:30:00', 12, 'Confirmada', null, 12, 1, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Isabella Castro', 'isa@mail.com', '2025-12-23 19:00:00', '2025-12-23 23:00:00', 6, 'Pendiente', null, 9, 1, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Sebastián Morales', 'seba@mail.com', '2025-12-24 18:00:00', '2025-12-24 22:00:00', 10, 'Confirmada', null, 11, 0, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Florencia Méndez', 'flor@mail.com', '2025-12-25 17:00:00', '2025-12-25 21:00:00', 7, 'Pendiente', null, 8, 1, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Agustín Romero', 'agus@mail.com', '2025-12-26 18:00:00', '2025-12-26 22:00:00', 9, 'Confirmada', null, 10, 0, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Julieta Cabrera', 'julieta@mail.com', '2025-12-27 19:00:00', '2025-12-27 23:00:00', 11, 'Pendiente', null, 13, 1, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Nicolás Sánchez', 'nico@mail.com', '2025-12-28 18:00:00', '2025-12-28 22:00:00', 5, 'Confirmada', null, 6, 0, 'ReservaCumpleañero');

INSERT INTO Reservas (Nombre, Email, FechaHoraInicio, FechaHoraFin, CantidadPersonas, Estado, MesaPreferida, EdadCumpleañero, RequiereTorta, Discriminator)
VALUES ('Martina Silva', 'martina@mail.com', '2025-12-29 17:30:00', '2025-12-29 21:30:00', 7, 'Pendiente', null, 7, 1, 'ReservaCumpleañero');