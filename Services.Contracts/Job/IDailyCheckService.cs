using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Job
{
    public interface IDailyCheckService
    {
        Task ProcessDailyRemindersAsync();
    }
}
