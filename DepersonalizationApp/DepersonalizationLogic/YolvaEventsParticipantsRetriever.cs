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
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("evPart.yolva_event", eventIds);
            sb.AppendLine(where);
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