/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MultiObjectiveGeneticAlgorithm : GeneticAlgorithm
    {
        protected override double run(out double[] xStar)
        {
            var population = new List<Candidate>();
            /* 1. make initial population and evaluate
             *    to ensure diversity, a latin hyper cube with Hammersley could be used.*/
            SearchIO.output("creating initial population", 4);
            initGenerator.GenerateCandidates(ref population, populationSize);
            SearchIO.output("evaluating initial population", 4);
            evaluate(population);

            do
            {
                SearchIO.output("", "", k, "iter = " + k,
                                "*******************\n* Iteration: " + k + " *\n*******************");
                /* 3. selection survivors*/
                SearchIO.output("selecting from population  (current pop = " + population.Count + ").", 4);
                SearchIO.output(StarMath.MakePrintString(CalcPopulationStats(population)), 4);
                fitnessSelector.selectCandidates(ref population);
                /* 4. generate remainder of population with crossover generators */
                SearchIO.output("generating new candidates (current pop = " + population.Count + ").", 4);
                crossoverGenerator.GenerateCandidates(ref population, populationSize);
                /* 5. generate modifications to all with mutation */
                SearchIO.output("performing mutation (current pop = " + population.Count + ").", 4);
                mutationGenerator.GenerateCandidates(ref population);
                /* 6. evaluate new members of population.*/
                SearchIO.output("evaluating new popluation members.", 4);
                evaluate(population);
                k++;
                fStar = population.Min(c => c.fValues[0]);
                xStar = (from candidate in population
                         where (candidate.fValues[0] == fStar)
                         select candidate.x).First();
                SearchIO.output("x* = " + StarMath.MakePrintString(xStar), 4);
                SearchIO.output("f* = " + fStar, 4);
            } while (notConverged(k, numEvals, fStar, xStar, population.Select(a => a.x).ToList(),
                population.Select(a => a.fValues[0]).ToList()));
            return fStar;
        }

        private double[,] CalcPopulationStats(IEnumerable<Candidate> population)
        {
            var result = new double[3, f.Count];
            for (int i = 0; i < f.Count; i++)
            {
                result[0, i] = (from c in population select c.fValues[i]).Min();
                result[1, i] = (from c in population select c.fValues[0]).Average();
                result[2, i] = (from c in population select c.fValues[0]).Max();
            };
            return result;
        }


        private void evaluate(IList<Candidate> population)
        {
            for (var i = population.Count - 1; i >= 0; i--)
            {
                var candidate = population[i];
                if (candidate.fValues.Contains(double.NaN))
                {
                    population.RemoveAt(i);
                    var fVals  = calc_f_vector(candidate.x);
                    population.Add(new Candidate(fVals, candidate.x));
                }
            }
        }
    }
}