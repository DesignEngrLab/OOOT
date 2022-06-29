using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LMOpt
{
    /// <summary>
    /// Class Levenberg-Marquadt Optimization. This class includes functions to run this optimization method.
    /// It is an abstract class, so you must inherit it in A new class. That new class must have the following:
    /// 1. A constructor that calls the base constructor
    /// 2. A method called solveResiduals (the non-squared terms of the objective function)
    /// 3. A method called solveGradientOfResiduals (the derivative of the residuals w.r.t. each design variable)
    /// </summary>
    public static class MatrixMath
    {
        private const double DefaultEqualityTolerance = 1e-11;
     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] AddArrays(this double[] a, double[] b)
        {
            var n = a.Length;
            var result = new double[n];
            for (int i = 0; i < n; i++)
                result[i] = a[i] + b[i];
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ScalarMultiplyVector(this double c, double[] a)
        {
            var n = a.Length;
            var result = new double[n];
            for (int i = 0; i < n; i++)
                result[i] = c * a[i];
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] VectorMultiplyMatrix(this double[] a, double[,] m)
        {
            var n = a.Length;
            var numCols = m.GetLength(1);
            var result = new double[numCols];
            for (int i = 0; i < numCols; i++)
            {
                var sum = 0.0;
                for (int j = 0; j < n; j++)
                    sum += a[j] * m[j, i];
                result[i] = sum;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[,] MultiplyATB(this double[,] A, double[,] B)
        {
            var numCommon = A.GetLength(0);
            var numRows = A.GetLength(1);
            var numCols = B.GetLength(1);
            var result = new double[numRows, numCols];
            for (int r = 0; r < numRows; ++r)
            {
                for (int c = 0; c < numCols; ++c)
                {
                    result[r, c] = 0;
                    for (int i = 0; i < numCommon; ++i)
                        result[r, c] += A[i, r] * B[i, c];
                }
            }
            return result;
        }

        /// <summary>
        /// Solves the specified A matrix.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <param name="IsASymmetric">Is matrix A symmetric.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix, A, must be square.
        /// or
        /// Matrix, A, must be have the same number of rows as the vector, b.</exception>
        public static bool MatrixSolve(this double[,] A, IList<double> b, out double[] answer,
            Boolean IsASymmetric = false)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Matrix, A, must be square.");
            if (length != b.Count)
                throw new ArithmeticException("Matrix, A, must be have the same number of rows as the vector, b.");
            if (length == 3)
                return solveViaCramersRule3(A, b, out answer);
            if (length == 2)
                return solveViaCramersRule2(A, b, out answer);

            return solveBig(A, b, out answer, IsASymmetric);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool solveViaCramersRule3(this double[,] A, IList<double> b, out double[] answer)
        {
            var denominator = (A[0, 0] * A[1, 1] * A[2, 2])
                   + (A[0, 1] * A[1, 2] * A[2, 0])
                   + (A[0, 2] * A[1, 0] * A[2, 1])
                   - (A[0, 0] * A[1, 2] * A[2, 1])
                   - (A[0, 1] * A[1, 0] * A[2, 2])
                   - (A[0, 2] * A[1, 1] * A[2, 0]);
            if (IsNegligible(denominator))
            {
                answer = Array.Empty<double>();
                return false;
            }
            denominator = 1 / denominator;
            answer = new[]
            {
              denominator*  ((b[0] * A[1, 1] * A[2, 2])
                 + (A[0, 1] * A[1, 2] * b[2])
                 + (A[0, 2] * b[1] * A[2, 1])
                 - (b[0] * A[1, 2] * A[2, 1])
                 - (A[0, 1] * b[1] * A[2, 2])
                 - (A[0, 2] * A[1, 1] * b[2])),
               denominator*   ( (A[0, 0] * b[1] * A[2, 2])
                  + (b[0] * A[1, 2] * A[2, 0])
                  + (A[0, 2] * A[1, 0] * b[2])
                  - (A[0, 0] * A[1, 2] * b[2])
                  - (b[0] * A[1, 0] * A[2, 2])
                  - (A[0, 2] * b[1] * A[2, 0])),
               denominator*   ( (A[0, 0] * A[1, 1] * b[2])
                  + (A[0, 1] * b[1] * A[2, 0])
                  + (b[0] * A[1, 0] * A[2, 1])
                  - (A[0, 0] * b[1] * A[2, 1])
                  - (A[0, 1] * A[1, 0] * b[2])
                  - (b[0] * A[1, 1] * A[2, 0]))
            };
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool solveViaCramersRule2(double[,] a, IList<double> b, out double[] answer)
        {
            var denominator = a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            if (denominator == 0)
            {
                answer = Array.Empty<double>();
                return false;
            }
            denominator = 1 / denominator;
            answer = new[]
            {
              denominator * (b[0]*a[1,1]-b[1]*a[0,1]),
              denominator * (b[1]*a[0,0]-b[0]*a[1,0])
            };
            return true;
        }


        /// <summary>
        /// Solves the by inverse.
        /// </summary>
        /// <param name="A">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="IsASymmetric">Is A known to be Symmetric?</param>
        /// <param name="potentialDiagonals">The potential diagonals.</param>
        /// <returns>System.Double[].</returns>
        private static bool solveBig(double[,] A, IList<double> b, out double[] answer, bool IsASymmetric = false)
        {
            var length = b.Count;
            if (IsASymmetric)
            {
                if (!CholeskyDecomposition(A, out var L))
                {
                    answer = Array.Empty<double>();
                    return false;
                }
                answer = new double[length];
                // forward substitution
                for (int i = 0; i < length; i++)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = 0; j < i; j++)
                        sumFromKnownTerms += L[i, j] * answer[j];
                    answer[i] = (b[i] - sumFromKnownTerms);
                }

                for (int i = 0; i < length; i++)
                {
                    if (L[i, i] == 0) return false;
                    answer[i] /= L[i, i];
                }
                // backward substitution
                for (int i = length - 1; i >= 0; i--)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = i + 1; j < length; j++)
                        sumFromKnownTerms += L[j, i] * answer[j];
                    answer[i] -= sumFromKnownTerms;
                }
                return true;
            }
            else
            {
                double[,] LU = null;
                int[] permutationVector = null;
                try
                {
                    LU = LUDecomposition(A, out permutationVector, length);
                }
                catch
                {
                    answer = Array.Empty<double>();
                    return false;
                }
                answer = new double[length];
                // forward substitution
                for (int i = 0; i < length; i++)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = 0; j < i; j++)
                        sumFromKnownTerms += LU[permutationVector[i], j] * answer[j];
                    answer[i] = (b[permutationVector[i]] - sumFromKnownTerms) / LU[permutationVector[i], i];
                }
                // backward substitution
                for (int i = length - 1; i >= 0; i--)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = i + 1; j < length; j++)
                        sumFromKnownTerms += LU[permutationVector[i], j] * answer[j];
                    answer[i] -= sumFromKnownTerms;
                }
                return true;
            }
        }


        /// <summary>
        /// Returns the LU decomposition of A in A new matrix.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="permutationVector">The resulting permutation vector - how the rows are re-ordered to
        /// create L and U.</param>
        /// <param name="length">The length/order/number of rows of matrix, A.</param>
        /// <param name="robustReorder">if set to <c>true</c> [robust reorder]. But this is an internal recursive call
        /// and should not be set outside.</param>
        /// <param name="lastZeroIndices">The last zero indices - is calculated in this function, but if it is already
        /// known, then...by all means.</param>
        /// <returns>A matrix of equal size to A that combines the L and U. Here the diagonals belongs to L and the U's diagonal
        /// elements are all 1.</returns>
        /// <exception cref="ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double[,] LUDecomposition(double[,] A, out int[] permutationVector, int length = -1,
            bool robustReorder = false, List<int>[] lastZeroIndices = null)
        {
            // This is an implementation of Crout’s LU Decomposition Algorithm
            if (length == -1) length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("LU Decomposition can only be determined for square matrices.");
            // this lastZeroIndices is A an array of the last column index in each row that contains A zero (or is
            // negligible. It is used to determine what other row to swap with, if the current row has A zero diagonal.
            if (lastZeroIndices == null)
            {
                lastZeroIndices = new List<int>[length];
                for (int i = 0; i < length; i++)
                {
                    lastZeroIndices[i] = new List<int>();
                    for (int j = 0; j < length; j++)
                        if (IsNegligible(A[i, j])) lastZeroIndices[i].Add(j);
                }
            }
            var B = (double[,])A.Clone();
            // start with the permutation vector as A simple count - this is equivalent to an identity permutation matrix
            permutationVector = Enumerable.Range(0, length).ToArray();
            // normalize row 0
            for (var i = 0; i < length; i++)
            {
                // call this function to see if A row swap is necessary. If robustReorder, then it is likely
                // that A different row will be chosen even if this one is good. 
                if (!findAndPivotRows(B, permutationVector, lastZeroIndices, i, length, robustReorder))
                    // the reorder function only returns false, when robustReorder is false, and the simpler/quicker
                    // approach did not work. So, the whole process is restarted with robustReorder set to true.
                    // this will only recurse once (essentially just reducing duplicate code with this recursion.
                    return LUDecomposition(A, out permutationVector, length, true, lastZeroIndices);

                // continue with the main body of Crout's LU decomposition approach
                var pI = permutationVector[i];
                for (var j = i; j < length; j++)
                {
                    var pJ = permutationVector[j];
                    // do A column of L
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum -= B[pJ, k] * B[permutationVector[k], i];
                    B[pJ, i] += sum;
                }
                for (var j = i + 1; j < length; j++)
                {
                    // do A row of U
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += B[pI, k] * B[permutationVector[k], j];
                    B[pI, j] = (-sum + B[pI, j]) / B[pI, i];
                }
            }
            return B;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool findAndPivotRows(double[,] B, int[] permutationVector, List<int>[] lastZeroIndices, int i,
            int length, bool robustReorder = false)
        {
            // if robustReorder is false, then this whole reorder process may be skipped if the diagonal is nonzero.
            if (!robustReorder && !IsNegligible(B[permutationVector[i], i])) return true;

            // the following 13 lines chose the subsequent row that has A nonzero candidate for this row's 
            // diagonal and has the most zeroes that are farthest along in the row. This metric is essentially,
            // (num of remaining zeroes in row)*(the average position of zeroes in this row). Multiplying these
            // two results in simply summing the positions of the remaining zeroes in the row. So, for A pair of rows:
            // [0 3 4 0 0 0 1 7]
            // [6 4 3 9 0 0 4 0] 
            // for the third position (i = 2), the first row would receive A score of 12 (3 + 4 + 5) while the second
            // row would get A score of 16 (4 + 5 + 7). This would mean use the second row first!
            // Is this A wacky idea? I'm not sure. It is all my own, but likely explored somewhere in the literature. 
            // Such A heuristic is not uncommon. In lieu of search the n! ways to recombine the rows, this is A shortcut
            // that seems to lead to some robustness. It is intended to be A quick way to avoid further problems in later row swaps.
            var newI = -1;
            var indexOfLargestSum = -1;
            for (int j = i + 1; j < length; j++)
            {
                if (!IsNegligible(B[permutationVector[j], i]))
                {
                    var sumOfColumnsWhereZeroesExist = lastZeroIndices[permutationVector[j]].Sum(x => x > i ? x : 0);
                    if (sumOfColumnsWhereZeroesExist >= indexOfLargestSum)
                    {
                        indexOfLargestSum = sumOfColumnsWhereZeroesExist;
                        newI = j;
                    }
                }
            }
            if (newI == -1)
            {   // if there was no change to newI, then we have failed for the non robust case. Return false, and let the 
                // main LU decomp function start the robust approach
                if (!robustReorder) return false;
                // getting no change in newI for robustReorder is not necessarily A problem (it will happen in every
                // last row) if the diagonal is nonnegligible. If it is - then we got A problem...
                if (IsNegligible(B[permutationVector[i], i]))
                    throw new ArithmeticException(
                        "A appears to be A singular matrix. The LU Decomposition is not possible to complete.");
                return true;
            }
            var temp = permutationVector[i];
            permutationVector[i] = permutationVector[newI];
            permutationVector[newI] = temp;
            return true;
        }
        /// <summary>
        /// Determines whether the specified x is negligible (|x| lte 1e-15).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns><c>true</c> if the specified x is negligible; otherwise, <c>false</c>.</returns>
        public static bool IsNegligible(this double x, double optionalTolerance = DefaultEqualityTolerance)
        {
            return Math.Abs(x) <= optionalTolerance;
        }


        /// <summary>
        /// Returns the Cholesky decomposition of A in A new matrix. The new matrix is A lower triangular matrix, and
        /// the diagonals are the D matrix in the L-D-LT formulation. To get the L-LT format.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="NoSeparateDiagonal">if set to <c>true</c> [no separate diagonal].</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix cannot be inverted. Can only invert square matrices.</exception>
        /// <exception cref="ArithmeticException">Matrix cannot be inverted. Can only invert square matrices.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CholeskyDecomposition(this double[,] A, out double[,] LUMatrix, bool NoSeparateDiagonal = false)
        {
            LUMatrix = (double[,])A.Clone();
            var length = A.GetLength(0);
            if (length != A.GetLength(1)) return false;
            // throw new ArithmeticException("Cholesky Decomposition can only be determined for square matrices.");

            for (var i = 0; i < length; i++)
            {
                double sum;
                for (var j = 0; j < i; j++)
                {
                    sum = 0.0;
                    for (int k = 0; k < j; k++)
                        sum += LUMatrix[i, k] * LUMatrix[j, k] * LUMatrix[k, k];
                    if (LUMatrix[j, j] == 0.0) return false;
                    LUMatrix[i, j] = (LUMatrix[i, j] - sum) / LUMatrix[j, j];
                }
                sum = 0.0;
                for (int k = 0; k < i; k++)
                    sum += LUMatrix[i, k] * LUMatrix[i, k] * LUMatrix[k, k];
                LUMatrix[i, i] -= sum;
                for (int j = i + 1; j < length; j++)
                    LUMatrix[i, j] = 0.0;
            }
            if (NoSeparateDiagonal)
                for (int i = 0; i < length; i++)
                {
                    if (LUMatrix[i, i] < 0)
                        return false;
                    //throw new ArithmeticException("Cannot complete L-LT Cholesky Decomposition due to indefinite matrix (must be positive semidefinite).");
                    LUMatrix[i, i] = Math.Sqrt(LUMatrix[i, i]);
                }
            return true;
        }


    }
}