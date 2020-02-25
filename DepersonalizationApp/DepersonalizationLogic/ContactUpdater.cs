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
        public ContactUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] contactIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select c.ContactId, c.FirstName, c.LastName, c.MiddleName");
            sb.AppendLine(" acc.Address1_PostalCode, acc.Description, acc.cmdsoft_inn, acc.ParentAccountId");
            sb.AppendLine(" from dbo.Contact as c");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("c.ContactId", contactIds);
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