CREATE OR ALTER PROCEDURE SPReporteSemestralPorDepartamento
@parFechaInicial DATE,
@parFechaFinal DATE,
@parCarreraID INT
AS

DECLARE @EstudiantesDeLaCarrera TABLE(
	UsuarioID INT NOT NULL,
	Nombre VARCHAR(60)
)
SELECT 
	u.Nombre,
	e.NumeroDeControl
FROM Estudiantes e 
JOIN Usuarios u
	ON u.ID = e.UsuarioID
WHERE e.CarreraID = @parCarreraID

 
GO
EXEC SPReporteSemestralPorDepartamento '2018-01-20' , '2021-01-20', 10
