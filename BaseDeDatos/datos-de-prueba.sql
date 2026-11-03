USE [BDD-PYME];
GO

INSERT INTO PYMES ([CUIT-PYME], Direccion, RazonSocial) VALUES
    (301234567, 'San Martin 123', 'Panificadora Del Sur SRL'),
    (302345678, 'Belgrano 456',   'Metalurgica Rafaela SA');

INSERT INTO ALYCs ([ID-ALYCS], NombreALYC) VALUES (1, 'ALYC Uno'), (2, 'ALYC Dos');

INSERT INTO BROKERS ([ID-BROKER], NombreBroker) VALUES (1, 'Broker Uno'), (2, 'Broker Dos');

INSERT INTO INSTRUMENTOS ([ID-INSTRUMENTO], TipoInstrumento) VALUES
    (1, 'Cheque de pago diferido'),
    (2, 'Caucion');

INSERT INTO SGRs ([ID-SGR], [Nombre-SGR]) VALUES (1, 'SGR Uno'), (2, 'SGR Dos');

INSERT INTO OPERACIONES
    ([ID-OPERACION], [CUIT-PYME], [ID-ALYC], [ID-BROKER], [ID-INSTRUMENTO], [ID-SGR], Estado, MontoOperacion)
VALUES
    (1, 301234567, 1, 1, 1, 1, 'Aprobada',   150000),
    (2, 302345678, 2, 2, 2, 2, 'Pendiente',  320000);
GO

SELECT * FROM VW_Operaciones_Pymes;

