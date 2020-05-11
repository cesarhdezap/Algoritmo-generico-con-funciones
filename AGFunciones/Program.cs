using System;
using System.Linq;
using System.Collections.Generic;

namespace AGFunciones
{
    class Program
    {
        static void Main(string[] args)
        {
            int TOTAL_ALGORIMOS = 30;
            List<int> Evaluaciones = new List<int>();
            List<int> Iteraciones = new List<int>();
            List<Individuo> Mejor = new List<Individuo>();

            for(int i = 0; i < TOTAL_ALGORIMOS; i++)
            {
                AlgoritmoGenetico algoritmo = new AlgoritmoGenetico();
                algoritmo.Iniciar();
                Evaluaciones.Add(algoritmo.NumeroDeEvaluaciones);
                Iteraciones.Add(algoritmo.NumeroDeIteraciones);
                Mejor.Add(algoritmo.ObtenerMejorIndividuo());
            }

            

            for(int i = 0; i < TOTAL_ALGORIMOS; i++)
            {
                Console.WriteLine("ALGORITMO {0}: Iteraciones = {1} | Evaluaciones = {2} | Mejor {3}", i, Iteraciones[i], Evaluaciones[i], Mejor[i].ToString());
            }

            Evaluaciones.Clear();
            Iteraciones.Clear();
            Mejor.Clear();

            for(int i = 0; i < TOTAL_ALGORIMOS; i++)
            {
                AlgoritmoGenetico algoritmo = new AlgoritmoGenetico();
                algoritmo.IniciarProblema2(-10, 10, 7);
                Evaluaciones.Add(algoritmo.NumeroDeEvaluaciones);
                Iteraciones.Add(algoritmo.NumeroDeIteraciones);
                Mejor.Add(algoritmo.ObtenerMejorIndividuo());
            }

            for(int i = 0; i < TOTAL_ALGORIMOS; i++)
            {
                Console.WriteLine("ALGORITMO {0}: Iteraciones = {1} | Evaluaciones = {2} | Mejor {3}", i, Iteraciones[i], Evaluaciones[i], Mejor[i].ToString());
            }
        }
    }
}
