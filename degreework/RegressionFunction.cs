using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace degreework
{
    public interface RegressionFunction
    {

        Func<double, double> Function();

        string FunctionString();

        int ParametersCount();

    }
}
