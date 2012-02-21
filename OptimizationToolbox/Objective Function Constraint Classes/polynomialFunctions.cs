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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OptimizationToolbox
{
    public class polynomialInequality : IDifferentiable, IInequality
    {
        private List<double[]> coeff_exponents;
        private List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

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

        public polynomialInequality()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        public polynomialInequality(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public polynomialInequality(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion


        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }

        #endregion
    }

    public class polynomialEquality : IDifferentiable, IEquality
    {
        private List<double[]> coeff_exponents;
        private List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

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

        public polynomialEquality()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        public polynomialEquality(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public polynomialEquality(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }
        #endregion

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion
    }

    public class polynomialObjFn : IDifferentiable, IObjectiveFunction
    {
        private List<double[]> coeff_exponents;
        private List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }

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

        public polynomialObjFn()
        {
            coeff_exponents = new List<double[]>();
            terms = new List<string>();
        }

        public polynomialObjFn(List<double[]> coeff_exponents)
        {
            this.coeff_exponents = coeff_exponents;
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public polynomialObjFn(double[,] matrixOfTerms)
        {
            coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            terms = polynomialHelper.convert(coeff_exponents);
        }

        public void Add(double[] newTerm)
        {
            coeff_exponents.Add(newTerm);
            terms.Add(polynomialHelper.convert(newTerm));
        }

        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                Add(term);
        }

        public void Add(string newTerm)
        {
            coeff_exponents.Add(polynomialHelper.convert(newTerm));
            terms.Add(newTerm);
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateDeriv(x, c_e_term, i));
        }
        #endregion

        #region Implementation of IOptFunction
        public double calculate(double[] x)
        {
            return Coeff_Exponents.Sum(c_e_term => polynomialHelper.calculateTerm(x, c_e_term));
        }

        #endregion
    }

    internal static class polynomialHelper
    {
        #region Converters

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
                pos = positions.FindIndex(whichVar => (i == whichVar));
                if (pos == -1) term[i] = 0.0;
                else term[i] = exponents[pos];
            }
            return term;
        }

        internal static List<double[]> convert(List<string> sTerms)
        {
            var terms = new List<double[]>(sTerms.Count);
            foreach (string s in sTerms)
                terms.Add(convert(s));
            return terms;
        }

        internal static List<string> convert(List<double[]> terms)
        {
            return new List<string>(terms.Select(convert));
        }

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

        internal static double calculateTerm(double[] x, double[] term)
        {
            /* the first term is always the coefficient. We can start the product with that. */
            var product = term[0];
            for (var i = 1; i < term.GetLength(0); i++)
                if (term[i] != 0.0)
                    product *= Math.Pow(x[i - 1], term[i]);
            return product;
        }

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