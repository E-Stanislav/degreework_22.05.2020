using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace degreework
{

    [Serializable]
    class Waveform: RegressionFunction
    {

        private double y0;
        private double a;
        private double b;
        private double c;
        private double d;

        public Waveform(double y0, double a, double b, double c, double d)
        {
            this.y0 = y0;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public Func<double, double> Function()
        {
            return (x) => y0 + a * Math.Exp(-x / d) * Math.Sin(2 * Math.PI * x / b + c);
        }

        public string FunctionString()
        {
            return y0 + " + " + a + " * exp(-x/" + d + " * sin(2*Pi*x/" + b + " " + c + ")";
        }

        public int ParametersCount()
        {
            return 5;
        }

    }

}
