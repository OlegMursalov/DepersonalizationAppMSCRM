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
    public class AccountUpdater : BaseUpdater<Account>
    {
        private static int _globalCounterBySessionApp = 1;

        public AccountUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] accountIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select acc.AccountId, acc.Name, acc.Telephone1, acc.EMailAddress1, acc.WebSiteURL,");
            sb.AppendLine(" acc.Address1_PostalCode, acc.Description, acc.cmdsoft_inn, acc.ParentAccountId");
            sb.AppendLine(" from dbo.Account as acc");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("acc.AccountId", accountIds);
            sb.AppendLine(where);
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
                cmdsoft_inn = sqlReader.GetValue(7) as string
            };
            var parentAccountId = sqlReader.GetValue(8) as Guid?;
            if (parentAccountId != null)
            {
                account.ParentAccountId = new EntityReference("account", parentAccountId.Value);
            }
            return account;
        }

        protected override void ChangeByRules(IEnumerable<Account> accounts)
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
            }

            // Все что есть в примечаниях (Notes) и действиях (actions), связанных с организациями, удалить (сообщения, эл. почта, прикрепленный файлы)
            var accountIds = accounts.Select(e => e.Id).ToArray();

            // Удаление задач
            var relatedTaskDeleter = new RelatedTaskDeleter(_orgService, _sqlConnection, accountIds);
            relatedTaskDeleter.Process();

            // Удаление факсов
            var relatedFaxDeleter = new RelatedFaxDeleter(_orgService, _sqlConnection, accountIds);
            relatedFaxDeleter.Process();

            // Удаление звонков
            var relatedPhoneCallDeleter = new RelatedPhoneCallDeleter(_orgService, _sqlConnection, accountIds);
            relatedPhoneCallDeleter.Process();

            // Удаление эмеилов
            var relatedEmailDeleter = new RelatedEmailDeleter(_orgService, _sqlConnection, accountIds);
            relatedEmailDeleter.Process();

            // Удаление писем
            var relatedLetterDeleter = new RelatedLetterDeleter(_orgService, _sqlConnection, accountIds);
            relatedLetterDeleter.Process();

            // Удаление встреч
            var relatedAppointmentDeleter = new RelatedAppointmentDeleter(_orgService, _sqlConnection, accountIds);
            relatedAppointmentDeleter.Process();

            // Удаление действий сервиса
            var relatedServiceAppointmentDeleter = new RelatedServiceAppointmentDeleter(_orgService, _sqlConnection, accountIds);
            relatedServiceAppointmentDeleter.Process();

            // Удаление откликов от кампании
            var relatedCampaignResponseDeleter = new RelatedCampaignResponseDeleter(_orgService, _sqlConnection, accountIds);
            relatedCampaignResponseDeleter.Process();

            // Удаление повторяющихся встреч
            var relatedRecurringAppointmentMasterDeleter = new RelatedRecurringAppointmentMasterDeleter(_orgService, _sqlConnection, accountIds);
            relatedRecurringAppointmentMasterDeleter.Process();

            // Удаление примечаний
            var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, accountIds);
            annotationDeleter.Process();
        }
    }
}