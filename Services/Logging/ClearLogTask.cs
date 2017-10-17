using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Tasks;

namespace Services.Logging
{
    public partial class ClearLogTask : ITask
    {
        private readonly ILogger _logger;

        public ClearLogTask(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.ClearLog();
        }
    }
}
