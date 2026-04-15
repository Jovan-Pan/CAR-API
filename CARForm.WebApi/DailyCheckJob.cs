using Contracts.Repository; 
using Entities.ParamRequest;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Services.Contracts.CAR;
using Services.Contracts.Job;

namespace WebApi
{
    public class DailyCheckJob : IJob
    {
        private readonly IDailyCheckService _dailyCheckService;

        public DailyCheckJob(IDailyCheckService dailyCheckService)
        {
            _dailyCheckService = dailyCheckService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _dailyCheckService.ProcessDailyRemindersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Job Error: {ex.Message}");
                throw; // Re-throw agar Quartz tahu job failed
            }
        }
    }
}
