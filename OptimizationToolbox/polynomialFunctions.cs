using System;
using System.Collections.Generic;

using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class polynomialInequality : inequality
    {
        List<double[]> coeff_exponents;
        List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }
        private List<double[]> Coeff_Exponents
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
            this.terms = polynomialHelper.convert(coeff_exponents);
        }
        public polynomialInequality(double[,] matrixOfTerms)
        {
            this.coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            this.terms = polynomialHelper.convert(this.coeff_exponents);
        }
        public void Add(double[] newTerm)
        {
            this.coeff_exponents.Add(newTerm);
            this.terms.Add(polynomialHelper.convert(newTerm));
        }
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                this.Add(term);
        }
        public void Add(string newTerm)
        {
            this.coeff_exponents.Add(polynomialHelper.convert(newTerm));
            this.terms.Add(newTerm);
        }
        #endregion

        #region Calculating
        protected override double calc(double[] x)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateTerm(x, c_e_term);
            return sum;
        }
        public override double deriv_wrt_xi(double[] x, int position)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateDeriv(x, c_e_term, position);
            return sum;
        }
        #endregion
    }
    public class polynomialEquality : equality
    {
        List<double[]> coeff_exponents;
        List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }
        private List<double[]> Coeff_Exponents
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
            this.terms = polynomialHelper.convert(coeff_exponents);
        }
        public polynomialEquality(double[,] matrixOfTerms)
        {
            this.coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            this.terms = polynomialHelper.convert(this.coeff_exponents);
        }
        public void Add(double[] newTerm)
        {
            this.coeff_exponents.Add(newTerm);
            this.terms.Add(polynomialHelper.convert(newTerm));
        }
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                this.Add(term);
        }
        public void Add(string newTerm)
        {
            this.coeff_exponents.Add(polynomialHelper.convert(newTerm));
            this.terms.Add(newTerm);
        }
        #endregion

        #region Calculating
        protected override double calc(double[] x)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateTerm(x, c_e_term);
            return sum;
        }
        public override double deriv_wrt_xi(double[] x, int position)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateDeriv(x, c_e_term, position);
            return sum;
        }
        #endregion
    }
    public class polynomialObjFn : objectiveFunction
    {
        List<double[]> coeff_exponents;
        List<string> terms;

        public List<string> Terms
        {
            get { return terms; }
            set { terms = value; }
        }
        private List<double[]> Coeff_Exponents
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
            this.terms = polynomialHelper.convert(coeff_exponents);
        }
        public polynomialObjFn(double[,] matrixOfTerms)
        {
            this.coeff_exponents = polynomialHelper.convert(matrixOfTerms);
            this.terms = polynomialHelper.convert(this.coeff_exponents);
        }
        public void Add(double[] newTerm)
        {
            this.coeff_exponents.Add(newTerm);
            this.terms.Add(polynomialHelper.convert(newTerm));
        }
        public void Add(List<string> newTerms)
        {
            foreach (string term in newTerms)
                this.Add(term);
        }
        public void Add(string newTerm)
        {
            this.coeff_exponents.Add(polynomialHelper.convert(newTerm));
            this.terms.Add(newTerm);
        }
        #endregion

        #region Calculating
        protected override double calc(double[] x)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateTerm(x, c_e_term);
            return sum;
        }
        public override double deriv_wrt_xi(double[] x, int position)
        {
            double sum = 0.0;
            foreach (double[] c_e_term in this.Coeff_Exponents)
                sum += polynomialHelper.calculateDeriv(x, c_e_term, position);
            return sum;
        }
        #endregion
    }

    public static class polynomialHelper
    {
        #region Converters
        internal static string convert(double[] term)
        {
            string sTerm = "";
            int num = term.GetLength(0);
            if (num == 0) return "";
            if (num == 1) return term[0].ToString();
            else
            {
                if (term[0] == -1.0) sTerm = "-";
                else if (term[0] != 1.0) sTerm = term[0].ToString() + "*";
                for (int i = 1; i != num; i++)
                    if (term[i] != 0.0)
                    {
                        sTerm += "x" + i.ToString();
                        if (term[i] != 1.0)
                            sTerm += "^" + term[i].ToString() + "*";
                    }
                return sTerm.TrimEnd(new char[] { '*' });
            }
        }

        internal static double[] convert(string sTerm)
        {
            List<int> positions = new List<int>();
            int pos;
            List<double> exponents = new List<double>();
            double exponent;

            double coeff = 1.0;
            double tempCoeff;
            char[] badChars = new char[] { ',', '(', ')', ' ', ':', ';', '\\', '\'', '\"' };

            if ((pos = sTerm.IndexOfAny(badChars)) >= 0)
                throw new Exception("Contains illegal character :" + sTerm[pos].ToString());

            // at first I was going to allow a division to occur, but that complicates things
            // what about parnetheses to be exact? what about fractions.
            //string[] numeratorDenominator = sTerm.Split(new char[]{'/'});
            //if ((numeratorDenominator.GetLength(0)<2)||(numeratorDenominator[1].Length==0))
            //    ndCount=1;
            //for (int j=0; j!=ndCount; j++)
            //{
            //   string[] subTerms = numeratorDenominator[j].Split(new char[]{'*'}); 
            string[] subTerms = sTerm.Split(new char[] { '*' });
            for (int j = 0; j != subTerms.GetLength(0); j++)
            {
                if (subTerms[j][0] == '-')
                {
                    subTerms[j] = subTerms[j].Remove(0, 1);
                    coeff *= -1;
                }
                if (char.IsLetter(subTerms[j][0]))
                {
                    string temp = subTerms[j].Remove(0, 1);
                    string[] temps = temp.Split(new char[] { '^' });
                    if ((temps.GetLength(0) == 1) && (int.TryParse(temps[0], out pos)))
                    {
                        positions.Add(pos);
                        exponents.Add(1.0);
                    }
                    else if ((temps.GetLength(0) == 2) && (int.TryParse(temps[0], out pos)) &&
                        (double.TryParse(temps[1], out exponent)))
                    {
                        positions.Add(pos);
                        exponents.Add(exponent);
                    }
                    else throw new Exception("Error in syntax of polynomial subterm: " + subTerms[j]);
                }
                else if (double.TryParse(subTerms[j], out tempCoeff))
                    coeff *= tempCoeff;
                else throw new Exception("Error in syntax of polynomial subterm: " + subTerms[j]);
            }
            int maxIndex = 0;
            foreach (int a in positions)
                if (a > maxIndex) maxIndex = a;

            double[] term = new double[maxIndex + 1];
            term[0] = coeff;
            for (int i = 1; i <= maxIndex; i++)
            {
                pos = positions.FindIndex(delegate(int whichVar) { return (i == whichVar); });
                if (pos == -1) term[i] = 0.0;
                else term[i] = exponents[pos];
            }
            return term;
        }
        internal static List<double[]> convert(List<string> sTerms)
        {
            List<double[]> terms = new List<double[]>(sTerms.Count);
            foreach (string s in sTerms)
                terms.Add(polynomialHelper.convert(s));
            return terms;
        }

        internal static List<string> convert(List<double[]> terms)
        {
            List<string> sTerms = new List<string>(terms.Count);
            foreach (double[] c in terms)
                sTerms.Add(polynomialHelper.convert(c));
            return sTerms;
        }

        internal static List<double[]> convert(double[,] matrixOfTerms)
        {
            int numTerms = matrixOfTerms.GetLength(0);
            int numVars = matrixOfTerms.GetLength(1);
            List<double[]> listOfTerms = new List<double[]>(numTerms);
            for (int i = 0; i != numTerms; i++)
            {
                double[] term = new double[numVars];
                for (int j = 0; j != numVars; j++)
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
            double product = term[0];
            for (int i = 1; i < term.GetLength(0); i++)
                if (term[i] != 0.0)
                    product *= Math.Pow(x[i - 1], term[i]);
            return product;
        }

        internal static double calculateDeriv(double[] x, double[] term, int position)
        {
            /* the first term is always the coefficient. We can start the product with that. */
            if (((position + 1) >= term.Length) || (term[position + 1] == 0.0)) return 0.0;
            double product = term[0];
            for (int i = 1; i < term.GetLength(0); i++)
                if ((i - 1) == position)
                    product *= term[i] * Math.Pow(x[i - 1], (term[i] - 1.0));
                else if (term[i] != 0.0)
                    product *= Math.Pow(x[i - 1], term[i]);
            return product;
        }
        #endregion
    }
}
