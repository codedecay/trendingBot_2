using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trendingBot2
{
    /// <summary>
    /// Class including all the regression-related code.
    /// At the moment, only 2nd-degree polynomial fits are being considered
    /// </summary>
    public class CurveFitting
    {
        //Method performing the 2nd degree polynomial regression (only one being considered at the moment)
        public PolCurve performPolRegress(CombValues xValues, CombValues yValues)
        {
            PolCurve curCurve = new PolCurve();
            try
            {
                curCurve.xValues = xValues;
                curCurve.yValues = yValues;

                //Getting the "Gauss coefficients", that is: relying on least squares to calculate the values of the matrix to be solved by Gauss-Jordan
                Coefficients curCoeffs = new Coefficients();
                Coefficients.GaussJordanCoeff curGauss = curCoeffs.getGaussJordanCoeffs(xValues, yValues);

                //Loops iterating through all the "Gauss coefficients" and performing the operations required by the Gauss-Jordan elimination
                for (int i = 0; i < 3; i++) 
                {
                    for (int i2 = 0; i2 < 3; i2++) 
                    {
                        if (i != i2)
                        {
                            double factor = curGauss.a[i, i] == 0.0 ? 0.0 : -1.0 * curGauss.a[i2, i] / curGauss.a[i, i];

                            for (int i3 = 0; i3 < 3; i3++)
                            {
                                curGauss.a[i2, i3] = factor * curGauss.a[i, i3] + curGauss.a[i2, i3];
                            }
                            curGauss.b[i2] = factor * curGauss.b[i] + curGauss.b[i2]; 
                        }
                    }
                }

                //A,B,C resulting coefficients of the fit. Example: result = A + B*x + C*x^2
                curCurve.coeffs.A = curGauss.a[0, 0] == 0.0 ? 0.0 : curGauss.b[0] / curGauss.a[0, 0];
                curCurve.coeffs.B = curGauss.a[1, 1] == 0.0 ? 0.0 : curGauss.b[1] / curGauss.a[1, 1];
                curCurve.coeffs.C = curGauss.a[2, 2] == 0.0 ? 0.0 : curGauss.b[2] / curGauss.a[2, 2];

            }
            catch
            {
                curCurve = null; //In case of dealing with too big numbers
            }

            return curCurve;
        }
    }

    /// <summary>
    /// Class including all the code required to calculate the "Gauss coefficients", that is: the values resulting from applying least squares regression
    /// to the inputs; stored in arrays emulating the corresponding matrix which will be solved by Gauss-Jordan elimination
    /// </summary>
    public class Coefficients
    {
        //Method returning the "Gauss coefficients" associated with the input values
        public GaussJordanCoeff getGaussJordanCoeffs(CombValues xValues, CombValues yValues)
        {
            LeastSquaresCoeff curLeastSquares = new LeastSquaresCoeff();

            for (int i = 0; i < xValues.values.Count; i++)
            {
                double curX2 = xValues.values[i].value * xValues.values[i].value;

                curLeastSquares.sumX1 = curLeastSquares.sumX1 + xValues.values[i].value;
                curLeastSquares.sumX2 = curLeastSquares.sumX2 + curX2;
                curLeastSquares.sumX12 = curLeastSquares.sumX12 + (xValues.values[i].value * curX2);
                curLeastSquares.sumX1Y = curLeastSquares.sumX1Y + (xValues.values[i].value * yValues.values[i].value);
                curLeastSquares.sumX22 = curLeastSquares.sumX22 + (curX2 * curX2);
                curLeastSquares.sumX2Y = curLeastSquares.sumX2Y + (curX2 * yValues.values[i].value);
                curLeastSquares.sumY = curLeastSquares.sumY + yValues.values[i].value;
            }

            //Coeffs of the associated matrix to be solved by Gauss-Jordan elimination (i.e., ordered as displayed in the comment below)
            GaussJordanCoeff curGaussJordan = new GaussJordanCoeff();
            curGaussJordan.a[0, 0] = xValues.values.Count;
            curGaussJordan.a[0, 1] = curLeastSquares.sumX1;
            curGaussJordan.a[0, 2] = curLeastSquares.sumX2;
            curGaussJordan.a[1, 0] = curLeastSquares.sumX1;
            curGaussJordan.a[1, 1] = curLeastSquares.sumX2;
            curGaussJordan.a[1, 2] = curLeastSquares.sumX12;
            curGaussJordan.a[2, 0] = curLeastSquares.sumX2;
            curGaussJordan.a[2, 1] = curLeastSquares.sumX12;
            curGaussJordan.a[2, 2] = curLeastSquares.sumX22;

            curGaussJordan.b[0] = curLeastSquares.sumY;
            curGaussJordan.b[1] = curLeastSquares.sumX1Y;
            curGaussJordan.b[2] = curLeastSquares.sumX2Y;

            return curGaussJordan;
        }

        /// <summary>
        /// Class storing all the variables required by the least squares calculations. They will be later stored in arrays emulating the matrix form expected by Gauss-Jordan
        /// </summary>
        private class LeastSquaresCoeff
        {
            public double sumX1; 
            public double sumX2;
            public double sumX12;
            public double sumX1Y;
            public double sumX22;
            public double sumX2Y;
            public double sumY;
        }

        /// <summary>
        /// Class storing the values resulting from the least squares regression in the matrix form expected by Gauss-Jordan, that is:
        /// a[0, 0]   a[0, 1]  a[0, 2]  | b[0]
        /// a[1, 0]   a[1, 1]  a[1, 2]  | b[1]
        /// a[2, 0]   a[2, 1]  a[2, 2]  | b[2]
        /// </summary>
        public class GaussJordanCoeff
        {
            public double[,] a;
            public double[] b;

            public GaussJordanCoeff()
            {
                a = new double[3, 3];
                b = new double[3];
            }
        }
    }

    /// <summary>
    /// Class defining the given (2nd-degree-polynomial) curve, resulting from the regression. That is, of the form: y = A + B*x + C*x^2
    /// </summary>
    public class PolCurve
    {
        public PolCoeffs coeffs; 
        public CombValues xValues;
        public CombValues yValues;

        public PolCurve()
        {
            coeffs = new PolCoeffs();
            xValues = new CombValues();
            yValues = new CombValues();
        }
    }

    /// <summary>
    /// Class defining the (2nd-degree-polynomial) coefficients, that is: A, B & C from y = A + B*x + C*x^2
    /// </summary>
    public class PolCoeffs
    {
        public double A;
        public double B;
        public double C;
    }
}
