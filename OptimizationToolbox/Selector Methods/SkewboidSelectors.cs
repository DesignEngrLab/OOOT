// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SkewboidSelectors.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skewboid;

namespace OptimizationToolbox.Selector_Methods
{
    /// <summary>
    /// Class SkewboidDiversity.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class SkewboidDiversity : abstractSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkewboidDiversity"/> class.
        /// </summary>
        /// <param name="optimizationDirections">The optimization directions.</param>
        public SkewboidDiversity(params optimize[] optimizationDirections) : base(optimizationDirections)
        {
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="fractionToKeep">The fraction to keep.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int) (candidates.Count()*fractionToKeep);
            double alphaTarget;
            candidates = ParetoFunctions.FindGivenNumCandidates(candidates, numKeep, out alphaTarget, null,
                optDirections);
        }
    }

    /// <summary>
    /// Class SkewboidWeighted.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class SkewboidWeighted : abstractSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkewboidWeighted"/> class.
        /// </summary>
        /// <param name="weights">The weights.</param>
        /// <param name="optimizationDirections">The optimization directions.</param>
        /// <exception cref="Exception">Received an unequal array of weights and optimization directions in Weighted Skewboid.</exception>
        public SkewboidWeighted(double[] weights, params optimize[] optimizationDirections)
            : base(optimizationDirections)
        {
            if (weights.GetLength(0) != optimizationDirections.GetLength(0))
                throw new Exception("Received an unequal array of weights and optimization directions in Weighted Skewboid.");
            this.weights = (double[])weights.Clone();
        }

        /// <summary>
        /// Gets or sets the weights.
        /// </summary>
        /// <value>The weights.</value>
        public double[] weights { get; set; }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="fractionToKeep">The fraction to keep.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count() * fractionToKeep);
            double alphaTarget;
            candidates = ParetoFunctions.FindGivenNumCandidates(candidates, numKeep, out alphaTarget, weights,
                optDirections);
        }
    }

}
