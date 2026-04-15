using Contracts.Repository;
using Entities.ParamRequest;
using Services.Contracts.CAR;
using Services.Contracts.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Job
{
    public class DailyCheckService : IDailyCheckService
    {
        private readonly IDataManager _dataManager;
        private readonly ISendMailSettingService _mailService;

        public DailyCheckService(IDataManager dataManager, ISendMailSettingService mailService)
        {
            _dataManager = dataManager;
            _mailService = mailService;
        }

        public async Task ProcessDailyRemindersAsync()
        {
            var allIssues = await _dataManager.IFR.GetAllIssuesForProcessing();

            if (allIssues == null || !allIssues.Any()) return;

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

                await _mailService.sendemail(mailParam);
            }
        }

        private static string GetMailWStatus(string issueStatus)
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
    }
}

