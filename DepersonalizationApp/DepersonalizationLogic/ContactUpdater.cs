using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ContactUpdater : BaseUpdater<Contact>
    {
        public ContactUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Opportunity> opportunities) : base(orgService, sqlConnection)
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
            sb.AppendLine("select c.ContactId, c.FirstName, c.LastName, c.MiddleName");
            sb.AppendLine(" acc.Address1_PostalCode, acc.Description, acc.cmdsoft_inn, acc.ParentAccountId");
            sb.AppendLine(" from dbo.Contact as c");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("c.ContactId", contactIdsDistinct);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override Contact ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var contact = new Contact
            {
                Id = (Guid)sqlReader.GetValue(0),
                FirstName = sqlReader.GetValue(1) as string,
                LastName = sqlReader.GetValue(2) as string,
                MiddleName = sqlReader.GetValue(3) as string,
            };
            return contact;
        }

        protected override void ChangeByRules(IEnumerable<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                
            }
        }
    }
}