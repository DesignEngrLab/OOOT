// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="MultiObjectiveGeneticAlgorithm.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*************************************************************************
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
    /// <summary>
    /// Class MultiObjectiveGeneticAlgorithm.
    /// Implements the <see cref="OptimizationToolbox.GeneticAlgorithm" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.GeneticAlgorithm" />
    public class MultiObjectiveGeneticAlgorithm : GeneticAlgorithm
    {
        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
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
                crossoverGenerator.GenerateCandidates(ref population, control: populationSize);
                /* 5. generate modifications to all with mutation */
                SearchIO.output("performing mutation (current pop = " + population.Count + ").", 4);
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

        /// <summary>
        /// Calculates the population stats.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <returns>System.Double[].</returns>
        private double[,] CalcPopulationStats(IEnumerable<ICandidate> population)
        {
            var result = new double[3, f.Count];
            for (int i = 0; i < f.Count; i++)
            {
                result[0, i] = (from c in population select c.objectives[i]).Min();
                result[1, i] = (from c in population select c.objectives[0]).Average();
                result[2, i] = (from c in population select c.objectives[0]).Max();
            };
            return result;
        }


        /// <summary>
        /// Evaluates the specified population.
        /// </summary>
        /// <param name="population">The population.</param>
        private void evaluate(IList<ICandidate> population)
        {
            for (var i = population.Count - 1; i >= 0; i--)
            {
                var candidate = population[i];
                if (candidate.objectives.Contains(double.NaN))
                {
                    population.RemoveAt(i);
                    var fVals  = calc_f_vector(candidate.x);
                    population.Add(new Candidate(fVals, candidate.x));
                }
            }
        }
    }
}