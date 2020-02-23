using System;
using System.Text;

namespace DepersonalizationApp.Helpers
{
    public static class SqlQueryHelper
    {
        public static string GetQueryOfActivityGuidsByRegardingObjectIds(string tableName, Guid[] regardingObjectIds)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select distinct item.ActivityId");
            sb.AppendLine($" from {tableName} as item");
            var where = GetPartOfQueryWhereIn("item.RegardingObjectId", regardingObjectIds);
            sb.AppendLine(where);
            return sb.ToString();
        }

        public static string GetPartOfQueryWhereIn<T>(string column, T[] items)
        {
            var sb = new StringBuilder();
            sb.AppendLine($" where {column} in (");
            for (int i = 0; i < items.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"'{items[i]}'");
                }
                else
                {
                    sb.Append($", '{items[i]}'");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}