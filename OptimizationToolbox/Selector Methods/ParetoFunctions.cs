using System;
using System.Collections.Generic;
using System.Linq;
using OptimizationToolbox;

namespace Skewboid
{
    internal static class ParetoFunctions
    {
        private static int numObjectives;
        private static double alpha;
        private static double[] weights;
        private static optimize[] optDirections;
        private const double tolerance = 0.0001; // tolerance


        /// <summary>
        /// Finds the given number candidates by iterating (root-finding) on alpha.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="numKeep">The num keep.</param>
        /// <param name="alphaTarget">The alpha target.</param>
        /// <param name="_weights">The _weights.</param>
        /// <param name="_optDirections">The _opt directions.</param>
        /// <returns></returns>
        public static List<ICandidate> FindGivenNumCandidates(List<ICandidate> candidates, int numKeep, out double alphaTarget,
                                                              double[] _weights, optimize[] _optDirections = null)
        {
            initializeWeightsAndDirections(candidates, _weights, _optDirections);
            double alphaLB, alphaUB;
            int numatLB, numatUB;
            var paretoSet = FindParetoCandidates(candidates, 0);
            var numTarget = paretoSet.Count;
            if (numTarget == numKeep)
            {
                alphaTarget = 0;
                return paretoSet;
            }
            if (numTarget < numKeep)
            {
                /* not enough in the real pareto - need to relax */
                alphaLB = -1;
                numatLB = candidates.Count;
                alphaUB = 0;
                numatUB = numTarget;
                if (numatLB <= numKeep)
                {
                    alphaTarget = alphaLB;
                    return candidates;
                }
            }
            else
            {
                /* too manyin the real pareto - need to filter */
                alphaLB = 0;
                numatLB = numTarget;
                alphaUB = 1;
                paretoSet = FindParetoCandidates(candidates, alphaUB);
                numatUB = paretoSet.Count;
                if (numatUB >= numKeep)
                {
                    alphaTarget = alphaUB;
                    return paretoSet;
                }
            }
            alphaTarget = double.NaN;
            /* debugger requires alphaTarget to be assigned, and it is worried that
                                   * the while loop will be passed over completely, hence need this line.
                                   * so, if that indeed happens, we throw an error. */
            int k = 0;
            while (numKeep != numTarget && alphaUB - alphaLB > tolerance)
            {
                k++;
                alphaTarget = alphaLB + (alphaUB - alphaLB) * (numatLB - numKeep) / (numatLB - numatUB);
                paretoSet = FindParetoCandidates(candidates, alphaTarget);
                numTarget = paretoSet.Count;
                if (numTarget > numKeep)
                {
                    alphaLB = alphaTarget;
                    numatLB = numTarget;
                }
                else if (numTarget < numKeep)
                {
                    alphaUB = alphaTarget;
                    numatUB = numTarget;
                }
            }
            if (double.IsNaN(alphaTarget)) throw new Exception("Somehow the while loop was passed over.");
            return paretoSet;
        }

        public static List<ICandidate> FindParetoCandidates(IEnumerable<ICandidate> candidates, double _alpha,
                                                              double[] _weights, optimize[] _optDirections = null)
        {
            initializeWeightsAndDirections(candidates, _weights, _optDirections);
            return FindParetoCandidates(candidates, _alpha);
        }
        /// <summary>
        /// Finds the pareto candidates (no skewboid, no weights - the real deal).
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="_optDirections">The _opt directions.</param>
        /// <returns></returns>
        public static List<ICandidate> FindParetoCandidates(IEnumerable<ICandidate> candidates, optimize[] _optDirections = null)
        {
            var paretoSet = new List<ICandidate>();
            foreach (var c in candidates)
            {
                var cIsDominated = false;         
                for (int i = paretoSet.Count - 1; i >= 0; i--)
                {
                    var pc = paretoSet[i];
                    if (dominates(c, pc))
                        paretoSet.Remove(pc);
                    else if (dominates(pc, c))
                    {
                        cIsDominated = true;
                        break;
                    }
                }
                if (!cIsDominated) paretoSet.Add(c);
            }  
            return paretoSet;
        }


