USE [ConexionSolidaria]
GO

CREATE TRIGGER [dbo].[TR_INSCRIPCION_GESTION_CUPO]
ON [dbo].[Inscripcion]
AFTER INSERT, UPDATE 
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE EVT
    SET EVT.CupoDisponible = EVT.CupoDisponible - Cambios.Total
    FROM Evento EVT
    INNER JOIN (
        SELECT I.EventoID, COUNT(*) AS Total
        FROM INSERTED I
        LEFT JOIN DELETED D ON I.InscripcionID = D.InscripcionID
        WHERE I.EstadoID = 4
          AND (D.EstadoID IS NULL OR D.EstadoID <> 4)
        GROUP BY I.EventoID
    ) Cambios ON EVT.EventoID = Cambios.EventoID
    WHERE EVT.EstadoID = 1; 
    UPDATE EVT
    SET EVT.CupoDisponible = EVT.CupoDisponible + Cambios.Total
    FROM Evento EVT
    INNER JOIN (
        SELECT D.EventoID, COUNT(*) AS Total
        FROM DELETED D
        INNER JOIN INSERTED I ON D.InscripcionID = I.InscripcionID
        WHERE D.EstadoID = 4
          AND I.EstadoID <> 4
        GROUP BY D.EventoID
    ) Cambios ON EVT.EventoID = Cambios.EventoID
    WHERE EVT.EstadoID = 1;

END;
GO

ALTER TABLE [dbo].[Inscripcion] ENABLE TRIGGER [TR_INSCRIPCION_GESTION_CUPO]
GO


