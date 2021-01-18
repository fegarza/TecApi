CREATE OR ALTER PROCEDURE SPReportePorDepartamento
@parFechaInicial DATE,
@parFechaFinal DATE,
@parDepartamentoID TINYINT
AS
 
DECLARE @Reporte TABLE (
	GrupoID INT,
	PersonalID INT PRIMARY KEY ,
	PersonalNombre VARCHAR(250),
	SesionesGrupales SMALLINT DEFAULT(0),
	SesionesIndividuales SMALLINT DEFAULT(0),
	AreasDeCanalizacion VARCHAR(500) DEFAULT('')
 );

INSERT INTO @Reporte 
(GrupoID, PersonalID, PersonalNombre)
SELECT g.ID, p.ID, CONCAT(u.Nombre, ' ', u.ApellidoPaterno, ' ' ,u.ApellidoMaterno) 
FROM Grupos g
JOIN Personales p
	ON p.ID = g.PersonalID
JOIN Usuarios u
	ON u.ID = p.usuarioID
WHERE p.DepartamentoID = @parDepartamentoID;

DECLARE @SesionesGrupales TABLE (
	GrupoID INT,
	Totales INT
);
DECLARE @SesionesIndividuales TABLE (
	GrupoID INT,
	Totales INT
);
DECLARE @EstudiantesPorGrupo TABLE (
	GrupoID INT,
	Semestre INT,
	Genero CHAR(1)
);
DECLARE @CanalizacionesPorPersonal TABLE(
	GrupoID INT,
	Area VARCHAR(200),
	Cantidad SMALLINT
);
DECLARE @CanalizacionesPorPersonalAgrupado TABLE(
	GrupoID INT,
	Area VARCHAR(900) 
	 
);
INSERT INTO @CanalizacionesPorPersonal (GrupoID, Area, Cantidad)
SELECT r.GrupoID, are.Titulo, COUNT(are.Titulo) FROM @Reporte r
	JOIN Canalizaciones c
		ON c.PersonalID = r.PersonalID
	JOIN Atenciones ate
		ON ate.ID = c.AtencionID
	JOIN Areas are
		ON are.ID = ate.AreaID
	WHERE  c.Fecha BETWEEN @parFechaInicial AND @parFechaFinal
	GROUP BY r.GrupoID, are.Titulo;

INSERT INTO @CanalizacionesPorPersonalAgrupado (GrupoID, Area)
SELECT  
	cpp.GrupoID,
	string_agg(CONCAT(cpp.Area ,' (', CAST(cpp.Cantidad AS VARCHAR(MAX)) ,') '), ', ')
  FROM @CanalizacionesPorPersonal cpp
  GROUP BY cpp.GrupoID
 

INSERT INTO @SesionesGrupales (GrupoID, Totales)
SELECT r.GrupoID, COUNT(es.EstudianteID) FROM @Reporte r 
JOIN EstudiantesSesiones es 
	ON es.GrupoID = r.GrupoID
JOIN  Sesiones s
	ON s.ID = es.SesionID
WHERE 
	s.Fecha BETWEEN @parFechaInicial AND @parFechaFinal   
	GROUP BY r.GrupoID;

INSERT INTO @SesionesIndividuales (GrupoID, Totales)
SELECT r.GrupoID, COUNT(es.EstudianteID) FROM @Reporte r 
JOIN EstudiantesSesionesIndividuales es 
	ON es.GrupoID = r.GrupoID
JOIN  SesionesIndividuales s
	ON s.ID = es.SesionIndividualID
WHERE 
	 s.Fecha BETWEEN @parFechaInicial AND @parFechaFinal   
	GROUP BY r.GrupoID;

INSERT INTO @EstudiantesPorGrupo  (GrupoID, Semestre, Genero)
	SELECT  r.GrupoID, e.Semestre, u.Genero 
	FROM @Reporte r 
	JOIN Estudiantes e
		ON e.GrupoID = r.GrupoID
	JOIN  Usuarios u
		ON u.ID = e.UsuarioID;

UPDATE r
SET r.SesionesGrupales = sg.Totales  
FROM @Reporte r
JOIN @SesionesGrupales sg
	ON sg.GrupoID = r.GrupoID;

UPDATE r
SET r.SesionesGrupales = si.Totales  
FROM @Reporte r
JOIN @SesionesIndividuales si
	ON si.GrupoID = r.GrupoID; 

UPDATE r
SET r.AreasDeCanalizacion = cppa.Area
FROM @Reporte r
JOIN @CanalizacionesPorPersonalAgrupado cppa
	ON cppa.GrupoID = r.GrupoID; 

SELECT 
 	r.PersonalNombre AS Tutor,
 	r.SesionesGrupales  AS SesionesGrupales,
	r.SesionesIndividuales AS SesionesIndividuales,
	(SELECT COUNT(*) FROM @EstudiantesPorGrupo eg WHERE r.GrupoID = eg.GrupoID)   AS Estudiantes,
	(SELECT COUNT(*) FROM @EstudiantesPorGrupo eg WHERE r.GrupoID = eg.GrupoID AND (eg.Semestre = 1 OR eg.Semestre = 2) AND eg.Genero = 'H')  AS EstudiantesPrimeroYSegundoHombres,
	(SELECT COUNT(*) FROM @EstudiantesPorGrupo eg WHERE r.GrupoID = eg.GrupoID AND (eg.Semestre = 1 OR eg.Semestre = 2) AND eg.Genero = 'M') AS EstudiantesPrimeroYSegundoMujeres,
	(SELECT COUNT(*) FROM @EstudiantesPorGrupo eg WHERE r.GrupoID = eg.GrupoID AND (eg.Semestre <> 1 OR eg.Semestre <> 2) AND eg.Genero = 'H')AS EstudiantesMayoresHombres,
	(SELECT COUNT(*) FROM @EstudiantesPorGrupo eg WHERE r.GrupoID = eg.GrupoID AND (eg.Semestre <> 1 OR eg.Semestre <> 2) AND eg.Genero = 'M') AS EstudiantesMayoresMujeres,
	r.AreasDeCanalizacion AS AreasDeCanalizacion
FROM @Reporte r;
GO
EXEC SPReportePorDepartamento '2018-01-20' , '2021-01-20', 1

 