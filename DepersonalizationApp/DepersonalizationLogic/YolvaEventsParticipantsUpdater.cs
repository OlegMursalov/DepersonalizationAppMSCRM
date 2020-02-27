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

        public YolvaEventsParticipantsUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select evPart.yolva_events_participantsId, evPart.yolva_name, evPart.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from yolva_events_participants as evPart");
            sb.AppendLine(" where evPart.yolva_events_participantsId in (select evPartIn.yolva_events_participantsId");
            sb.AppendLine("  from dbo.yolva_events_participants as evPartIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("evPart.yolva_events_participantsId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("evPart.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
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
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?
            };
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