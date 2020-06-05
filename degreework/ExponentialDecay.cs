using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace degreework
{

    [Serializable]
    public class ExponentialDecay: RegressionFunction
    {

        private double y0;
        private double a;
        private double b;

        public ExponentialDecay(double y0, double a, double b)
        {
            this.y0 = y0;
            this.a = a;
            this.b = b;
        }

        public Func<double, double> Function()
        {
            return (x) => y0 + a * Math.Exp(-b * x);
        }

        public string FunctionString()
        {
            return y0 + " + " + a + " * exp(-" + b + " * x)";
        }

        public int ParametersCount()
        {
            return 3;
        }

    }
}
