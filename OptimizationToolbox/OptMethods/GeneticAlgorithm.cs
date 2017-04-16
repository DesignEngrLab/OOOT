﻿/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class GeneticAlgorithm : abstractOptMethod
    {
        #region Fields

        protected readonly int populationSize;
        public abstractGenerator initGenerator { get; set; }
        public abstractGenerator crossoverGenerator { get; set; }
        public abstractGenerator mutationGenerator { get; set; }
        public abstractSelector fitnessSelector { get; set; }

        #endregion

        #region Constructor

        public GeneticAlgorithm(int populationSize = 100)
        {
            this.populationSize = populationSize;

            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = false;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = true;
        }

        #endregion

        public override void Add(object function)
        {
            if (function is SamplingGenerator)
                initGenerator = (SamplingGenerator)function;
            else if (function is GeneticCrossoverGenerator)
                crossoverGenerator = (GeneticCrossoverGenerator)function;
            else if (function is GeneticMutationGenerator)
                mutationGenerator = (GeneticMutationGenerator)function;
            else if (function is abstractSelector)
                fitnessSelector = (abstractSelector)function;
            else base.Add(function);
        }

        protected override double run(out double[] xStar)
        {
            var population = new List<ICandidate>();
            /* 1. make initial population and evaluate
             *    to ensure diversity, a latin hyper cube with Hammersley could be used.*/
            SearchIO.output("creating initial population", 4);
            initGenerator.GenerateCandidates(ref population, control: populationSize);
            SearchIO.output("evaluating initial population", 4);
            evaluate(population);

            do
            {
                SearchIO.output("", "", k, "iter = " + k,
                                "*******************\n* Iteration: " + k + " *\n*******************");
                /* 3. selection survivors*/
                SearchIO.output("selecting from population  (current pop = " + population.Count + ").", 4);
                SearchIO.output(CalcPopulationStats(population).MakePrintString(), 4);
                fitnessSelector.SelectCandidates(ref population);
                /* 4. generate remainder of population with crossover generators */
                SearchIO.output("generating new candidates (current pop = " + population.Count + ").", 4);
                if (crossoverGenerator != null)
                    crossoverGenerator.GenerateCandidates(ref population, control: populationSize);
                /* 5. generate modifications to all with mutation */
                SearchIO.output("performing mutation (current pop = " + population.Count + ").", 4);
                if (mutationGenerator != null)
                    mutationGenerator.GenerateCandidates(ref population);
                /* 6. evaluate new members of population.*/
                SearchIO.output("evaluating new popluation members.", 4);
                evaluate(population);
                k++;
                fStar = population.Min(c => c.objectives[0]);
                xStar = (from candidate in population
                         where (candidate.objectives[0] == fStar)
                         select candidate.x).First();
                SearchIO.output("x* = " + xStar.MakePrintString(), 4);
                SearchIO.output("f* = " + fStar, 4);
            } while (notConverged(k, numEvals, fStar, xStar, population.Select(a => a.x).ToList(),
                population.Select(a => a.objectives[0]).ToList()));
            return fStar;
        }

        private static double[] CalcPopulationStats(IEnumerable<ICandidate> population)
        {
            return new[]
                       {
                           (from c in population select c.objectives[0]).Min(),
                           (from c in population select c.objectives[0]).Average(),
                           (from c in population select c.objectives[0]).Max()
                       };
        }


        private void evaluate(IList<ICandidate> population)
        {
            for (var i = population.Count - 1; i >= 0; i--)
            {
                var candidate = population[i];
                if (double.IsNaN(candidate.objectives[0]))
                {
                    population.RemoveAt(i);
                    var f = calc_f(candidate.x);
                    population.Add(new Candidate(f, candidate.x));
                }
            }
        }
    }
}