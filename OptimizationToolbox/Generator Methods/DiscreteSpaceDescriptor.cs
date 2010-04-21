using System;
//using System.Linq;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DiscreteSpaceDescriptor
    {
        #region Fields
        abstractOptMethod optMethod;
        public double[][] unlinkedSpace;
        public List<double[][]> linkedSpace;
        int n;
        public double score;
        Boolean reCalcCharacteristics = true;

        /// The following four fields are referred to as the characteristics
        /// of the discrete space. They are all calculated in updateCharacteristics.
        Boolean allDiscrete;
        Boolean isSpaceValid;
        List<int> unlinkedVars;
        List<int> linkedVars;
        double sizeOfSpace = 0;
        #endregion

        #region Constructor
        public DiscreteSpaceDescriptor(abstractOptMethod optMethod, int n)
        {
            this.optMethod = optMethod;
            this.n = n;
            unlinkedSpace = new double[n][];
            linkedSpace = new List<double[][]>();
        }
        #endregion

        #region Properties
        public Boolean AllDiscrete
        {
            get
            {
                if (reCalcCharacteristics) updateCharacteristics();
                return allDiscrete;
            }
        }
        public Boolean IsSpaceValid
        {
            get
            {
                if (reCalcCharacteristics) updateCharacteristics();
                return isSpaceValid;
            }
        }
        public List<int> UnlinkedVars
        {
            get
            {
                if (reCalcCharacteristics) updateCharacteristics();
                return unlinkedVars;
            }
        }
        public List<int> LinkedVars
        {
            get
            {
                if (reCalcCharacteristics) updateCharacteristics();
                return linkedVars;
            }
        }
        public double SizeOfSpace
        {
            get
            {
                if (reCalcCharacteristics) updateCharacteristics();
                return sizeOfSpace;
            }
        }

        private void updateCharacteristics()
        {
            allDiscrete = true;
            isSpaceValid = true;
            linkedVars = new List<int>();
            unlinkedVars = new List<int>();

            for (int i = 0; i < n; i++)
            {
                if ((unlinkedSpace[i] != null)
                    && (unlinkedSpace[i].GetLength(0) > 0)
                    && varIsInLinked(i))
                    isSpaceValid = false;
                if ((unlinkedSpace[i] != null)
                    && (unlinkedSpace[i].GetLength(0) > 0))
                    unlinkedVars.Add(i);
                else if (varIsInLinked(i))
                {
                    linkedVars.Add(i);
                    if (!varIsSinglyLinked(i)) isSpaceValid = false;
                }
                else allDiscrete = false;
            }
            if (isSpaceValid)
            {
                sizeOfSpace = 1;
                foreach (int varIndex in unlinkedVars)
                    sizeOfSpace *= unlinkedSpace[varIndex].GetLength(0);
                foreach (double[][] linkSet in linkedSpace)
                    sizeOfSpace *= linkSet.GetLength(0);
            }
            else sizeOfSpace = 0;
            reCalcCharacteristics = false;
        }
        private Boolean varIsSinglyLinked(int i)
        {
            Boolean temp = false;
            foreach (double[][] linkSet in linkedSpace)
                if (!temp && (!Double.IsNaN(linkSet[0][i]))) temp = true;
                else if (!Double.IsNaN(linkSet[0][i])) return false;
            return temp;
        }
        private Boolean varIsInLinked(int i)
        {
            foreach (double[][] linkSet in linkedSpace)
                if (!Double.IsNaN(linkSet[0][i])) return true;
            return false;
        }
        #endregion

        #region Space-Sampling
        public long LatinHyperCube()
        {
            int numSamples = int.MaxValue;
            foreach (double[] vars in unlinkedSpace)
                if ((vars != null) && (vars.GetLength(0) > 0))
                    numSamples = Math.Min(numSamples, vars.GetLength(0));
            foreach (double[][] linkVars in linkedSpace)
                numSamples = Math.Min(numSamples, linkVars.GetLength(0));
            return LatinHyperCube(numSamples);
        }
        public long LatinHyperCube(int numSamples)
        {
            Random rnd = new Random();
            List<int>[] sampleIndices = new List<int>[n];
            for (int j = 0; j < n; j++)
            {
                sampleIndices[j] = new List<int>();
                for (int i = 0; i < numSamples; i++)
                    sampleIndices[j].Insert(rnd.Next(sampleIndices[j].Count), i);
            }
            long start = DateTime.Now.Ticks;
            double[] f = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                double[] point = new double[n];
                for (int j = 0; j < unlinkedVars.Count; j++)
                    point[unlinkedVars[j]]
                        = unlinkedSpace[unlinkedVars[j]][sampleIndices[unlinkedVars[j]][i]];
                f[i] = optMethod.calc_f(point, true);
            }
            return DateTime.Now.Ticks - start;
        }
        #endregion
        #region addUnlinkedVariableDeltas
        public void addUnlinkedVariableDeltas(int[] VarIndices, double[] lowerBound, double[] upperBound, double[] delta)
        {
            int lbi = 0, ubi = 0, di = 0;
            for (int vi = 0; vi < VarIndices.GetLength(0); vi++)
            {
                if (++lbi == lowerBound.GetLength(0)) lbi = 0;
                if (++ubi == lowerBound.GetLength(0)) ubi = 0;
                if (++di == lowerBound.GetLength(0)) di = 0;
                addUnlinkedVariableDeltas(VarIndices[vi], lowerBound[lbi], upperBound[ubi], delta[di]);
            }
        }
        public void addUnlinkedVariableDeltas(int[] VarIndices, double[] lowerBound, double[] upperBound, double delta)
        {
            int lbi = 0, ubi = 0;
            for (int vi = 0; vi < VarIndices.GetLength(0); vi++)
            {
                if (++lbi == lowerBound.GetLength(0)) lbi = 0;
                if (++ubi == lowerBound.GetLength(0)) ubi = 0;
                addUnlinkedVariableDeltas(VarIndices[vi], lowerBound[lbi], upperBound[ubi], delta);
            }
        }
        public void addUnlinkedVariableDeltas(int[] VarIndices, double lowerBound, double upperBound, double delta)
        {
            for (int vi = 0; vi < VarIndices.GetLength(0); vi++)
                addUnlinkedVariableDeltas(VarIndices[vi], lowerBound, upperBound, delta);
        }
        public void addUnlinkedVariableDeltas(int VarIndex, double lowerBound, double upperBound, double delta)
        {
            int numValues = 1 + (int)((upperBound - lowerBound) / delta);
            double[] values = new double[numValues];
            for (int i = 0; i < numValues; i++)
                values[i] = lowerBound + i * delta;
            addUnlinkedVariableValues(VarIndex, values);
        }
        #endregion
        #region addUnlinkedVariableValues
        /// <summary>
        /// Adds the unlinked variable values from a matrix, where the rows of
        /// the matrix correspond to the values for each variable in varIndices.
        /// </summary>
        /// <param name="varIndices">The variable indices.</param>
        /// <param name="values">The values matrix.</param>
        public void addUnlinkedVariableValues(int[] varIndices, double[,] values)
        {
            for (int i = 0; i < varIndices.GetLength(0); i++)
                addUnlinkedVariableValues(varIndices[i], StarMath.GetRow(i, values));
        }
        public void addUnlinkedVariableValues(int[] varIndices, double[][] values)
        {
            for (int i = 0; i < varIndices.GetLength(0); i++)
                addUnlinkedVariableValues(varIndices[i], values[i]);
        }
        public void addUnlinkedVariableValues(int[] varIndices, double[] values)
        {
            // the same values are added to all indices
            foreach (int i in varIndices)
                addUnlinkedVariableValues(i, values);
        }
        public void addUnlinkedVariableValues(int varIndex, double[] values)
        {
            if ((unlinkedSpace[varIndex] == null) || (unlinkedSpace[varIndex].GetLength(0) == 0))
                unlinkedSpace[varIndex] = (double[])values.Clone();
            else
            {
                double[] temp = unlinkedSpace[varIndex];
                unlinkedSpace[varIndex] = new double[temp.GetLength(0) + values.GetLength(0)];
                temp.CopyTo(unlinkedSpace[varIndex], 0);
                values.CopyTo(unlinkedSpace[varIndex], temp.GetLength(0));
            }
            reCalcCharacteristics = true;
        }
        #endregion
        #region addinkedVariableDeltas
        public void addLinkedVariableDeltas(int[] varIndices, double lowerBound, double upperBound, double delta)
        {
            int numValues = 1 + (int)((upperBound - lowerBound) / delta);
            double[,] valuesMatrix = new double[varIndices.GetLength(0), numValues];
            for (int i = 0; i < numValues; i++)
            {
                valuesMatrix[0, i] = lowerBound + i * delta;
                for (int j = 1; j < varIndices.GetLength(0); j++)
                    valuesMatrix[i, j] = valuesMatrix[i, 0];
            }
            addLinkedVariableValues(varIndices, valuesMatrix);
        }
        public void addLinkedVariableDeltas(
            int VarIndex1, double lowerBound1, double upperBound1, double delta1,
            int VarIndex2, double lowerBound2, double upperBound2, double delta2,
            int VarIndex3, double lowerBound3, double upperBound3, double delta3)
        {
            int numValues = 1 + (int)((upperBound1 - lowerBound1) / delta1);
            if (double.IsNaN(delta2)) delta2 = (upperBound2 - lowerBound2) / numValues;
            if (double.IsNaN(delta3)) delta3 = (upperBound3 - lowerBound3) / numValues;
            double[,] valuesMatrix = new double[3, numValues];
            for (int i = 0; i < numValues; i++)
            {
                valuesMatrix[i, 0] = lowerBound1 + i * delta1;
                valuesMatrix[i, 1] = lowerBound2 + i * delta2;
                valuesMatrix[i, 2] = lowerBound3 + i * delta3;
            }
            addLinkedVariableValues(new int[] { VarIndex1, VarIndex2, VarIndex3 }, valuesMatrix);
        }
        public void addLinkedVariableDeltas(
            int VarIndex1, double lowerBound1, double upperBound1, double delta1,
            int VarIndex2, double lowerBound2, double upperBound2, double delta2)
        {
            int numValues = 1 + (int)((upperBound1 - lowerBound1) / delta1);
            if (double.IsNaN(delta2)) delta2 = (upperBound2 - lowerBound2) / numValues;
            double[,] valuesMatrix = new double[2, numValues];
            for (int i = 0; i < numValues; i++)
            {
                valuesMatrix[i, 0] = lowerBound1 + i * delta1;
                valuesMatrix[i, 1] = lowerBound2 + i * delta2;
            }
            addLinkedVariableValues(new int[] { VarIndex1, VarIndex2 }, valuesMatrix);
        }
        #endregion
        #region addLinkedVariableValues
        public void addLinkedVariableValues(int[] varIndices, double[,] values)
        {
            double[][] linkedValMatrix = new double[values.GetLength(0)][];
            for (int i = 0; i < values.GetLength(0); i++)
                linkedValMatrix[i] = addLinkedVariableValue(varIndices, StarMath.GetRow(i, values));
            linkedSpace.Add(linkedValMatrix);
        }
        public void addLinkedVariableValues(int[] varIndices, double[][] values)
        {
            double[][] linkedValMatrix = new double[values.GetLength(0)][];
            for (int i = 0; i < values.GetLength(0); i++)
                linkedValMatrix[i] = addLinkedVariableValue(varIndices, values[i]);
            linkedSpace.Add(linkedValMatrix);
        }
        double[] addLinkedVariableValue(int[] varIndices, double[] values)
        {
            double[] linkedVal = new double[n];
            for (int i = 0; i < n; i++) linkedVal[i] = Double.NaN;
            for (int i = 0; i < varIndices.GetLength(0); i++)
                linkedVal[varIndices[i]] = values[i];
            reCalcCharacteristics = true;
            return linkedVal;
        }
        #endregion

        public double[] GetDesignVector(int[] UnlinkedIndices, int[] LinkedIndices)
        {
            double[] designVector = new double[n];

            if (UnlinkedIndices != null)
            {
                for (int i = 0; i < UnlinkedIndices.GetLength(0); i++)
                    designVector[unlinkedVars[i]] = unlinkedSpace[unlinkedVars[i]][UnlinkedIndices[i]];
            }
            if (LinkedIndices != null)
            {
                for (int i = 0; i < LinkedIndices.GetLength(0); i++)
                {
                    double[] linkValues = linkedSpace[i][LinkedIndices[i]];
                    for (int j = 0; j < n; j++)
                        if (!double.IsNaN(linkValues[j]))
                            designVector[j] = linkValues[j];
                }
            }
            return designVector;
        }
    }
}