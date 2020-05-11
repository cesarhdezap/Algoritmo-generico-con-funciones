using System;
using System.Collections.Generic;
using System.Linq;

namespace AGFunciones
{
    public class AlgoritmoGenetico
    {
        private readonly int MAXIMO_ITERACIONES = 100;
        private int NUMERO_DE_VARIABLES = 10;
        private readonly int TAMAÑO_POBLACION = 100;
        private double RANGO_INFERIOR = -5.12;
        private double RANGO_SUPERIOR = 5.12;
        private readonly int PADRES_POR_TORNEO = 10;
        private readonly double PORCENTAJE_DE_MUTACION = 0.5;
        private readonly int MAXIMO_DECIMALES = 3;
        private readonly List<int> ALFA = new List<int>() { 0, 1 };
        private List<Individuo> Poblacion = new List<Individuo>();

        public List<Individuo> MejorIndividuoPorIteracion = new List<Individuo>();
        public int NumeroDeIteraciones;
        public int NumeroDeEvaluaciones;


        public void Iniciar()
        {
            GenerarPoblacionInicial();
            while (NumeroDeIteraciones < MAXIMO_ITERACIONES)
            {
                ReemplazarPoblacion();
                NumeroDeIteraciones++;
                MejorIndividuoPorIteracion.Add(ObtenerMejorIndividuo());
            }

        }

        public void IniciarProblema2(double rangoInferior, double rangoSuperior, int numeroDeVariables)
        {
            RANGO_INFERIOR = rangoInferior;
            RANGO_SUPERIOR = rangoSuperior;
            NUMERO_DE_VARIABLES = numeroDeVariables;
            GenerarPoblacionInicial(true);

            while (NumeroDeIteraciones < MAXIMO_ITERACIONES)
            {
                ReemplazarPoblacion();
                NumeroDeIteraciones++;
                MejorIndividuoPorIteracion.Add(ObtenerMejorIndividuo());
            }
            
        }

        private void ReemplazarPoblacion()
        {
            List<Individuo> nuevaPoblacion = new List<Individuo>();
            nuevaPoblacion.Add(ObtenerMejorIndividuo());

            while (nuevaPoblacion.Count < TAMAÑO_POBLACION)
            {
                Individuo padre1 = SeleccionarPadrePorTorneo();
                Individuo padre2 = SeleccionarPadrePorTorneo();
                List<Individuo> hijos = CruzarPadres(padre1, padre2);
                hijos = MutarHijosUniforme(hijos);
                nuevaPoblacion = nuevaPoblacion.Concat(hijos).ToList();
            }

            Poblacion = nuevaPoblacion;
        }

        public Individuo ObtenerMejorIndividuo()
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < Poblacion.Count; i++)
            {
                indices.Add(i);
            }

            indices = indices.OrderBy(i => Poblacion[i].Aptitud).ToList();

