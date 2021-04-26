// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="polynomialFunctions.cs" company="OptimizationToolbox">
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class polynomialInequality.
    /// Implements the <see cref="OptimizationToolbox.IDifferentiable" />
    /// Implements the <see cref="OptimizationToolbox.IInequality" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IDifferentiable" />
    /// <seealso cref="OptimizationToolbox.IInequality" />
    public class polynomialInequality : IDifferentiable, IInequality
    {
        /// <summary>
        /// The coeff exponents
        /// </summary>
        private List<double[]> coeff_exponents;
        /// <summary>
        /// The terms
        /// </summary>
        private List<string> terms;

        /// <summary>
        /// Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

        /// <summary>
        /// Gets the coeff exponents.
        /// </summary>
        /// <value>The coeff exponents.</value>
        private IEnumerable<double[]> Coeff_Exponents
        {
            get
            {
                if (coeff_exponents.Count < terms.Count)
                    coeff_exponents = polynomialHelper.convert(terms);
                return coeff_exponents;
            }
        }

        #region Constructing the Polynomial

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialInequality"/> class.
        /// </summary>
        public polynomialInequality()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialInequality"/> class.
        /// </summary>
        /// <param name="coeff_exponents">The coeff exponents.</param>
        public polynomialInequality(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialInequality"/> class.
        /// </summary>
        /// <param name="matrixOfTerms">The matrix of terms.</param>
        public polynomialInequality(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        /// <summary>
        /// Adds the specified new terms.
        /// </summary>
        /// <param name="newTerms">The new terms.</param>
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion


        #region Implementation of IOptFunction

        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion

        #region Implementation of IDifferentiable

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }

        #endregion
    }

    /// <summary>
    /// Class polynomialEquality.
    /// Implements the <see cref="OptimizationToolbox.IDifferentiable" />
    /// Implements the <see cref="OptimizationToolbox.IEquality" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IDifferentiable" />
    /// <seealso cref="OptimizationToolbox.IEquality" />
    public class polynomialEquality : IDifferentiable, IEquality
    {
        /// <summary>
        /// The coeff exponents
        /// </summary>
        private List<double[]> coeff_exponents;
        /// <summary>
        /// The terms
        /// </summary>
        private List<string> terms;

        /// <summary>
        /// Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

        /// <summary>
        /// Gets the coeff exponents.
        /// </summary>
        /// <value>The coeff exponents.</value>
        private IEnumerable<double[]> Coeff_Exponents
        {
            get
            {
                if (coeff_exponents.Count < terms.Count)
                    coeff_exponents = polynomialHelper.convert(terms);
                return coeff_exponents;
            }
        }

        #region Constructing the Polynomial

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialEquality"/> class.
        /// </summary>
        public polynomialEquality()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialEquality"/> class.
        /// </summary>
        /// <param name="coeff_exponents">The coeff exponents.</param>
        public polynomialEquality(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialEquality"/> class.
        /// </summary>
        /// <param name="matrixOfTerms">The matrix of terms.</param>
        public polynomialEquality(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        /// <summary>
        /// Adds the specified new terms.
        /// </summary>
        /// <param name="newTerms">The new terms.</param>
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion

        #region Implementation of IDifferentiable

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }
        #endregion

        #region Implementation of IOptFunction

        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion
    }

    /// <summary>
    /// Class polynomialObjFn.
    /// Implements the <see cref="OptimizationToolbox.IDifferentiable" />
    /// Implements the <see cref="OptimizationToolbox.IObjectiveFunction" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IDifferentiable" />
    /// <seealso cref="OptimizationToolbox.IObjectiveFunction" />
    public class polynomialObjFn : IDifferentiable, IObjectiveFunction
    {
        /// <summary>
        /// The coeff exponents
        /// </summary>
        private List<double[]> coeff_exponents;
        /// <summary>
        /// The terms
        /// </summary>
        private List<string> terms;

        /// <summary>
        /// Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

        /// <summary>
        /// Gets the coeff exponents.
        /// </summary>
        /// <value>The coeff exponents.</value>
        private IEnumerable<double[]> Coeff_Exponents
        {
            get
            {
                if (coeff_exponents.Count < terms.Count)
                    coeff_exponents = polynomialHelper.convert(terms);
                return coeff_exponents;
            }
        }

        #region Constructing the Polynomial

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialObjFn"/> class.
        /// </summary>
        public polynomialObjFn()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialObjFn"/> class.
        /// </summary>
        /// <param name="coeff_exponents">The coeff exponents.</param>
        public polynomialObjFn(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="polynomialObjFn"/> class.
        /// </summary>
        /// <param name="matrixOfTerms">The matrix of terms.</param>
        public polynomialObjFn(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        /// <summary>
        /// Adds the specified new terms.
        /// </summary>
        /// <param name="newTerms">The new terms.</param>
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        /// <summary>
        /// Adds the specified new term.
        /// </summary>
        /// <param name="newTerm">The new term.</param>
        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion

        #region Implementation of IDifferentiable

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }
        #endregion

        #region Implementation of IOptFunction
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion
    }

    /// <summary>
    /// Class polynomialHelper.
    /// </summary>
    internal static class polynomialHelper
    {
        #region Converters

        /// <summary>
        /// Converts the specified term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>System.String.</returns>
        internal static string convert(double[] term)
        {
            var sTerm = "";
            var num = term.GetLength(0);
            if (num == 0) return "";
            if (num == 1) return term[0].ToString(CultureInfo.InvariantCulture);
            if (term[0] == -1.0) sTerm = "-";
            else if (term[0] != 1.0) sTerm = term[0] + "*";
            for (var i = 1; i != num; i++)
                if (term[i] != 0.0)
                {
                    sTerm += "x" + i;
                    if (term[i] != 1.0)
                        sTerm += "^" + term[i] + "*";
                }
            return sTerm.TrimEnd(new[] { '*' });
        }

        /// <summary>
        /// Converts the specified s term.
        /// </summary>
        /// <param name="sTerm">The s term.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="Exception">Contains illegal character :" + sTerm[pos]</exception>
        /// <exception cref="Exception">Error in syntax of polynomial subterm: " + subTerms[j]</exception>
        /// <exception cref="Exception">Error in syntax of polynomial subterm: " + subTerms[j]</exception>
        internal static double[] convert(string sTerm)
        {
            var positions = new List<int>();
            int pos;
            var exponents = new List<double>();

            var coeff = 1.0;
            var badChars = new[] { ',', '(', ')', ' ', ':', ';', '\\', '\'', '\"' };

            if ((pos = sTerm.IndexOfAny(badChars)) >= 0)
                throw new Exception("Contains illegal character :" + sTerm[pos]);

            // at first I was going to allow a division to occur, but that complicates things
            // what about parnetheses to be exact? what about fractions.
            //string[] numeratorDenominator = sTerm.Split(new char[]{'/'});
            //if ((numeratorDenominator.GetLength(0)<2)||(numeratorDenominator[1].Length==0))
            //    ndCount=1;
            //for (int j=0; j!=ndCount; j++)
            //{
            //   string[] subTerms = numeratorDenominator[j].Split(new char[]{'*'}); 
            var subTerms = sTerm.Split(new[] { '*' });
            for (var j = 0; j != subTerms.GetLength(0); j++)
            {
                if (subTerms[j][0] == '-')
                {
                    subTerms[j] = subTerms[j].Remove(0, 1);
                    coeff *= -1;
                }
                if (char.IsLetter(subTerms[j][0]))
                {
                    var temp = subTerms[j].Remove(0, 1);
                    var temps = temp.Split(new[] { '^' });
                    if ((temps.GetLength(0) == 1) && (int.TryParse(temps[0], out pos)))
                    {
                        positions.Add(pos);
                        exponents.Add(1.0);
                    }
                    else
                    {
                        double exponent;
                        if ((temps.GetLength(0) == 2) && (int.TryParse(temps[0], out pos)) &&
                            (double.TryParse(temps[1], out exponent)))
                        {
                            positions.Add(pos);
                            exponents.Add(exponent);
                        }
                        else throw new Exception("Error in syntax of polynomial subterm: " + subTerms[j]);
                    }
                }
                else
                {
                    double tempCoeff;
                    if (double.TryParse(subTerms[j], out tempCoeff))
                        coeff *= tempCoeff;
                    else throw new Exception("Error in syntax of polynomial subterm: " + subTerms[j]);
                }
            }
            var maxIndex = (positions.Count == 0) ? 0 : positions.Max();

            var term = new double[maxIndex + 1];
            term[0] = coeff;
            for (var i = 1; i <= maxIndex; i++)
            {
                var w = positions.FirstOrDefault(whichVar => (i == whichVar));
                pos = positions.IndexOf(w);
                if (pos == -1) term[i] = 0.0;
                else term[i] = exponents[pos];
            }
            return term;
        }

        /// <summary>
        /// Converts the specified s terms.
        /// </summary>
        /// <param name="sTerms">The s terms.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        internal static List<double[]> convert(List<string> sTerms)
        {
            var terms = new List<double[]>(sTerms.Count);
            foreach (string s in sTerms)
                terms.Add(convert(s));
            return terms;
        }

        /// <summary>
        /// Converts the specified terms.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        internal static List<string> convert(List<double[]> terms)
        {
            return new List<string>(terms.Select(convert));
        }

        /// <summary>
        /// Converts the specified matrix of terms.
        /// </summary>
        /// <param name="matrixOfTerms">The matrix of terms.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        internal static List<double[]> convert(double[,] matrixOfTerms)
        {
            var numTerms = matrixOfTerms.GetLength(0);
            var numVars = matrixOfTerms.GetLength(1);
            var listOfTerms = new List<double[]>(numTerms);
            for (var i = 0; i != numTerms; i++)
            {
                var term = new double[numVars];
                for (var j = 0; j != numVars; j++)
                    term[j] = matrixOfTerms[i, j];
                listOfTerms.Add(term);
            }
            return listOfTerms;
        }

        #endregion

        #region Calculating Helpers

        /// <summary>
        /// Calculates the term.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="term">The term.</param>
        /// <returns>System.Double.</returns>
        internal static double calculateTerm(double[] x, double[] term)
        {
            /* the first term is always the coefficient. We can start the product with that. */
            var product = term[0];
            for (var i = 1; i < term.GetLength(0); i++)
                if (term[i] != 0.0)
                    product *= Math.Pow(x[i - 1], term[i]);
            return product;
        }

        /// <summary>
        /// Calculates the deriv.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="term">The term.</param>
        /// <param name="position">The position.</param>
        /// <returns>System.Double.</returns>
        internal static double calculateDeriv(double[] x, double[] term, int position)
        {
            /* the first term is always the coefficient. We can start the product with that. */
            if (((position + 1) >= term.Length) || (term[position + 1] == 0.0)) return 0.0;
            var product = term[0];
            for (var i = 1; i < term.GetLength(0); i++)
                if ((i - 1) == position)
                    product *= term[i] * Math.Pow(x[i - 1], (term[i] - 1.0));
                else if (term[i] != 0.0)
                    product *= Math.Pow(x[i - 1], term[i]);
            return product;
        }

        #endregion
    }
}