using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ContactLink
    {
        public Guid Id { get; set; }
        public Guid? ParentCustomerId { get; set; }
    }

    /// <summary>
    /// Извлекаем связанные с Opportunity контакты
    /// </summary>
    public class ContactRetriever : Base<ContactLink>
    {
        public ContactRetriever(SqlConnection sqlConnection, IEnumerable<McdsoftSalesAppealLink> salesAppealLinks) : base(sqlConnection)
        {
            var contactIds = new List<Guid>();
            foreach (var salesAppealLink in salesAppealLinks)
            {
                if (salesAppealLink.McdsoftRefContact != null)
                {
                    contactIds.Add(salesAppealLink.McdsoftRefContact.Value);
                }
                if (salesAppealLink.McdsoftRefContactAsc != null)
                {
                    contactIds.Add(salesAppealLink.McdsoftRefContactAsc.Value);
                }
            }
            var contactIdsDistinct = contactIds.Distinct();
            _retrieveSqlQuery = SetQuery(contactIdsDistinct);
        }

        public ContactRetriever(SqlConnection sqlConnection, IEnumerable<YolvaEventsParticipantsLink> yolvaEventsParticipantsLinks) : base(sqlConnection)
        {
            var contactIds = new List<Guid>();
            foreach (var yolvaEventsParticipantsLink in yolvaEventsParticipantsLinks)
            {
                if (yolvaEventsParticipantsLink.YolvaContact != null)
                {
                    contactIds.Add(yolvaEventsParticipantsLink.YolvaContact.Value);
                }
            }
            var contactIdsDistinct = contactIds.Distinct();
            _retrieveSqlQuery = SetQuery(contactIdsDistinct);
        }

        public ContactRetriever(SqlConnection sqlConnection, IEnumerable<OpportunityLink> opportunityLinks) : base(sqlConnection)
        {
            var contactIds = new List<Guid>();
            foreach (var opportunityLink in opportunityLinks)
            {
                if (opportunityLink.CmdsoftManagerProject != null)
                {
                    contactIds.Add(opportunityLink.CmdsoftManagerProject.Value);
                }
                if (opportunityLink.CmdsoftDealer != null)
                {
                    contactIds.Add(opportunityLink.CmdsoftDealer.Value);
                }
                if (opportunityLink.CmdsoftContactProjectAgency != null)
                {
                    contactIds.Add(opportunityLink.CmdsoftContactProjectAgency.Value);
                }
                if (opportunityLink.McdsoftRefContact != null)
                {
                    contactIds.Add(opportunityLink.McdsoftRefContact.Value);
                }
            }
            var contactIdsDistinct = contactIds.Distinct();
            _retrieveSqlQuery = SetQuery(contactIdsDistinct);
        }

        private string SetQuery(IEnumerable<Guid> contactIdsDistinct)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select c.ContactId, c.ParentCustomerId");
            sb.AppendLine(" from dbo.Contact as c");
            sb.AppendLine(" where c.ContactId in (select cIn.ContactId");
            sb.AppendLine("  from dbo.Contact as cIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("cIn.AccountId", contactIdsDistinct);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("cIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            return sb.ToString();
        }

        public IEnumerable<ContactLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override ContactLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new ContactLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                ParentCustomerId = sqlReader.GetValue(1) as Guid?
            };
        }
    }
}