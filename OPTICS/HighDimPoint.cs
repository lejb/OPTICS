using System;
using System.Collections.Generic;
using System.Linq;

namespace OPTICS.Clustering.Core
{
    public class HighDimPoint
    {
        private List<double> dimensionValues;

        public IEnumerable<double> AllValues => dimensionValues;

        public int Dim => dimensionValues.Count;

        public double this[int index] { get => dimensionValues[index]; set => dimensionValues[index] = value; }

        public HighDimPoint() => dimensionValues = new List<double>();

        public HighDimPoint(int dim) => dimensionValues = new List<double>(dim);

        public HighDimPoint(IEnumerable<double> values) => dimensionValues = values.ToList();

        public HighDimPoint(params double[] values) => dimensionValues = values.ToList();
    }

    public static class Distance
    {
        public static Func<HighDimPoint, HighDimPoint, double> EuclideanSquaredDistance
        {
            get => (a, b) =>
            {
                double sum = 0.0;

                for (int i = 0; i < a.Dim; i++)
                {
                    sum += Math.Pow(a[i] - b[i], 2);
                }

                return sum;
            };
        }

        public static Func<HighDimPoint, HighDimPoint, double> EuclideanDistance
        {
            get => (a, b) => Math.Sqrt(EuclideanSquaredDistance(a, b));
        }
    }
}
