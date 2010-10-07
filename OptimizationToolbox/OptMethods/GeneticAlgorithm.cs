using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class GeneticAlgorithm : abstractOptMethod
    {
        #region Fields
        private readonly int populationSize;
        public abstractGenerator initGenerator { get; set; }
        public abstractGenerator crossoverGenerator { get; set; }
        public abstractGenerator mutationGenerator { get; set; }
        public abstractSelector fitnessSelector { get; set; }
        private List<KeyValuePair<double, double[]>> population;
        #endregion

        #region Constructor
        public GeneticAlgorithm(DiscreteSpaceDescriptor discreteSpaceDescriptor, int populationSize = 100, optimize direction = optimize.minimize)
        {
            this.discreteSpaceDescriptor = discreteSpaceDescriptor;
            this.populationSize = populationSize;
            ConstraintsSolvedWithPenalties = true;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = false;
            RequiresDiscreteSpaceDescriptor = true;
            population = new List<KeyValuePair<double, double[]>>();
        }
        #endregion

        public override void Add(object function)
        {
            if (function.GetType() == typeof(SamplingGenerator))
                initGenerator = (SamplingGenerator)function;
            else if (function.GetType() == typeof(GeneticCrossoverGenerator))
                crossoverGenerator = (GeneticCrossoverGenerator)function;
            else if (function.GetType() == typeof(GeneticMutationGenerator))
                mutationGenerator = (GeneticMutationGenerator)function;
            else if (function.GetType() == typeof(abstractSelector))
                fitnessSelector = (abstractSelector)function;

            base.Add(function);
        }

        protected override double run(out double[] xStar)
        {
            var population = new List<KeyValuePair<double, double[]>>();
            /* 1. make initial population and evaluate
             *    to ensure diversity, a latin hyper cube with Hammersley could be used.*/
            initGenerator.generateCandidates(ref population, populationSize);
            evaluate(population);

            /* 2. while not converged*/
            while (notConverged(k, double.NaN, population.Select(a => a.Key).ToList(), null, population.Select(a => a.Value).ToList()))
            {
                /* 3. selection survivors*/
                fitnessSelector.selectCandidates(ref population);
                /* 4. generate remainder of population with crossover generators */
                crossoverGenerator.generateCandidates(ref population, populationSize);
                /* 5. generate modifications to all with mutation */
                mutationGenerator.generateCandidates(ref population);
                /* 6. evaluate new members of population.*/
                evaluate(population);
            }
            fStar = population.Min(c => c.Key);
            xStar = (from candidate in population
                     where (candidate.Key == fStar)
                     select candidate.Value).First();
            return fStar;
        }

        private void evaluate(List<KeyValuePair<double, double[]>> population)
        {
            foreach (var c in population)
            {
                if (double.IsNaN(c.Key))
                {
                    population.Remove(c);
                    var f = objfn.calculate(c.Value);
                    population.Add(new KeyValuePair<double, double[]>(f, c.Value));
                }
            }
        }
    }
}

