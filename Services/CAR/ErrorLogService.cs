using Contracts.Repository;
using Entities;
using Entities.CAR;
using Entities.ParamRequest;
using Services.Contracts.CAR;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CAR
{
    public class ErrorLogService(IDataManager repoManager) : IErrorLogService
    {
        private readonly IDataManager _repoManager = repoManager;

        static Dictionary<string, string> keyValuePair;

        public async Task<PagedResponse> ShowDataErrorLog(GlobalParam param)
        {
            try
            {
                var skip = (param.PageNumber - 1) * param.PageSize;
                keyValuePair = DisplayDataCondition.GenerateDataTablesAndMaterialTablePair(param.columns);

                var whereCondition = DisplayDataCondition.GenerateWhereCondition(param);
                var orderByCondition = DisplayDataCondition.GenerateOrderByCondition(param.order);
                int totalRecords = 0;
                int take = param.PageSize;
                ConditionParams Cpr = new ConditionParams();
                Cpr.take = take;
                Cpr.skip = skip;
                Cpr.ExtraWhereCondition = whereCondition;
                Cpr.OrderByCondition = orderByCondition;
                var (data, totalCount) = await _repoManager.ErrorLog.ShowDataErrorLog(param, Cpr);
                var pagedList = new PagedList<dynamic>(data.ToList<dynamic>(), totalCount, param.PageNumber, param.PageSize);

                var dtaresult = new PagedResponse
                {
                    data = pagedList,
                    metaData = pagedList.MetaData
                };
                return dtaresult;
            }
            catch (Exception ex)
            {
                PagedResponse pr = new PagedResponse();
                pr.success = false;
                pr.message = ex.Message;
                return pr;
            }
        }

        public async Task<int> SaveErrorLog(Exception exdb, string userLogin)
        {
            try
            {
                string exceptionMessage = exdb.Message.ToString();
                string fullerr = exdb.StackTrace.ToString();
                string lastStackTraceLine = GetLastStackTraceLine(fullerr);

                if (fullerr.Length > 3500)
                {
                    string first3500Chars = fullerr.Substring(0, 3500);
                    fullerr = first3500Chars;
                }

                ErrorLogModel ErrorLogData = new ErrorLogModel();
                ErrorLogData.ErrDescription = exceptionMessage;
                ErrorLogData.ErrType = exdb.GetType().Name.ToString();
                ErrorLogData.ErrSource = fullerr;
                ErrorLogData.ErrUrl = lastStackTraceLine;
                ErrorLogData.AddedBy = userLogin;
                ErrorLogData.AddedOn = DateTime.Now;

                var result = await _repoManager.ErrorLog.InsertDataToTERRORLOG(ErrorLogData);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: " + ex.Message);
                return 0;
            }
        }

        public static string GetLastStackTraceLine(string stackTrace)
        {
            string[] stackTraceLines = stackTrace.Split('\n');
            if (stackTraceLines.Length > 0)
            {
                string lastLine = stackTraceLines[stackTraceLines.Length - 2].Trim(); // Subtract 2 to ignore the last empty line

                if (lastLine != null)
                {
                    if (lastLine.Length > 100)
                    {
                        string last100tesxt = GetLast100Characters(lastLine);
                        lastLine = last100tesxt;
                    }
                }
                return lastLine;
            }
            return "No stack trace information available.";
        }

        public static string GetLast100Characters(string input)
        {
            if (input.Length <= 100)
            {
                return input;
            }

            int startIndex = input.Length - 100;
            string last100Characters = input.Substring(startIndex);

            return last100Characters;
        }

        
    }
}
