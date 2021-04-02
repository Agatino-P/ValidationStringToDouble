using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationStringToDouble
{
    public class MainWindowModel
    {
        public double Numero { get; private set; }
        
        public void SetNumero (double numero)
        {
            Numero = numero;

        }
        public MainWindowModel(double numero)
        {
            Numero = numero;
        }

    }
}
