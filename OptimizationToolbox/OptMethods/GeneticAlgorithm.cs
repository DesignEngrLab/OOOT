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
        private SortedList<double, double[]> population;
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
            population = new SortedList<double, double[]>(new optimizeSort(direction));
        }
        #endregion

        public override void  Add(object function)
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
            List<double[]> newCandidates = new List<double[]>();
            /* 1. make initial population and evaluate
             *    to ensure diversity, a latin hyper cube with Hammersley could be used.*/
            initGenerator.generateCandidates(ref newCandidates, populationSize);
            foreach (var c in newCandidates)
            {
                var f = objfn.calculate(c);
                population.Add(f, c);
            }
            var aveF = population.Keys.Average();

            /* 2. while not converged*/
            while (notConverged(k, aveF, null, null, population.Values))
            {
                /* 3. selection survivors*/
                population = fitnessSelector.selectCandidates(population);
                /* 4. generate remainder of population with crossover generators */
                crossoverGenerator.generateCandidates(ref newCandidates, populationSize - population.Count);
                /* 5. generate modifications to all with mutation */
                mutationGenerator.generateCandidates(ref newCandidates);

                /* 6. evaluate new members of population.*/
                foreach (var c in newCandidates)
                {
                    var f = objfn.calculate(c);
                    population.Add(f, c);
                }
            }
            xStar = population.Values[0];
            return population.Keys[0];
        }
    }
}

