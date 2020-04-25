using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizadorDeLexico.Models
{
    public class AutomataViewModel
    {
        public List<string> Conjuntos { get; set; }
        public List<string> EstadosAceptacion { get; set; }
        public List<string> NoTerminales { get; set; }
        public List<List<string>> NombreConjuntos { get; set; }
        public List<List<string>> NoTerminalesConjunto { get; set; }
    }
}