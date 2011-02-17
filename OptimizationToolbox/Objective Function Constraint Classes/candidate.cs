using System;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{

    /// <summary>
    /// 
    /// </summary>
    public class Candidate
    {
        /// <summary>
        /// Gets or sets the f values.
        /// </summary>
        /// <value>
        /// The f values.
        /// </value>
        public double[] fValues { get; set; }
        /// <summary>
        /// Gets or sets the g values.
        /// </summary>
        /// <value>
        /// The g values.
        /// </value>
        public double[] gValues { get; set; }
        /// <summary>
        /// Gets or sets the h values.
        /// </summary>
        /// <value>
        /// The h values.
        /// </value>
        public double[] hValues { get; set; }
        /// <summary>
        /// Gets or sets the x vector - the vector of design variables.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double[] x { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        public Candidate(double[]x, abstractOptMethod evaluationMethods = null)
        {
            this.x = (double[]) x.Clone();
            if (evaluationMethods!=null)
            {
//                evaluationMethods
            }
        }


        public Candidate(double f, double[] x)
        {
            fValues = new[] {f};
            this.x = (double[])x.Clone();
        }
    }
}
