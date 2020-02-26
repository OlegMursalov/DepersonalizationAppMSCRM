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
    public class ContactUpdater : BaseUpdater<Contact>
    {
        public ContactUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] contactIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select c.ContactId, c.FirstName, c.LastName, c.MiddleName, c.mcdsoft_contactnumber");
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
                mcdsoft_contactnumber = sqlReader.GetValue(4) as string
            };
            return contact;
        }

        protected override IEnumerable<Contact> ChangeByRules(IEnumerable<Contact> contacts)
        {
            var random = new Random();
            var shuffleFieldValues = new ShuffleFieldValuesHelper<Contact>();

            foreach (var contact in contacts)
            {
                shuffleFieldValues.AddEntity(contact);
                shuffleFieldValues.AddValue("firstname", contact.FirstName);
                shuffleFieldValues.AddValue("lastname", contact.LastName);
                shuffleFieldValues.AddValue("middlename", contact.MiddleName);
                contact.mcdsoft_contactnumber = random.Next(10000, 99999).ToString();
            }

            contacts = shuffleFieldValues.Process();

            return contacts;
        }

        protected override Entity GetEntityForUpdate(Contact contact)
        {
            var entityForUpdate = new Entity(contact.LogicalName, contact.Id);
            entityForUpdate[_commonDepersonalizationNameField] = true;
            entityForUpdate["firstname"] = contact.FirstName;
            entityForUpdate["lastname"] = contact.LastName;
            entityForUpdate["middlename"] = contact.MiddleName;
            entityForUpdate["mcdsoft_contactnumber"] = contact.mcdsoft_contactnumber;
            return entityForUpdate;
        }
    }
}