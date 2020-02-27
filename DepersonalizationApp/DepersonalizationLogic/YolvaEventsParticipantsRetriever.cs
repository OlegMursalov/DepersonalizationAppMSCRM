using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class YolvaEventsParticipantsLink
    {
        public Guid Id { get; set; }

        // Контакт (contact)
        public Guid? YolvaContact { get; set; }

        // Организация (account)
        public Guid? YolvaOrganization { get; set; }
    }

    public class YolvaEventsParticipantsRetriever : Base<YolvaEventsParticipantsLink>
    {
        public YolvaEventsParticipantsRetriever(SqlConnection sqlConnection, IEnumerable<Guid> eventIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select evPart.yolva_events_participantsId, evPart.yolva_contact, evPart.yolva_organization");
            sb.AppendLine(" from yolva_events_participants as evPart");
            sb.AppendLine(" where evPart.yolva_events_participantsId in (select evPartIn.yolva_events_participantsId");
            sb.AppendLine("  from dbo.yolva_events_participants as evPartIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("evPartIn.yolva_event", eventIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("evPartIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        public IEnumerable<YolvaEventsParticipantsLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override YolvaEventsParticipantsLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new YolvaEventsParticipantsLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                YolvaContact = sqlReader.GetValue(1) as Guid?,
                YolvaOrganization = sqlReader.GetValue(2) as Guid?
            };
        }
    }
}