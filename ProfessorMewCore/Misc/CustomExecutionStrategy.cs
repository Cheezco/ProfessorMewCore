using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Misc
{
    public class CustomExecutionStrategy : ExecutionStrategy
    {
        protected override bool ShouldRetryOn(Exception exception)
        {
            return exception.GetType() == typeof(DbUpdateException);
        }

        public CustomExecutionStrategy(ExecutionStrategyDependencies dependencies) : base(dependencies, 20, TimeSpan.FromSeconds(30))
        {

        }

        public CustomExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) : base(context, maxRetryCount, maxRetryDelay)
        {

        }
    }
}
