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
            sb.AppendLine(" where item.RegardingObjectId in (");
            for (int i = 0; i < regardingObjectIds.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"'{regardingObjectIds[i]}'");
                }
                else
                {
                    sb.Append($", '{regardingObjectIds[i]}'");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}