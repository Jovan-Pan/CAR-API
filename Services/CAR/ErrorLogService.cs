using Contracts.Repository;
using Entities;
using Entities.CAR;
using Entities.ParamRequest;
using Services.Contracts.CAR;
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

        public async Task<PagedResponse> ShowDataErrorLog(ErrorLogParam param)
        {
            try
            {
                var skip = (param.PageNumber - 1) * param.PageSize;
                keyValuePair = GenerateDataTablesAndMaterialTablePair(param.columns);

                var whereCondition = GenerateWhereCondition(param);
                var orderByCondition = GenerateOrderByCondition(param.order);
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
                ErrorLogData.ErrDescription = exceptionMessage + " : " + fullerr;
                ErrorLogData.ErrType = exdb.GetType().Name.ToString();
                ErrorLogData.ErrSource = exdb.StackTrace.ToString();
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

        //FOR GENERATING WHERE AND ORDER BY CONDITION 
        //TO HANDLE IF CLIENT SIDE PROPERTY NAME IS DIFFERENT WITH TABLE COLUMN NAME
        //TO HANDLE AMBIGIOUS COLUMN

        private static Dictionary<string, string> GenerateDataTablesAndMaterialTablePair(List<Column> dataTablesCol)
        {
            var keyValuePair = new Dictionary<string, string>();

            foreach (var col in dataTablesCol.Where(c => c.searchable))
            {
                if (col.data != null)
                {
                    switch (col.data.ToLower())
                    {

                        case "plant":
                            keyValuePair.Add(col.data, "plant");
                            break;
                        default:
                            keyValuePair.Add(col.data, col.data);
                            break;
                    }
                }
            }

            return keyValuePair;
        }

        private static string GenerateWhereCondition(ErrorLogParam param)
        {

            string whereCondition = string.Empty;
            var dtSearchWithValue = param.columns.Where(col => col.searchable && !string.IsNullOrWhiteSpace(col.Search.value)).ToList();

            whereCondition += "1=1 ";

            //search by each column in data tables
            whereCondition += GenerateDatatablesSearchWhereCondition(dtSearchWithValue);
            if (param.search != null)
            {
                //global search in datatable
                whereCondition += GenerateGlobalSearchTermWhereCondition(param.columns, param.search.value);
            }
            return whereCondition;
        }

        private static string GenerateDatatablesSearchWhereCondition(List<Column> dataTablesCol)
        {
            string dataTablesSearchCondition = string.Empty;

            foreach (var col in dataTablesCol.Where(s => s.searchable))
            {
                if (col.data == "No") continue;
                if (!keyValuePair.ContainsKey(col.data)) continue;

                if (col.data == "CreatedDate")
                {
                    dataTablesSearchCondition += string.Format("  AND (format({1},'yyyy-MM-dd HH:mm:ss') LIKE '%{0}%') ", col.Search.value, keyValuePair[col.data]);
                }
                else if (col.data == "UpdatedDate")
                {
                    dataTablesSearchCondition += string.Format("  AND (format({1},'yyyy-MM-dd HH:mm:ss') LIKE '%{0}%') ", col.Search.value, keyValuePair[col.data]);
                }
                else if (col.data == "JobReqID")
                {
                    dataTablesSearchCondition += string.Format("AND ((ISNULL('{0}','') = '') OR (JRD.{1} LIKE '%{0}%' )) ", col.Search.value, keyValuePair[col.data]);
                }
                else
                {
                    dataTablesSearchCondition += string.Format("AND ((ISNULL('{0}','') = '') OR ({1} LIKE '%{0}%' )) ", col.Search.value, keyValuePair[col.data]);
                }
            }

            return dataTablesSearchCondition;
        }

        private static string GenerateGlobalSearchTermWhereCondition(List<Column> dataTablesCol, string globalSearchTerm)
        {
            if (string.IsNullOrWhiteSpace(globalSearchTerm)) return string.Empty;

            string globalSearchTermWhereCondition = "AND (";

            foreach (var col in dataTablesCol)
            {
                if (col.data != null)
                {
                    if (!keyValuePair.ContainsKey(col.data)) continue;
                    if (col.data == "No") continue;

                    string whereCond = "";
                    if (col.data == "JobReqID")
                    {
                        whereCond = string.Format("(ISNULL('{0}','') = '' OR (CHARINDEX('{0}', LOWER(JRD.{1})) > 0)) ", globalSearchTerm, keyValuePair[col.data]);
                    }
                    else
                    {
                        whereCond = string.Format("(ISNULL('{0}','') = '' OR (CHARINDEX('{0}', LOWER({1})) > 0)) ", globalSearchTerm, keyValuePair[col.data]);
                    }

                    globalSearchTermWhereCondition += globalSearchTermWhereCondition == "AND (" ? whereCond : "OR" + whereCond;
                }
            }

            globalSearchTermWhereCondition += ")";

            return globalSearchTermWhereCondition;
        }

        private static string GenerateOrderByCondition(List<Order> dataTablesCol)
        {
            string orderByCondition = string.Empty;
            if (dataTablesCol != null)
            {
                if (dataTablesCol.Count > 0)
                {
                    foreach (var col in dataTablesCol)
                    {
                        //if (col.column == 1)
                        //{
                        //    orderByCondition += string.Format("{0} {1}", keyValuePair.ElementAt(0).Value, col.dir);
                        //}
                        //else if (col.column > 1)
                        //{

                        //}

                        string colorderBy = string.Format("{0} {1}", keyValuePair.ElementAt(col.column == 0 ? 0 : col.column - 1).Value, col.dir);
                        orderByCondition += string.IsNullOrWhiteSpace(orderByCondition)
                            ? colorderBy
                            : "," + colorderBy;
                    }
                }
            }

            return string.IsNullOrWhiteSpace(orderByCondition) ? (keyValuePair.Count == 0 ? string.Empty : keyValuePair.ElementAt(1).Value) : orderByCondition;
        }
    }
}