            return Poblacion[indices.First()];
        }

        public List<Individuo> MutarHijosUniforme(List<Individuo> hijos)
        {
            foreach (Individuo individuo in hijos)
            {
                double probabilidad = new Random().NextDouble();
                if (probabilidad >= PORCENTAJE_DE_MUTACION)
                {

                    for (int i = 0; i < individuo.Variables.Count; i++)
                    {
                        double distribucionNormal = ObtenerDistribucionNormal(individuo.Variables, individuo.Variables[i]);
                        individuo.Variables[i] += distribucionNormal;
                        individuo.Variables[i] = Math.Round(individuo.Variables[i], MAXIMO_DECIMALES);

                        if (individuo.Variables[i] < RANGO_INFERIOR)
                        {
                            individuo.Variables[i] = RANGO_INFERIOR;
                        }
                        if (individuo.Variables[i] > RANGO_SUPERIOR)
                        {
                            individuo.Variables[i] = RANGO_SUPERIOR;
                        }
                    }
                }
            }

            return hijos;
        }

        private double ObtenerDesviacionEstandar(List<double> valores)
        {
            double promedio = valores.Average();
            double sumaDeDerivacion = 0;
            foreach (double valor in valores)
            {
                sumaDeDerivacion += (valor) * (valor);
            }
            double sumaDerivacionPromedio = sumaDeDerivacion / (valores.Count - 1);
            return Math.Sqrt(sumaDerivacionPromedio - (promedio * promedio));
        }

        public double ObtenerDistribucionNormal(List<double> valores, double valor)
        {
            double distribucion;
            double desEst = ObtenerDesviacionEstandar(valores);
            double promedio = valores.Average();

            distribucion = (1 / (desEst * Math.Sqrt(2 * Math.PI))) * Math.Exp( - (Math.Pow((valor - promedio), 2) / (2 * Math.Pow(desEst, 2)) ) ) ;

            return distribucion;
        }



        public List<Individuo> CruzarPadres(Individuo padre1, Individuo padre2, bool esProblema2 = false)
        {
            List<Individuo> hijos = new List<Individuo>();

            int NUMERO_DE_HIJOS = 2;
            for (int indiceHijo = 0; indiceHijo < NUMERO_DE_HIJOS; indiceHijo++)
            {
                Individuo hijo = new Individuo();
                for (int i = 0; i < NUMERO_DE_VARIABLES; i++)
                {
                    Random random = new Random();
                    double min = Math.Min(padre1.Variables[i], padre2.Variables[i]);
                    double max = Math.Max(padre1.Variables[i], padre2.Variables[i]);
                    double I = max - min;

                    int indiceAleatorioAlfa = random.Next(ALFA.Count);

                    double limiteInferior = min - (ALFA[indiceAleatorioAlfa] * I);
                    double limiteSuperior = max + (ALFA[indiceAleatorioAlfa] * I);

                    if (limiteSuperior > RANGO_SUPERIOR)
                    {
                        limiteSuperior = RANGO_SUPERIOR;
                    }
                    if (limiteInferior < RANGO_INFERIOR)
                    {
                        limiteInferior = RANGO_INFERIOR;
                    }

                    double resultado = random.NextDouble() * (limiteInferior - (limiteSuperior)) + (limiteSuperior);
                    resultado = Math.Round(resultado, 3);

                    hijo.Variables.Add(resultado);
                }

                if (esProblema2)
                {
                    hijo.Aptitud = FuncionDeAptitudProblema2(hijo);
                    ValidarRestriccionesProblema2(hijo);
                }
                else
                {
                    hijo.Aptitud = FuncionDeAptitud(hijo);
                }
                hijos.Add(hijo);
            }

            return hijos;
        }


        public Individuo SeleccionarPadrePorTorneo()
        {
            List<int> indicesPadres = new List<int>();
            for (int i = 0; i < PADRES_POR_TORNEO; i++)
            {
                int padreEscogido = new Random().Next(TAMAÑO_POBLACION);
                indicesPadres.Add(padreEscogido);
            }

            indicesPadres = indicesPadres.OrderBy(i => Poblacion[i].Aptitud).ToList();


            int padreElegido = -1;
            foreach (int i in indicesPadres)
            {
                Random aleatorio = new Random();
                double probabilidad = aleatorio.NextDouble() * (0 - (1)) + (1);
                if (probabilidad >= 0.5)
                {
                    padreElegido = i;
                    break;
                }
            }
            if (padreElegido == -1)
            {
                padreElegido = indicesPadres.Last();
            }

            return Poblacion[padreElegido];
        }

        private double FuncionDeAptitud(Individuo individuo)
        {
            NumeroDeEvaluaciones++;
            double aptitud;

            aptitud = 10 * NUMERO_DE_VARIABLES;

            for (int i = 0; i < NUMERO_DE_VARIABLES; i++)
            {
                double x = individuo.Variables[i];
                double resultado = Math.Pow(x, 2) - 10 * Math.Cos(2 * Math.PI * x);
                aptitud = aptitud + resultado;
            }
            aptitud = Math.Round(aptitud, MAXIMO_DECIMALES);
            return aptitud;
        }

        private double FuncionDeAptitudProblema2(Individuo individuo)
        {
            NumeroDeEvaluaciones++;
            double aptitud;
            double x1 = individuo.Variables[0];
            double x2 = individuo.Variables[1];
            double x3 = individuo.Variables[2];
            double x4 = individuo.Variables[3];
            double x5 = individuo.Variables[4];
            double x6 = individuo.Variables[5];
            double x7 = individuo.Variables[6];


            aptitud = Math.Pow((x1 - 10), 2) + (5 * Math.Pow((x2 - 12), 2)) + (3 * Math.Pow((x4 - 11), 2)) + (10 * Math.Pow(x5, 6)) + (7 * Math.Pow(x6, 2)) + Math.Pow(x7, 4) - (4 * x6 * x7) - (10 * x6) - (8 * x7);

            aptitud = Math.Round(aptitud, MAXIMO_DECIMALES);
            return aptitud;
        }

        private void ValidarRestriccionesProblema2(Individuo individuo)
        {
            double x1 = individuo.Variables[0];
            double x2 = individuo.Variables[1];
            double x3 = individuo.Variables[2];
            double x4 = individuo.Variables[3];
            double x5 = individuo.Variables[4];
            double x6 = individuo.Variables[5];
            double x7 = individuo.Variables[6];

            bool g1 = -127 + (2 * (Math.Pow(x1,2))) + (3 * (Math.Pow(x2,4))) + x3 + (4* (Math.Pow(x4,2))) + (5 * x5) <= 0;
            bool g2 = -282 + (7 * x1) + (3 * x2) + (10 * (Math.Pow(x3,2))) + x4 - x5  <= 0;
            bool g3 = -196 + (23 * x1) + (Math.Pow(x2, 2)) + (6 * (Math.Pow(x6,2))) - (8 * x7)  <= 0;
            bool g4 = (4 * Math.Pow(x1, 2)) + Math.Pow(x2, 2) - (3 * x1 * x2) + (2 * Math.Pow(x3, 2)) + (5 * x6) - (11 * x7) <= 0;
            // Cuando una restriccion no pasa se tiene que

            for(int i = 0; i < individuo.Variables.Count; i++)
            {
                if(g1 || g2 || g3 || g4)
                {
                    individuo.Variables[i] = RANGO_SUPERIOR;
                }
            }
        }

        public void GenerarPoblacionInicial(bool esProblema2 = false)
        {
            for (int i = 0; i < TAMAÑO_POBLACION; i++)
            {
                Individuo individuo = new Individuo();
                for (int j = 0; j < NUMERO_DE_VARIABLES; j++)
                {
                    individuo.Variables.Add(ObtenerXAleatorio());
                }
                if(esProblema2)
                {
                    individuo.Aptitud = FuncionDeAptitudProblema2(individuo);
                    ValidarRestriccionesProblema2(individuo);
                }
                else
                {
                    individuo.Aptitud = FuncionDeAptitud(individuo);
                }
                Poblacion.Add(individuo);
            }
        }

        private double ObtenerXAleatorio()
        {
            double X = 0.0;
            Random aleatorio = new Random();

            X = aleatorio.NextDouble() * (RANGO_INFERIOR - (RANGO_SUPERIOR)) + (RANGO_SUPERIOR);
            X = Math.Round(X, MAXIMO_DECIMALES);
            return X;
        }
    }
}