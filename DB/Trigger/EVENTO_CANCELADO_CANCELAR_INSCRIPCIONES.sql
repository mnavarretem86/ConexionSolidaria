USE [ConexionSolidaria]
GO

CREATE TRIGGER [dbo].[TR_EVENTO_CANCELADO_CANCELAR_INSCRIPCIONES]
ON [dbo].[Evento]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(EstadoID)
    BEGIN
        UPDATE INS
        SET 
            INS.EstadoID = 6,      
            INS.Estado   = 'Cancelada por evento'
        FROM [dbo].[Inscripcion] INS
        INNER JOIN INSERTED EVT ON INS.EventoID = EVT.EventoID
        INNER JOIN DELETED OLD ON EVT.EventoID = OLD.EventoID
        WHERE EVT.EstadoID = 3 
          AND OLD.EstadoID <> 3
          AND INS.EstadoID = 4;
        UPDATE EVT_BASE
        SET EVT_BASE.CupoDisponible = I.CupoMaximo
        FROM [dbo].[Evento] EVT_BASE
        INNER JOIN INSERTED I ON EVT_BASE.EventoID = I.EventoID
        INNER JOIN DELETED D ON I.EventoID = D.EventoID
        WHERE I.EstadoID = 3
          AND D.EstadoID <> 3;
    END
END;
GO

ALTER TABLE [dbo].[Evento] ENABLE TRIGGER [TR_EVENTO_CANCELADO_CANCELAR_INSCRIPCIONES]
GO