        private static List<ICandidate> FindParetoCandidates(IEnumerable<ICandidate> candidates, double _alpha)
        {
            alpha = _alpha;
            var paretoSet = new List<ICandidate>();
            if (weights != null)
                foreach (var c in candidates)
                    UpdateParetoWithWeights(paretoSet, c);
            else
                foreach (var c in candidates)
                    UpdateParetoDiversity(paretoSet, c);

            return paretoSet;
        }


        private static void initializeWeightsAndDirections(IEnumerable<ICandidate> candidates, double[] _weights, optimize[] _optDirections)
        {
            numObjectives = candidates.First().objectives.GetLength(0);
            if (_weights != null) weights = (double[])_weights.Clone();
            else weights = null;
            if (_optDirections == null)
            {
                optDirections = new optimize[numObjectives];
                for (int i = 0; i < numObjectives; i++)
                    optDirections[i] = optimize.minimize;
            }
            else optDirections = (optimize[])_optDirections.Clone();
        }


        private static void UpdateParetoDiversity(List<ICandidate> paretoSet, ICandidate c)
        {
            for (int i = paretoSet.Count - 1; i >= 0; i--)
            {
                var pc = paretoSet[i];
                if (dominatesDiversity(c, pc))
                    paretoSet.Remove(pc);
                else if (dominatesDiversity(pc, c)) return;
            }
            paretoSet.Add(c);
        }

        private static void UpdateParetoWithWeights(List<ICandidate> paretoSet, ICandidate c)
        {
            for (int i = paretoSet.Count - 1; i >= 0; i--)
            {
                var pc = paretoSet[i];
                if (dominatesWithWeights(c, pc))
                    paretoSet.Remove(pc);
                else if (dominatesWithWeights(pc, c)) return;
            }
            paretoSet.Add(c);
        }

        /// <summary>
        /// Does c1 dominate c2?
        /// </summary>
        /// <param name="c1">the subject candidate, c1 (does this dominate...).</param>
        /// <param name="c2">the object candidate, c2 (is dominated by).</param>
        /// <returns></returns>
        private static Boolean dominatesWithWeights(ICandidate c1, ICandidate c2)
        {
            var Dominates = false;
            for (int i = 0; i < numObjectives; i++)
            {
                double c1Value = 0.0, c2Value = 0.0;
                for (int j = 0; j < numObjectives; j++)
                {
                    if (j == i)
                    {
                        c1Value += (int)optDirections[j] * weights[j] * c1.objectives[j];
                        c2Value += (int)optDirections[j] * weights[j] * c2.objectives[j];
                    }
                    else
                    {
                        c1Value += (int)optDirections[j] * alpha * weights[j] * c1.objectives[j];
                        c2Value += (int)optDirections[j] * alpha * weights[j] * c2.objectives[j];
                    }
                }
                if (c1Value < c2Value) return false;
                if (c1Value > c2Value) Dominates = true;
            }
            return Dominates;
        }

        /// <summary>
        /// Does c1 dominate c2?
        /// </summary>
        /// <param name="c1">the subject candidate, c1 (does this dominate...).</param>
        /// <param name="c2">the object candidate, c2 (is dominated by).</param>
        /// <returns></returns>
        private static Boolean dominatesDiversity(ICandidate c1, ICandidate c2)
        {
            var Dominates = false;
            for (int i = 0; i < numObjectives; i++)
            {
                double c1Value = 0.0, c2Value = 0.0;
                for (int j = 0; j < numObjectives; j++)
                {
                    if (j == i)
                    {
                        c1Value += (int)optDirections[j] * c1.objectives[j] / Math.Abs(c2.objectives[j]);
                        c2Value += (int)optDirections[j] * Math.Sign(c2.objectives[j]);
                    }
                    else
                    {
                        c1Value += (int)optDirections[j] * alpha * c1.objectives[j] / Math.Abs(c2.objectives[j]);
                        c2Value += (int)optDirections[j] * alpha * Math.Sign(c2.objectives[j]);
                    }
                }
                if (c1Value < c2Value) return false;
                if (c1Value > c2Value) Dominates = true;
            }
            return Dominates;
        }

        /// <summary>
        /// Dominateses the specified c1.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns></returns>
        private static Boolean dominates(ICandidate c1, ICandidate c2)
        {
            for (var i = 0; i < numObjectives; i++)
                if (((int)optDirections[i]) * c1.objectives[i] <
                    ((int)optDirections[i]) * c2.objectives[i])
                    return false;
            return true;
        }

    }

}
