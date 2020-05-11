using System;
using System.Collections.Generic;

namespace AGFunciones
{
    public class Individuo 
    {
        public List<double> Variables = new List<double>();
        
        public double Aptitud;

        public string ToString()
        {
            string cadena = "Aptitud: " + Aptitud + ", Valores = {";

            for(int i = 0; i < Variables.Count; i++)
            {
                if (i == Variables.Count - 1)
                {
                    cadena = cadena + Variables[i] + "}";
                }
                else
                {
                    cadena = cadena + Variables[i] + ", ";
                }
            }

            return cadena;
        }
    }
}