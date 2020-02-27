using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновить мероприятия
    /// </summary>
    public class McdsoftEventUpdater : BaseUpdater<mcdsoft_event>
    {
        public McdsoftEventUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select ev.mcdsoft_eventId, ev.new_expenses, ev.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.mcdsoft_event as ev");
            sb.AppendLine(" where ev.mcdsoft_eventId in (select evIn.mcdsoft_eventId");
            sb.AppendLine("  from dbo.mcdsoft_event as evIn");
            sb.AppendLine("  order by evIn.CreatedOn desc");
            sb.AppendLine("  offset 0 rows");
            sb.AppendLine("  fetch next 500 rows only)");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<mcdsoft_event> ChangeByRules(IEnumerable<mcdsoft_event> mcdsoftEvents)
        {
            var random = new Random();
            foreach (var mcdsoftEvent in mcdsoftEvents)
            {
                mcdsoftEvent.new_expenses = random.Next(10000, 99999);
                yield return mcdsoftEvent;
            }
        }

        protected override mcdsoft_event ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new mcdsoft_event
            {
                Id = (Guid)sqlReader.GetValue(0),
                new_expenses = sqlReader.GetValue(1) as int?,
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?,
            };
        }

        protected override Entity GetEntityForUpdate(mcdsoft_event mcdsoftEvent)
        {
            var entityForUpdate = new Entity(mcdsoftEvent.LogicalName, mcdsoftEvent.Id);
            entityForUpdate["new_expenses"] = mcdsoftEvent.new_expenses;
            return entityForUpdate;
        }
    }
}