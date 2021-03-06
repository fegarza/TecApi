﻿using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class EstudiantesDatos
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string TelefonoDomicilio { get; set; }
        public string TelefonoMovil { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string CiudadNacimiento { get; set; }
        public string EstadoNacimiento { get; set; }
        public string EstadoCivil { get; set; }
        public byte? NumeroHijos { get; set; }
        public string CalleDomicilio { get; set; }
        public string NumDomicilio { get; set; }
        public string ColoniaDomicilio { get; set; }
        public string CodigoPostalDomicilio { get; set; }
        public string CependenciaEconomica { get; set; }
        public bool? Trabaja { get; set; }
        public string Empresa { get; set; }
        public string Horario { get; set; }
        public bool? PadreVive { get; set; }
        public bool? MadreVive { get; set; }
        public string EstudiosPadre { get; set; }
        public string EstudiosMadre { get; set; }
        public string LugarTrabajoPadre { get; set; }
        public string LugarTrabajoMadre { get; set; }
        public string LugarTrabajoFamiliar { get; set; }
        public string TelefonoTrabajoPadre { get; set; }
        public string TelefonoTrabajoMadre { get; set; }
        public string TelefonoTrabajoFamiliar { get; set; }
        public bool? TieneBeca { get; set; }
        public string BecadoPor { get; set; }
        public string Nss { get; set; }
        public bool? PrescripcionVista { get; set; }
        public bool? PrescripcionOido { get; set; }
        public bool? PrescripcionLenguaje { get; set; }
        public bool? PrescripcionMotriz { get; set; }
        public bool? PrescripcionSistemaNervioso { get; set; }
        public bool? PrescripcionSistemaCirculatorio { get; set; }
        public bool? PrescripcionSistemaDigestivo { get; set; }
        public bool? PrescripcionSistemaRespiratorio { get; set; }
        public bool? PrescripcionSistemaOseo { get; set; }
        public bool? PrescripcionSistemaOtro { get; set; }
        public bool? TratamientoPsicologicoPsiquiatrico { get; set; }
        public string TratamientoPsicologicoPsiquiatricoExplicacion { get; set; }
        public byte? ManosPiesHinchados { get; set; }
        public byte? DoloresVientre { get; set; }
        public byte? DoloresCabezaVomito { get; set; }
        public byte? PerdidaEquilibrio { get; set; }
        public byte? FatigaAgotamiento { get; set; }
        public byte? PerdidaVistaOido { get; set; }
        public byte? DificultadDormir { get; set; }
        public byte? PesadillasTerroresNocturnos { get; set; }
        public string PesadillasTerroresNocturnosAque { get; set; }
        public byte? Incontinencia { get; set; }
        public byte? Tartamudeos { get; set; }
        public byte? MiedosIntensos { get; set; }
        public string ObservacionesHigiene { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public virtual Estudiantes Estudiante { get; set; }
    }
}
