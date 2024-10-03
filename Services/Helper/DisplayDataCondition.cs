using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class DisplayDataCondition
    {
        //FOR GENERATING WHERE AND ORDER BY CONDITION 
        //TO HANDLE IF CLIENT SIDE PROPERTY NAME IS DIFFERENT WITH TABLE COLUMN NAME
        //TO HANDLE AMBIGIOUS COLUMN

        static Dictionary<string, string> keyValuePair;

        public static Dictionary<string, string> GenerateDataTablesAndMaterialTablePair(List<Column> dataTablesCol)
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

        public static string GenerateWhereCondition(GlobalParam param)
        {
            keyValuePair = GenerateDataTablesAndMaterialTablePair(param.columns);

            string whereCondition = string.Empty;
            var dtSearchWithValue = param.columns.Where(col => col.searchable && !string.IsNullOrWhiteSpace(col.Search.value)).ToList();

            //whereCondition += "1=1 ";

            //search by each column in data tables
            whereCondition += GenerateDatatablesSearchWhereCondition(dtSearchWithValue);
            if (param.search != null)
            {
                //global search in datatable
                whereCondition += GenerateGlobalSearchTermWhereCondition(param.columns, param.search.value);
            }
            return whereCondition;
        }

        public static string GenerateDatatablesSearchWhereCondition(List<Column> dataTablesCol)
        {
            string dataTablesSearchCondition = string.Empty;

            foreach (var col in dataTablesCol.Where(s => s.searchable))
            {
                if (col.data == "No") continue;
                if (!keyValuePair.ContainsKey(col.data)) continue;

                dataTablesSearchCondition += string.Format("AND ((ISNULL('{0}','') = '') OR ({1} LIKE '%{0}%' )) ", col.Search.value, keyValuePair[col.data]);
            }

            return dataTablesSearchCondition;
        }

        public static string GenerateGlobalSearchTermWhereCondition(List<Column> dataTablesCol, string globalSearchTerm)
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
                    whereCond = string.Format("(ISNULL('{0}','') = '' OR (CHARINDEX('{0}', LOWER({1})) > 0)) ", globalSearchTerm, keyValuePair[col.data]);

                    globalSearchTermWhereCondition += globalSearchTermWhereCondition == "AND (" ? whereCond : "OR" + whereCond;
                }
            }

            globalSearchTermWhereCondition += ")";

            return globalSearchTermWhereCondition;
        }

        public static string GenerateOrderByCondition(List<Order> dataTablesCol)
        {
            string orderByCondition = string.Empty;
            if (dataTablesCol != null)
            {
                if (dataTablesCol.Count > 0)
                {
                    string colorderBy = " order by ";
                    foreach (var col in dataTablesCol)
                    {
                        colorderBy += string.Format("{0} {1}", keyValuePair.ElementAt(col.column == 0 ? 0 : col.column - 1).Value, col.dir);
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
