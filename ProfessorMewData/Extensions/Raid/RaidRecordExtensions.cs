using Microsoft.EntityFrameworkCore;
using ProfessorMewData.Contexts;
using ProfessorMewData.Enums.Raid;
using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Extensions.Raid
{
    public static class RaidRecordExtensions
    {
        public static string GetConvertedDPS(this IRaidRecord record)
        {
            if(record.DPS < 1000)
            {
                return record.DPS.ToString();
            }
            if(record.DPS < 1000000)
            {
                return (record.DPS / 1000.0).ToString() + "K";
            }

            return ">=1M";
        }

        public static void UpdateRecordStatuses(this ICollection<IRaidRecord> records, ICollection<IRaidBench> benches)
        {
            if (records is null || benches is null)
            {
                return;
            }

            foreach (var record in records)
            {
                UpdateRecordStatus(record, benches);
            }
        }

        public static async Task UpdateRecordStatusesAsync(this ICollection<IRaidRecord> records, IAsyncEnumerable<IRaidBench> benches)
        {
            if(records is null || benches is null)
            {
                return;
            }

            List<IRaidBench> requiredBenches = new List<IRaidBench>();

            await foreach(var bench in benches)
            {
                if (records.Any(x => x.Class == bench.Class &&
                    x.Specialization == bench.Specialization &&
                    x.Role == bench.Role))
                {
                    requiredBenches.Add(bench);
                }
            }

            foreach(var record in records)
            {
                UpdateRecordStatus(record, requiredBenches);
            }
        }

        public static void UpdateRecordStatus(this IRaidRecord record, ICollection<IRaidBench> benches)
        {
            if(record is null)
            {
                return;
            }
            if(benches is null || benches.Count == 0)
            {
                record.Status = BenchStatus.Unkown;
                return;
            }

            var bench = benches.FirstOrDefault(x => x.Class == record.Class &&
                x.Specialization == record.Specialization &&
                x.Role == record.Role);
            if(bench is null)
            {
                record.Status = BenchStatus.Unkown;
                return;
            }

            if(record.DPS < Math.Floor(bench.DPS * bench.Scale) || record.BoonUptime < bench.BoonUptime)
            {
                record.Status = BenchStatus.Failed;
                return;
            }

            record.Status = BenchStatus.Passed;
        }
    }
}
