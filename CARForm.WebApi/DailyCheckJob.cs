using Contracts.Repository; 
using Entities.ParamRequest;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Services.Contracts.CAR;

namespace WebApi
{
    public class DailyCheckJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public static string GetMailWStatus(string issueStatus)
        {
            return issueStatus switch
            {
                // === SUBMITED Flow ===
                "SUBMITED" => "SUBMITED",
                "RE-SUBMIT" => "RE-SUBMITED",
                "RE-SUBMIT-APPEAL" => "RE-SUBMITED",
                "SUBMITED-REJECT" => "ISSUER-MGR-REJECT",
                "SUBMITED-APPEAL" => "RECEIVER-PIC-APPEAL",

                // === OPEN Flow ===
                "OPEN" => "OPEN",
                "OPEN-REJECT" => "PDA-REJECT",
                "OPEN-APPEAL" => "OPEN",

                // === ISSUED Flow ===
                "ISSUED" => "ISSUED",
                "ISSUED-REJECT" => "RECEIVER-PIC-REJECT",
                "ISSUED-APPEAL" => "ISSUED",
                "ISSUED-REJECTED" => "ANALYZE",
                "ISSUED-REJECTED (WA)" => "ANALYZE",

                // === PDA Flow ===
                "PDA-DECISION" => "PDA-DECISION",
                "PDA-DECISION-APPEAL" => "PDA-DECISION",

                // === ACTION Flow ===
                "ACTION-ISSUED" => "ACTION-ISSUED",

                // === ANALYZE Flow ===
                "ANALYZE" => "ANALYZE",
                "ANALYZE-MANAGEMENT" => "REVIEW-MANAGEMENT",

                // === REVIEW Flow ===
                "REVIEW" => "REVIEW",

                // === COMPLETE Flow ===
                "COMPLETE" => "COMPLETE",
                "NOT EFFECTIVE" => "ISSUER-REVIEW-REJECT",

                // === REJECT/VOID Flow ===
                "REJECT" => "PDA-REVIEW-REJECT",
                "VOID" => "ISSUER-MGR-VOID",

                // === DRAFT/NEW (fallback ke DAILY_REMINDER) ===
                "DRAFT-SUBMIT" => "DAILY_REMINDER",
                "NEW" => "DAILY_REMINDER",

                // === Default fallback ===
                _ => "DAILY_REMINDER"
            };
        }


        public DailyCheckJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataManager = scope.ServiceProvider.GetRequiredService<IDataManager>();
                var mailService = scope.ServiceProvider.GetRequiredService<ISendMailSettingService>();

                try
                {
                    var allIssues = await dataManager.IFR.GetAllIssuesForProcessing();

                    if (allIssues != null && allIssues.Any())
                    {
                        foreach (var issue in allIssues)
                        {

                            var mailParam = new IssueSubmissionParameters
                            {
                                UserPlant = issue.Plant,
                                FormNumber = issue.FormNo,
                                FormType = issue.FormType,
                                Dept = issue.Dept,
                                VendorCode = issue.VendorCode,
                                VendorDesc = issue.VendorDesc,
                                IssueStatus = issue.Status,
                                Product = issue.Product,
                                MaterialCode = issue.MaterialCode,
                                MaterialDesc = issue.MaterialDescription,
                                DetectionDate = issue.DetectionDate,
                                NCCategory = issue.NCCategory,
                                NCDescription = issue.NCDescription,
                                NcQty = issue.NcQty,
                                NcRatio = issue.NcRatio,
                                SamplingCheck = issue.SamplingCheck,
                                AffectedCavity = issue.AffectedCavity,
                                TttlQty = issue.TttlQty,

                                mailactionType = "DAILY_REMINDER",
                                mailWStatus = GetMailWStatus(issue.Status),
                                sendmailUserAction = "System Reminder",
                                UserName = "System Administrator",
                                UserId = "SYSTEM"
                            };

                            var emailResult = await mailService.sendemail(mailParam);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Quartz] Job Error: {ex.Message}");
                }
            }
        }
    }
}