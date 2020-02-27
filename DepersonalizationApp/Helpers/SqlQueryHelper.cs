using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static string GetPartOfQueryWhereIn<T>(string column, IEnumerable<T> items)
        {
            var array = items.ToArray();
            var sb = new StringBuilder();
            sb.AppendLine($" where {column} in (");
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"'{array[i]}'");
                }
                else
                {
                    sb.Append($", '{array[i]}'");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static int GetOffsetNumber(string query)
        {
            var array = query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var parts = array[array.Length - 2].Trim().Split(new[] { ' ' });
            return int.Parse(parts[1]);
        }

        public static int GetFetchNumber(string query)
        {
            var array = query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var parts = array[array.Length - 1].Trim().Split(new[] { ' ' });
            return int.Parse(parts[2]);
        }

        public static string ChangeSqlQueryPagination(string query, int offset, int fetchNext)
        {
            var parts = query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            parts[parts.Length - 2] = Regex.Replace(parts[parts.Length - 2], "\\d+", offset.ToString());
            parts[parts.Length - 1] = Regex.Replace(parts[parts.Length - 1], "\\d+", fetchNext.ToString());
            return string.Join(Environment.NewLine, parts);
        }
    }
}