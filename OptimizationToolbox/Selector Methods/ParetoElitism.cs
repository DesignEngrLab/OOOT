// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="ParetoElitism.cs" company="OptimizationToolbox">
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
    /// Class ParetoElitism.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class ParetoElitism : abstractSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParetoElitism"/> class.
        /// </summary>
        /// <param name="optimizationDirections">The optimization directions.</param>
        public ParetoElitism(params optimize[] optimizationDirections)
            : base(optimizationDirections)
        {
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double control = double.NaN)
        {                                                     
            candidates = ParetoFunctions.FindParetoCandidates(candidates, optDirections);
        }
    }

}
