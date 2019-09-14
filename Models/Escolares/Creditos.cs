using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Creditos
    {
        public int CreditoId { get; set; }
        public string NumeroDeControl { get; set; }
        public DateTime FechaDeOficio { get; set; }
        public int ActividadId { get; set; }
        public int Periodo { get; set; }
        public int ResponsableId { get; set; }
        public int JefeId { get; set; }
        public byte CarreraId { get; set; }
        public byte? Pregunta1 { get; set; }
        public byte? Pregunta2 { get; set; }
        public byte? Pregunta3 { get; set; }
        public byte? Pregunta4 { get; set; }
        public byte? Pregunta5 { get; set; }
        public byte? Pregunta6 { get; set; }
        public byte? Pregunta7 { get; set; }
        public double? Promedio { get; set; }
        public string EstadoDeLaActividad { get; set; }
        public string EstadoDeLaFirma { get; set; }
        public int? GrupoId { get; set; }

        public virtual Actividades Actividad { get; set; }
    }
}
