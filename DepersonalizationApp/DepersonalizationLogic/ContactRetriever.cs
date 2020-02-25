using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ContactSimple
    {
        public Guid ContactId { get; set; }
        public Guid? ParentCustomerId { get; set; }
    }

    /// <summary>
    /// Извлекаем связанные с Opportunity контакты
    /// </summary>
    public class ContactRetriever : Base<ContactSimple>
    {
        public ContactRetriever(SqlConnection sqlConnection, IEnumerable<Opportunity> opportunities) : base(sqlConnection)
        {
            var contactIds = new List<Guid>();

            foreach (var opportunity in opportunities)
            {
                if (opportunity.cmdsoft_Managerproject != null)
                {
                    contactIds.Add(opportunity.cmdsoft_Managerproject.Id);
                }
                if (opportunity.cmdsoft_Dealer != null)
                {
                    contactIds.Add(opportunity.cmdsoft_Dealer.Id);
                }
                if (opportunity.cmdsoft_contact_project_agency != null)
                {
                    contactIds.Add(opportunity.cmdsoft_contact_project_agency.Id);
                }
                if (opportunity.mcdsoft_ref_contact != null)
                {
                    contactIds.Add(opportunity.mcdsoft_ref_contact.Id);
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

        public IEnumerable<ContactSimple> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override ContactSimple ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new ContactSimple
            {
                ContactId = (Guid)sqlReader.GetValue(0),
                ParentCustomerId = sqlReader.GetValue(1) as Guid?
            };
        }
    }
}