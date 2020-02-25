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

        protected override void ChangeByRules(IEnumerable<Contact> contacts)
        {
            var random = new Random();
            var shuffleFirstName = new ShuffleFieldValuesHelper<Contact, string>("firstname");
            var shuffleLastName = new ShuffleFieldValuesHelper<Contact, string>("lastname");
            var shuffleMiddleName = new ShuffleFieldValuesHelper<Contact, string>("middlename");

            foreach (var contact in contacts)
            {
                shuffleFirstName.AddEntity(contact);
                shuffleFirstName.AddValue(contact.FirstName);

                shuffleLastName.AddEntity(contact);
                shuffleLastName.AddValue(contact.LastName);

                shuffleMiddleName.AddEntity(contact);
                shuffleMiddleName.AddValue(contact.MiddleName);

                contact.mcdsoft_contactnumber = random.Next(10000, 99999).ToString();
            }

            contacts = shuffleFirstName.Process();
            contacts = shuffleLastName.Process();
            contacts = shuffleMiddleName.Process();

            // Все что есть в примечаниях (Notes) и действиях (actions), связанных с организациями, удалить (сообщения, эл. почта, прикрепленный файлы)
            var contactIds = contacts.Select(e => e.Id).ToArray();

            var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, contactIds);
            relatedActivityDeleter.Process();

            // Удаление примечаний
            var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, contactIds);
            annotationDeleter.Process();
        }
    }
}