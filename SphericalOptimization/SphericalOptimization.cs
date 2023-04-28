using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OptimizationToolbox;
using StarMathLib;
using System.Threading.Tasks;

namespace SphericalOptimization
{
    public class SphericalOptimization : abstractOptMethod
    {
        #region Fields

        /// <summary>
        /// The chi
        /// </summary>
        private readonly double chi = 2;
        /// <summary>
        /// The initialize new point addition
        /// </summary>
        private readonly double initNewPointAddition = 0.5;
        /// <summary>
        /// The initialize new point percentage
        /// </summary>
        private readonly double initNewPointPercentage = 0.01;
        /// <summary>
        /// The psi
        /// </summary>
        private readonly double psi = 0.5;
        /// <summary>
        /// The rho
        /// </summary>
        private readonly double rho = 1;
        /// <summary>
        /// The sigma
        /// </summary>
        private readonly double sigma = 0.5;


        public SortedList<double, double[]> sortedBest { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NelderMead"/> class.
        /// </summary>
        public SphericalOptimization()
        {
            x = new double[2];
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SphericalOptimization"/> class.
        /// </summary>
        /// <param name="rho">The rho.</param>
        /// <param name="chi">The chi.</param>
        /// <param name="psi">The psi.</param>
        /// <param name="sigma">The sigma.</param>
        /// <param name="initNewPointPercentage">The initialize new point percentage.</param>
        /// <param name="initNewPointAddition">The initialize new point addition.</param>
        public SphericalOptimization(double rho, double chi, double psi, double sigma, double initNewPointPercentage = double.NaN,
                          double initNewPointAddition = double.NaN)
            : this()
        {
            this.rho = rho;
            this.chi = chi;
            this.psi = psi;
            this.sigma = sigma;
            if (!double.IsNaN(initNewPointPercentage)) this.initNewPointPercentage = initNewPointPercentage;
            if (!double.IsNaN(initNewPointAddition)) this.initNewPointAddition = initNewPointAddition;
        }

        #endregion

        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
        protected override double run(out double[] xStar)
        {
            var icosaEdgeScores = new double[30];
            // first evaluate all the 30 faces of a triacontahedron
#if RELEASE
            Parallel.For(0, icosaEdgeScores.Length, i =>
#else
            for (int i = 0; i < PlatonicConstants.TriacontahedronDirections.Length; i++)
#endif
            {
                icosaEdgeScores[i] = calc_f(PlatonicConstants.TriacontahedronDirections[i]);

            }
#if RELEASE
            );
#endif
            // second, find icosahedron faces that are not worth pursuing
            var icosaFacesToIgnore = new HashSet<int>();
            for (int i = 0; i < PlatonicConstants.IcosaEdgeToFaces.Length; i++)
            {
                var leftFaceIndex = PlatonicConstants.IcosaEdgeToFaces[i][0];
                var rightFaceIndex = PlatonicConstants.IcosaEdgeToFaces[i][1];
                var otherLeftTriaContaIndices = FindTriacontahedronValuesExceptCommon(leftFaceIndex, i, icosaEdgeScores);
                var otherRightTriaContaIndices = FindTriacontahedronValuesExceptCommon(rightFaceIndex, i, icosaEdgeScores);
                if (otherLeftTriaContaIndices.Item1 >= otherRightTriaContaIndices.Item2)
                    icosaFacesToIgnore.Add(leftFaceIndex);
                if (otherRightTriaContaIndices.Item1 >= otherLeftTriaContaIndices.Item2)
                    icosaFacesToIgnore.Add(rightFaceIndex);
            }
            //icosaFacesToIgnore.Clear();
            //third, complete barycentric nelder mead for icosahedron faces that are not in the ignore list
            var bestResults = new ConcurrentBag<(double, double[], int)>();
#if RELEASE
            Parallel.For(0, 20, i =>
#else

            for (int i = 0; i < 20; i++)
#endif
            {
                if (!icosaFacesToIgnore.Contains(i))
                {
                    var vertexNormals = PlatonicConstants.IcosaTriangleVertices[i]
                        .Select(j => PlatonicConstants.DodechedronDirections[j]).ToArray();
                    var thisFStar = BarycentricSphericalNelderMead(vertexNormals, PlatonicConstants.IcosaTriangleEdges[i]
                        .Select(v => icosaEdgeScores[v]).ToList(), out double[] thisXStar);
                    bestResults.Add((thisFStar, thisXStar, i));
                }
            }
#if RELEASE
            );
#endif

            sortedBest = new SortedList<double, double[]>(new optimizeSort(optimize.minimize));
            foreach (var result in bestResults)
                sortedBest.Add(result.Item1, ConvertTo3DUnitVector(result.Item2,
                    PlatonicConstants.IcosaTriangleVertices[result.Item3]
                    .Select(j => PlatonicConstants.DodechedronDirections[j]).ToArray()));
            for (int i = sortedBest.Count - 2; i >= 0; i--)
            {
                if (sortedBest.Keys[i].IsPracticallySame(sortedBest.Keys[i + 1], 1e-10) &&
                    sortedBest.Values[i].dotProduct(sortedBest.Values[i + 1]).IsPracticallySame(1.0, 1e-3))
                    sortedBest.RemoveAt(i + 1);
            }
            xStar = sortedBest.Values[0];
            return sortedBest.Keys[0];
        }

        private double BarycentricSphericalNelderMead(double[][] vertexNormals, List<double> initialFValues, out double[] thisXStar)
        {
            var vertices = new SortedList<double, double[]>(new optimizeSort(optimize.minimize));
            vertices.Add(initialFValues[0], new[] { 0.5, 0.5 });
            vertices.Add(initialFValues[1], new[] { 0.0, 0.5 });
            vertices.Add(initialFValues[2], new[] { 0.5, 0.0 });
            while (notConverged(k, numEvals, vertices.Keys[0], vertices.Values[0], vertices.Values))
            {
                #region Compute the REFLECTION POINT

                // computing the average for each variable for n variables NOT n+1
                var Xm = new double[n];
                for (var dim = 0; dim < n; dim++)
                {
                    double sumX = 0;
                    for (var j = 0; j < n; j++)
                        sumX += vertices.Values[j][dim];
                    Xm[dim] = sumX / n;
                }

                var Xr = CloneVertex(vertices.Values[n]);
                for (var i = 0; i < n; i++)
                    Xr[i] = (1 + rho) * Xm[i] - rho * Xr[i];
                var fXr = OutsideOfTriangle(Xr) ? double.PositiveInfinity :
                    calc_f(ConvertTo3DUnitVector(Xr, vertexNormals));
                SearchIO.output("x_r = " + MakePrintString(Xr), 4);
                #endregion

                #region if reflection point is better than best

                if (fXr < vertices.Keys[0])
                {
                    #region Compute the Expansion Point
                    var Xe = CloneVertex(vertices.Values[n]);
                    for (var i = 0; i < n; i++)
                        Xe[i] = (1 + rho * chi) * Xm[i] - rho * chi * Xe[i];
                    var fXe = OutsideOfTriangle(Xe) ? double.PositiveInfinity :
                    calc_f(ConvertTo3DUnitVector(Xe, vertexNormals));
                    #endregion

                    vertices.RemoveAt(n);  // remove the worst
                    if (fXe < fXr)
                    {
                        vertices.Add(fXe, Xe);
                        SearchIO.output("expand", 4);
                    }
                    else
                    {
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                }
                #endregion
                #region if reflection point is NOT better than best

                else
                {
                    #region but if it's better than second worst, still do reflect

                    if (fXr < vertices.Keys[n - 1])
                    {
                        vertices.RemoveAt(n);  // remove the worst
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                    #endregion

                    else
                    {
                        #region if better than worst, do Outside Contraction
                        if (fXr < vertices.Keys[n])
                        {
                            var Xc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xc[i] = (1 + rho * psi) * Xm[i] - rho * psi * Xc[i];
                            var fXc = OutsideOfTriangle(Xc) ? double.PositiveInfinity :
                                calc_f(ConvertTo3DUnitVector(Xc, vertexNormals));

                            if (fXc <= fXr)
                            {
                                vertices.RemoveAt(n);  // remove the worst
                                vertices.Add(fXc, Xc);
                                SearchIO.output("outside constract", 4);
                            }
                            #endregion
                            #region Shrink all others towards best
                            else
                            {
                                var newXs = new List<double[]>();
                                for (var j = n; j >= 1; j--)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                        Xs[i] = vertices.Values[0][i]
                                                    + sigma * (Xs[i] - vertices.Values[0][i]);
                                    newXs.Add(Xs);
                                    vertices.RemoveAt(j);
                                }
                                for (int j = 0; j < n; j++)
                                {
                                    var Xs = newXs[j];
                                    //if (!OutsideOfTriangle(Xs))
                                    vertices.Add(calc_f(ConvertTo3DUnitVector(Xs, vertexNormals)), Xs);
                                }
                                SearchIO.output("shrink towards best", 4);
                            }

                            #endregion
                        }
                        else
                        {
                            #region Compute Inside Contraction

                            var Xcc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xcc[i] = (1 - psi) * Xm[i] + psi * Xcc[i];
                            var fXcc = OutsideOfTriangle(Xcc) ? double.PositiveInfinity :
                                calc_f(ConvertTo3DUnitVector(Xcc, vertexNormals));

                            if (fXcc < vertices.Keys[n])
                            {
                                vertices.RemoveAt(n);  // remove the worst
                                vertices.Add(fXcc, Xcc);
                                SearchIO.output("inside contract", 4);
                            }
                            #endregion
                            #region Shrink all others towards best and flip over

                            else
                            {
                                var newXs = new List<double[]>();
                                for (var j = n; j >= 1; j--)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                        Xs[i] = vertices.Values[0][i]
                                                    + sigma * (Xs[i] - vertices.Values[0][i]);
                                    newXs.Add(Xs);
                                    vertices.RemoveAt(j);
                                }
                                for (int j = 0; j < n; j++)
                                {
                                    var Xs = newXs[j];
                                    //if (!OutsideOfTriangle(Xs))
                                    vertices.Add(calc_f(ConvertTo3DUnitVector(Xs, vertexNormals)), Xs);
                                }
                                SearchIO.output("shrink towards best and flip", 4);
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                k++;
                SearchIO.output("iter. = " + k, 1);
                SearchIO.output("Fitness = " + vertices.Keys[0], 1);
            } // END While Loop
            thisXStar = vertices.Values[0];
            var thisfStar = vertices.Keys[0];
            vertices.Clear();
            return thisfStar;
        }

        private bool OutsideOfTriangle(double[] barycentricCoords)
        {
            if (barycentricCoords[0] < 0) return true;
            if (barycentricCoords[1] < 0) return true;
            return 1 - barycentricCoords[0] - barycentricCoords[1] < 0;
        }

        private double[] ConvertTo3DUnitVector(double[] coords, double[][] vertexNormals)
        {
            var a = coords[0];
            var b = coords[1];
            var c = 1 - a - b;
            var dir = new[]
            {
                a * vertexNormals[0][0] + b * vertexNormals[1][0] + c * vertexNormals[2][0],
                a * vertexNormals[0][1] + b * vertexNormals[1][1] + c * vertexNormals[2][1],
                a * vertexNormals[0][2] + b * vertexNormals[1][2] + c * vertexNormals[2][2]
            };
            return dir.normalize();
        }

        private (double, double) FindTriacontahedronValuesExceptCommon(int faceIndex, int edgeIndexToIgnore, double[] icosaEdgeScores)
        {
            var edgeIndices = PlatonicConstants.IcosaTriangleEdges[faceIndex];
            var edgeIndex1 = edgeIndices[0] != edgeIndexToIgnore ? edgeIndices[0] : edgeIndices[1];
            var edgeIndex2 = edgeIndices[2] != edgeIndexToIgnore ? edgeIndices[2] : edgeIndices[1];
            var f1 = icosaEdgeScores[edgeIndex1];
            var f2 = icosaEdgeScores[edgeIndex2];
            if (f1 <= f2) return (f1, f2);  // so the first return number is lower or equal to the second
            return (f2, f1);
        }

        private string MakePrintString(double[] vector)
        {
            if (vector == null) return "<null>";
            var format = "{0:F" + PrintNumDecimals + "}";
            var p = "{ ";
            foreach (var d in vector)
                p += formatCell(format, d) + ",";
            p = p.Remove(p.Length - 1);
            p += " }";
            return p;
        }
        private static object formatCell(string format, double p)
        {
            var numStr = string.Format(format, p);
            numStr = numStr.TrimEnd('0');
            numStr = numStr.TrimEnd('.');
            var padAmt = ((float)(PrintCellWidth - numStr.Length)) / 2;
            numStr = numStr.PadLeft((int)Math.Floor(padAmt + numStr.Length));
            numStr = numStr.PadRight(PrintCellWidth);
            return numStr;
        }

        const int PrintCellWidth = 10;

        const int PrintNumDecimals = 3;

        /// <summary>
        /// Clones the vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>System.Double[].</returns>
        private static double[] CloneVertex(double[] vertex)
        {
            return (double[])vertex.Clone();
        }
    }
}
