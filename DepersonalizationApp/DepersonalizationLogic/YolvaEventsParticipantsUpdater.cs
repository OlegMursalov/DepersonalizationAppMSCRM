using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновление участников мероприятия
    /// </summary>
    public class YolvaEventsParticipantsUpdater : BaseUpdater<yolva_events_participants>
    {
        private static int _globalCounterBySessionApp = 1;

        public YolvaEventsParticipantsUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] mcdsoftEventIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select evPart.yolva_events_participantsId, evPart.yolva_name, evPart.yolva_contact, evPart.yolva_organization, evPart.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from yolva_events_participants as evPart");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("evPart.yolva_event", mcdsoftEventIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<yolva_events_participants> ChangeByRules(IEnumerable<yolva_events_participants> eventsParticipants)
        {
            foreach (var eventsParticipant in eventsParticipants)
            {
                eventsParticipant.yolva_name = $"Участник №{_globalCounterBySessionApp}";
                _globalCounterBySessionApp++;
                yield return eventsParticipant;
            }
        }

        protected override yolva_events_participants ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var yolva_events_participants = new yolva_events_participants
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_name = sqlReader.GetValue(1) as string,
                yolva_is_depersonalized = sqlReader.GetValue(4) as bool?
            };
            var yolva_contactId = sqlReader.GetValue(2) as Guid?;
            if (yolva_contactId != null)
            {
                yolva_events_participants.yolva_contact = new EntityReference("contact", yolva_contactId.Value);
            }
            var yolva_organizationId = sqlReader.GetValue(3) as Guid?;
            if (yolva_organizationId != null)
            {
                yolva_events_participants.yolva_organization = new EntityReference("account", yolva_organizationId.Value);
            }
            return yolva_events_participants;
        }

        protected override Entity GetEntityForUpdate(yolva_events_participants eventsParticipant)
        {
            var entityForUpdate = new Entity(eventsParticipant.LogicalName, eventsParticipant.Id);
            entityForUpdate["yolva_name"] = eventsParticipant.yolva_name;
            return entityForUpdate;
        }
    }
}