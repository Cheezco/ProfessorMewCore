using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Extensions.Raid
{
    public static class RaidBenchExtensions
    {
        public static string GetConvertedDPS(this IRaidBench bench, bool applyScale = false)
        {
            int dps = applyScale ? (int)(bench.DPS * bench.Scale) : bench.DPS;
            if (dps < 1000)
            {
                return dps.ToString();
            }
            if (dps < 1000000)
            {
                return (dps / 1000.0).ToString() + "K";
            }

            return ">=1M";
        }
    }
}
