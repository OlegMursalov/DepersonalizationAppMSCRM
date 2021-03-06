﻿using CRMEntities;
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
        public ContactUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select c.ContactId, c.FirstName, c.LastName, c.MiddleName, c.mcdsoft_contactnumber, c.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.Contact as c");
            sb.AppendLine(" where c.ContactId in (select cIn.ContactId");
            sb.AppendLine("  from dbo.Contact as cIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("cIn.ContactId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("cIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
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
                mcdsoft_contactnumber = sqlReader.GetValue(4) as string,
                yolva_is_depersonalized = sqlReader.GetValue(5) as bool?
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

            shuffleFieldValues.Process();

            return contacts;
        }

        protected override Entity GetEntityForUpdate(Contact contact)
        {
            var entityForUpdate = new Entity(contact.LogicalName, contact.Id);
            entityForUpdate["firstname"] = contact.FirstName;
            entityForUpdate["lastname"] = contact.LastName;
            entityForUpdate["middlename"] = contact.MiddleName;
            entityForUpdate["mcdsoft_contactnumber"] = contact.mcdsoft_contactnumber;
            return entityForUpdate;
        }
    }
}