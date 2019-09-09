using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    /// <summary>
    /// Clase que se utiliza para  validar que las transacciones de dos bases sean iguales
    /// </summary>
    public class Conteo
    {
        public String nombreTabla { get; set; }
        public int conteoPrimeraBase { get; set; }
        public int conteoSegundaBase { get; set; }
        public int conteoDiferencia { get; set; }
        public String error { get; set; }

        public Conteo()
        {
            nombreTabla = String.Empty;
            conteoPrimeraBase = 0;
            conteoSegundaBase = 0;
            error = "Sin errores en la validación";
        }
    }
}
