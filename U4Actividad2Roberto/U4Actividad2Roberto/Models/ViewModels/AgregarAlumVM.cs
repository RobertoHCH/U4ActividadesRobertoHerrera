using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace U4Actividad2Roberto.Models.ViewModels
{
    public class AgregarAlumVM
    {
        public Alumno Alumno { get; set; }
        public Docente Docente { get; set; }
        public IEnumerable<Docente> lstDocentes { get; set; }
    }
}
