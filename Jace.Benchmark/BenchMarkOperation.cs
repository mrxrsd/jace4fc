using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jace.Benchmark
{
    public class BenchMarkOperation<T>
    {
        public string Formula { get; set; }
        public BenchmarkMode Mode { get; set; }
        public Func<ICalculationEngine<T>, string, Dictionary<string, double>, TimeSpan> BenchMarkDelegate { get; set; }
        public Dictionary<string, T> VariableDict { get; set; }
    }
}
