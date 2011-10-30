using System;

namespace GisExperiments.ContoursLabeling
{
    public abstract class SimulatedAnnealing
    {
        public void Run()
        {
            InitializeAlgorithm();

            currentValue = InitializeSolution ();

            for (int i = 0; i < coolingSteps; i++)
            {
                temperature *= coolingFraction;

                startValue = currentValue;

                for (int j = 0; j < stepsPerTemperature; j++)
                {
                    transitionValue = GenerateRandomTransition();
                    double valueDiff = startValue - transitionValue;

                    if (valueDiff > 0)
                        UseTransitionAsSolution ();
                    else
                    {
                        double exponent = valueDiff/(k*temperature);
                        double merit = Math.Exp(exponent);
                        double flip = rnd.NextDouble();
                        if (merit > flip)
                            UseTransitionAsSolution ();
                    }
                }

                // restore temperature if progress has been made
                if (currentValue - startValue < 0)
                    temperature = temperature/coolingFraction;
            }
        }

        private void InitializeAlgorithm()
        {
            temperature = initialTemperature;
        }

        protected SimulatedAnnealing(
            double initialTemperature, 
            double coolingFraction, 
            double k, 
            int coolingSteps, 
            decimal stepsPerTemperature)
        {
            this.initialTemperature = initialTemperature;
            this.coolingFraction = coolingFraction;
            this.coolingSteps = coolingSteps;
            this.k = k;
            this.stepsPerTemperature = stepsPerTemperature;
        }

        public Random Rnd
        {
            get { return rnd; }
            set { rnd = value; }
        }

        protected abstract double GenerateRandomTransition();
        protected abstract double InitializeSolution();
        protected abstract void UseTransitionAsSolution();

        private double coolingFraction;
        private int coolingSteps;
        private double currentValue;
        private double initialTemperature;
        private double k;
        private Random rnd;
        private double startValue;
        private decimal stepsPerTemperature;
        private double temperature;
        private double transitionValue;
    }
}