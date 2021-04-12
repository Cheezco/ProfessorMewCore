using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Extensions
{
    public static class DoubleExtensions
    {
        public static void Clamp(ref this double number, double min, double max)
        {
            if(number < min)
            {
                number = min;
                return;
            }
            if(number > max)
            {
                number = max;
                return;
            }
        }
    }
}
