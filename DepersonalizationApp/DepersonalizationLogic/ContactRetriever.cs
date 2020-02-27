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

            var contactIdsDistinct = contactIds.Distinct().ToArray();

            var sb = new StringBuilder();
            sb.AppendLine("select c.ContactId, c.ParentCustomerId");
            sb.AppendLine(" from dbo.Contact as c");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("c.ContactId", contactIdsDistinct);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
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