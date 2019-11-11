using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class AsesoriaHorario
    {
        public int Id { get; set; }
        public int AsesoriaId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public short Dia { get; set; }

        public virtual Asesorias Asesoria { get; set; }
    }
}
