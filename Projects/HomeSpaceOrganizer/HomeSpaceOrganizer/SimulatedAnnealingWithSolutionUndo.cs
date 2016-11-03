//#define SIMANN_LOGGING

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using log4net;

// do not use simulated anneal logging when running NCrunch tests since it then produces log files
// inside NuGet package folder
#if SIMANN_LOGGING && !NCRUNCH
using LibroLib.FileSystem;
#endif

namespace HomeSpaceOrganizer
{
    // http://mathworld.wolfram.com/SimulatedAnnealing.html
    public class SimulatedAnnealingWithSolutionUndo<TSituation, TSolution>
        where TSituation : ISimulatedAnnealingSituation
        where TSolution : class, ISimulatedAnnealingSolution
    {
        public SimulatedAnnealingWithSolutionUndo(
            [NotNull] ISimulatedAnnealingProblem<TSituation, TSolution> problem,
            [NotNull] IRandomizer randomizer)
        {
            Contract.Requires(randomizer != null);

            this.problem = problem;
            this.randomizer = randomizer;
        }

        public int CalculatedInitialTemperatureMultiplier
        {
            get { return calculatedInitialTemperatureMultiplier; }
            set { calculatedInitialTemperatureMultiplier = value; }
        }

        public TSolution Run()
        {
            if (log.IsDebugEnabled)
                log.Debug("Started annealing...");
            
            searchStopwatch.Start();

#if SIMANN_LOGGING && !NCRUNCH
            using (logger = new SimulatedAnnealingLogger(new WindowsFileSystem()))
#endif
            {
#if SIMANN_LOGGING && !NCRUNCH
                ApplicationInfo appInfo = new ApplicationInfo();
                logger.Start(appInfo.GetAppDirectoryPath("sim-anneal.csv"));
#endif
                if (!problem.IsProblemValid)
                    return null;

                currentSituation = problem.GenerateInitialSituation(randomizer);
                initialSolutionValue = currentSituation.SituationValue;

                if (double.IsNaN(currentSituation.SituationValue))
                    return null;

                RememberCurrentSituationAsBestSolution();

                SimulateAnnealing();

                bestSolution.AcceptAsFinalSolution(searchStopwatch.Elapsed, runOutOfTime);

                LogSimAnnParameters();

                problem.AfterSolutionFound();
            }

            return bestSolution;
        }

        private void SimulateAnnealing()
        {
            runOutOfTime = false;
            bestSolutionCoolingStep = 0;

            if (currentSituation.IsOptimal)
                return;

            CalculateInitialTemperature();

            for (currentCoolingStep = 0;; currentCoolingStep++)
            {
                if (PerformCoolingStep())
                {
                    if (runOutOfTime)
                        log.DebugFormat("Stopped because it run out of time");
                    break;
                }
            }
        }

        /// <summary>
        /// Calculates the initial temperature of the system based on the average
        /// value difference produced by 
        /// <see cref="ISimulatedAnnealingProblem{TSituation,TSolution}.GenerateRandomTransition"/> 
        /// method.
        /// </summary>
        private void CalculateInitialTemperature()
        {
            if (log.IsDebugEnabled)
                log.Debug("Calculating the initial temperature...");

#if SIMANN_LOGGING && !NCRUNCH
            logger.Log(new object[] { "calc initial temp" });
#endif
            double previousValue = currentSituation.SituationValue;
            double totalValueDiff = 0;
            int valuesCounted = 0;

            int transitionsPerTemperature = problem.TransitionsPerTemperature;
            for (int j = 0; j < transitionsPerTemperature; j++)
            {
                bool transitionFound = problem.GenerateRandomTransition(currentSituation, randomizer);
                if (!transitionFound)
                    return;

                double newValue = currentSituation.SituationValue;

                double valueDiff = newValue - previousValue;
                if (valueDiff > 0)
                {
                    totalValueDiff += valueDiff;
                    valuesCounted++;
                }

                if (newValue > bestSolution.SolutionValue)
                    RememberCurrentSituationAsBestSolution();

                previousValue = newValue;
            }

            double averageValueDiff = totalValueDiff / valuesCounted;
            initialTemperature = temperature = 
                -averageValueDiff * calculatedInitialTemperatureMultiplier / Math.Log(1.0 / 3);

            if (log.IsDebugEnabled)
                log.DebugFormat(CultureInfo.InvariantCulture, "Average value diff = {0}", averageValueDiff);

            if (log.IsDebugEnabled)
                log.DebugFormat(CultureInfo.InvariantCulture, "Initial temperature is now = {0}", temperature);
        }

        private bool PerformCoolingStep()
        {
            transitionsAcceptedInCurrentStep = 0;

            if (searchStopwatch.Elapsed >= problem.MaxRunningTime)
            {
                runOutOfTime = true;
                return true;
            }

            temperature *= problem.CoolingFraction;

            if (log.IsDebugEnabled)
                log.DebugFormat(CultureInfo.InvariantCulture, "PerformCoolingStep (step={0}, temperature={1})", currentCoolingStep, temperature);

            coolingStepInitialValue = currentSituation.SituationValue;
            coolingStepPreviousValue = coolingStepInitialValue;

            int transitionsPerTemperature = problem.TransitionsPerTemperature;
            for (int j = 0; j < transitionsPerTemperature; j++)
            {
                if (PerformRandomTransition())
                    break;
            }

            if (currentSituation.IsOptimal)
            {
                RememberCurrentSituationAsBestSolution();
                return true;
            }

            double currentValue = currentSituation.SituationValue;

            if (currentValue > bestSolution.SolutionValue)
                RememberCurrentSituationAsBestSolution();

            double improvementSinceLastStep = currentValue - coolingStepInitialValue;

#if SIMANN_LOGGING && !NCRUNCH
            logger.Log(new object[] { "cooling step", currentCoolingStep, transitionsAcceptedInCurrentStep, improvementSinceLastStep.ToString("N8", CultureInfo.InvariantCulture), temperature.ToString("N8", CultureInfo.InvariantCulture) });
#endif

            string stopConditionDescription;
            int stepsWithoutImprovementCount = currentCoolingStep - bestSolutionCoolingStep;
            if (problem.StopEvaluationFunction(currentSituation, bestSolution, improvementSinceLastStep, currentCoolingStep, stepsWithoutImprovementCount, out stopConditionDescription))
            {
                log.Debug(stopConditionDescription);
#if SIMANN_LOGGING && !NCRUNCH
                logger.Log(new object[] { stopConditionDescription });
#endif
                return true;
            }

            return false;
        }

        private bool PerformRandomTransition()
        {
            if (currentSituation.IsOptimal)
                return true;

            bool transitionFound = problem.GenerateRandomTransition(currentSituation, randomizer);
            if (!transitionFound)
                return true;

            double transitionValue = currentSituation.SituationValue;

            if (log.IsDebugEnabled)
                log.DebugFormat(CultureInfo.InvariantCulture, "Transition value: {0}", transitionValue);

            if (transitionValue < worstSolutionValue)
                worstSolutionValue = transitionValue;

            if (transitionValue > bestSolution.SolutionValue)
                RememberCurrentSituationAsBestSolution();

            double valueDiff = transitionValue - coolingStepPreviousValue;

            if (valueDiff >= 0)
            {
                UseTransitionAsSolution();
                coolingStepPreviousValue = transitionValue;
                return false;
            }

            double exponent = valueDiff / temperature;
            double merit = Math.Exp(exponent);
            double flip = randomizer.NextDouble();
            if (merit > flip)
            {
                UseTransitionAsSolution();
                coolingStepPreviousValue = transitionValue;
                return false;
            }

            RejectTransitionAsSolution();
            return false;
        }

        private void RememberCurrentSituationAsBestSolution()
        {
#if SIMANN_LOGGING && !NCRUNCH
            bool isInitialSolution = bestSolution == null;
#endif
            bestSolution = (TSolution)currentSituation.GenerateSolutionCandidate();
            bestSolutionCoolingStep = currentCoolingStep;

            if (log.IsDebugEnabled)
                log.DebugFormat(CultureInfo.InvariantCulture, "Best transition value: {0}", bestSolution.SolutionValue);

#if SIMANN_LOGGING && !NCRUNCH
            List<object> logValues = new List<object>();
            logValues.Add(isInitialSolution ? "init" : "best");
            problem.FillLogValues(logValues, bestSolution);
            logger.Log(logValues);
#endif
            problem.OnBetterSolutionFound(bestSolution);
        }

        private void RejectTransitionAsSolution()
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("RejectTransitionAsSolution");

            currentSituation.RejectChange();
        }

