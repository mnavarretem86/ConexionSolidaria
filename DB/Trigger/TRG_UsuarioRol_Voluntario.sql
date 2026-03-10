USE [ConexionSolidaria]
GO

CREATE TRIGGER [dbo].[TRG_UsuarioRol_Voluntario]
ON [dbo].[UsuarioRol]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RolVoluntario INT = 1;
    DECLARE @EstadoActivo INT = 4;
    DECLARE @EstadoInactivo INT = 7;

    INSERT INTO Voluntario (UsuarioID, Habilidades, HorasAcumuladas, EstadoID)
    SELECT  
        i.UsuarioID,
        NULL, 
        0,    
        @EstadoActivo 
    FROM inserted i
    WHERE i.RolID = @RolVoluntario
      AND NOT EXISTS (
          SELECT 1 FROM Voluntario v WHERE v.UsuarioID = i.UsuarioID
      );

    UPDATE v
    SET EstadoID = @EstadoActivo
    FROM Voluntario v
    INNER JOIN inserted i ON v.UsuarioID = i.UsuarioID
    WHERE i.RolID = @RolVoluntario
      AND v.EstadoID <> @EstadoActivo; -- Solo si no está activo ya

    ;WITH VoluntariosQuitados AS (

        SELECT d.UsuarioID
        FROM deleted d
        WHERE d.RolID = @RolVoluntario
          AND NOT EXISTS (
              SELECT 1
              FROM inserted i
              WHERE i.UsuarioID = d.UsuarioID
                AND i.RolID = @RolVoluntario
          )
    )
    UPDATE v
    SET EstadoID = @EstadoInactivo
    FROM Voluntario v
    INNER JOIN VoluntariosQuitados vq ON v.UsuarioID = vq.UsuarioID;
END;
GO

ALTER TABLE [dbo].[UsuarioRol] ENABLE TRIGGER [TRG_UsuarioRol_Voluntario]
GO