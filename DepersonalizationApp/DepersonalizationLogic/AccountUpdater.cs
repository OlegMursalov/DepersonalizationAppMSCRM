﻿using CRMEntities;
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
    public class AccountUpdater : BaseUpdater<Account>
    {
        private static int _globalCounterBySessionApp = 1;

        public AccountUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select acc.AccountId, acc.Name, acc.Telephone1, acc.EMailAddress1, acc.WebSiteURL,");
            sb.AppendLine($" acc.Address1_PostalCode, acc.Description, acc.cmdsoft_inn, acc.ParentAccountId, acc.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.Account as acc");
            sb.AppendLine(" where acc.AccountId in (select accIn.AccountId");
            sb.AppendLine("  from dbo.Account as accIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("accIn.AccountId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("accIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override Account ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var account = new Account
            {
                Id = (Guid)sqlReader.GetValue(0),
                Name = sqlReader.GetValue(1) as string,
                Telephone1 = sqlReader.GetValue(2) as string,
                EMailAddress1 = sqlReader.GetValue(3) as string,
                WebSiteURL = sqlReader.GetValue(4) as string,
                Address1_PostalCode = sqlReader.GetValue(5) as string,
                Description = sqlReader.GetValue(6) as string,
                cmdsoft_inn = sqlReader.GetValue(7) as string,
                yolva_is_depersonalized = sqlReader.GetValue(9) as bool?
            };
            var parentAccountId = sqlReader.GetValue(8) as Guid?;
            if (parentAccountId != null)
            {
                account.ParentAccountId = new EntityReference("account", parentAccountId.Value);
            }
            return account;
        }

        protected override IEnumerable<Account> ChangeByRules(IEnumerable<Account> accounts)
        {
            var randomTelephoneByMaskHelper = new RandomTelephoneByMaskHelper(CommonObjsHelper.TelephoneMask1);
            var randomEmailByMaskHelper = new RandomEmailByMaskHelper(CommonObjsHelper.EmailMask1);
            foreach (var account in accounts)
            {
                account.Name = $"Организация №{_globalCounterBySessionApp}";
                account.Telephone1 = randomTelephoneByMaskHelper.Get();
                account.EMailAddress1 = randomEmailByMaskHelper.Get();
                account.WebSiteURL = null;
                account.Address1_PostalCode = null;
                account.Description = null;
                account.cmdsoft_inn = null;
                account.ParentAccountId = null;
                _globalCounterBySessionApp++;
                yield return account;
            }
        }

        protected override Entity GetEntityForUpdate(Account account)
        {
            var entityForUpdate = new Entity(account.LogicalName, account.Id);
            entityForUpdate["name"] = account.Name;
            entityForUpdate["telephone1"] = account.Telephone1;
            entityForUpdate["emailaddress1"] = account.EMailAddress1;
            entityForUpdate["websiteurl"] = account.WebSiteURL;
            entityForUpdate["address1_postalcode"] = account.Address1_PostalCode;
            entityForUpdate["description"] = account.Description;
            entityForUpdate["cmdsoft_inn"] = account.cmdsoft_inn;
            entityForUpdate["parentaccountid"] = account.ParentAccountId;
            return entityForUpdate;
        }
    }
}