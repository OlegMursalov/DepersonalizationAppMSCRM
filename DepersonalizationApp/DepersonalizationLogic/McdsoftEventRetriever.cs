﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class McdsoftEventRetriever : Base<Guid>
    {
        public McdsoftEventRetriever(SqlConnection sqlConnection) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select ev.mcdsoft_eventId");
            sb.AppendLine(" from dbo.mcdsoft_event as ev");
            sb.AppendLine(" where ev.mcdsoft_eventId in (select evIn.mcdsoft_eventId");
            sb.AppendLine("  from dbo.mcdsoft_event as evIn");
            sb.AppendLine("  order by evIn.CreatedOn desc");
            sb.AppendLine("  offset 0 rows");
            sb.AppendLine("  fetch next 500 rows only)");
            _retrieveSqlQuery = sb.ToString();
        }

        public IEnumerable<Guid> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override Guid ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return (Guid)sqlReader.GetValue(0);
        }
    }
}