        private void UseTransitionAsSolution()
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("UseTransitionAsSolution");

            currentSituation.AcceptChange();
            problem.OnNewSituationAccepted();
            transitionsAcceptedInCurrentStep++;
        }

        private void LogSimAnnParameters()
        {
            if (!log.IsDebugEnabled)
                return;

            log.DebugFormat(CultureInfo.InvariantCulture, "Finished '{0}'", problem.ProblemDescription);
            log.DebugFormat(CultureInfo.InvariantCulture, "Running time: {0}", searchStopwatch.Elapsed);
            log.DebugFormat(CultureInfo.InvariantCulture, "Run out of time: {0}", runOutOfTime);
            log.DebugFormat(CultureInfo.InvariantCulture, "Initial temperature: {0}", initialTemperature);
            log.DebugFormat(CultureInfo.InvariantCulture, "Final temperature: {0}", temperature);
            log.DebugFormat(CultureInfo.InvariantCulture, "Cooling steps executed: {0}", currentCoolingStep);
            int stepsWithoutImprovementCount = currentCoolingStep - bestSolutionCoolingStep;
            log.DebugFormat(CultureInfo.InvariantCulture, "Count of steps without improvement: {0}", stepsWithoutImprovementCount);
            log.DebugFormat(CultureInfo.InvariantCulture, "Initial solution value: {0}", initialSolutionValue);
            log.DebugFormat(CultureInfo.InvariantCulture, "Best solution value: {0}", bestSolution.SolutionValue);
            log.DebugFormat(CultureInfo.InvariantCulture, "Worst solution value: {0}", worstSolutionValue);
        }

        private readonly Stopwatch searchStopwatch = new Stopwatch();
        private TSolution bestSolution;
        private int bestSolutionCoolingStep;
        private TSituation currentSituation;
        private double initialSolutionValue;
        private double worstSolutionValue = double.MaxValue;
        private int currentCoolingStep;
        private double coolingStepInitialValue;
        private double coolingStepPreviousValue;
        private int transitionsAcceptedInCurrentStep;
        private double initialTemperature;
        private double temperature;
        private bool runOutOfTime;
        private readonly ISimulatedAnnealingProblem<TSituation, TSolution> problem;
        private readonly IRandomizer randomizer;
#if SIMANN_LOGGING && !NCRUNCH
        private ISimulatedAnnealingLogger logger;
#endif
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int calculatedInitialTemperatureMultiplier = 5;
    }
}