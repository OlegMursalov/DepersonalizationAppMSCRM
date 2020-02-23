using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace UpdaterApp.DepersonalizationLogic
{
    public class OpportunityUpdater : BaseUpdater<Opportunity>
    {
        public OpportunityUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
            _retrieveSqlQuery =
                "select top(100) opp.OpportunityId, opp.mcdsoft_discount, opp.cmdsoft_standartdiscount, opp.mcdsoft_standartdiscount_chiller," +
                " opp.cmdsoft_warranty, opp.cmdsoft_Result, opp.mcdsoft_reason_for_the_loss, opp.CustomerId, opp.cmdsoft_project_agency," +
                " opp.mcdsoft_ref_account, opp.cmdsoft_GeneralContractor" +
                " from dbo.Opportunity as opp" +
                " order by opp.CreatedOn desc";
        }

        protected override Opportunity ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var opportunity = new Opportunity
            {
                Id = (Guid)sqlReader.GetValue(0),
                mcdsoft_discount = sqlReader.GetValue(1) as bool?,
                cmdsoft_standartdiscount = sqlReader.GetValue(2) as decimal?,
                mcdsoft_standartdiscount_chiller = sqlReader.GetValue(3) as decimal?,
                cmdsoft_warranty = sqlReader.GetValue(4) as decimal?,
                mcdsoft_reason_for_the_loss = sqlReader.GetValue(6) as string,
            };
            var cmdsoft_ResultVal = sqlReader.GetValue(5) as int?;
            if (cmdsoft_ResultVal != null)
            {
                opportunity.cmdsoft_Result = new OptionSetValue(cmdsoft_ResultVal.Value);
            }
            var customerId = sqlReader.GetValue(7) as Guid?;
            if (customerId != null)
            {
                opportunity.CustomerId = new EntityReference("account", customerId.Value);
            }
            var cmdsoft_project_agencyId = sqlReader.GetValue(8) as Guid?;
            if (cmdsoft_project_agencyId != null)
            {
                opportunity.cmdsoft_project_agency = new EntityReference("account", cmdsoft_project_agencyId.Value);
            }
            var mcdsoft_ref_accountId = sqlReader.GetValue(9) as Guid?;
            if (mcdsoft_ref_accountId != null)
            {
                opportunity.mcdsoft_ref_account = new EntityReference("account", mcdsoft_ref_accountId.Value);
            }
            var cmdsoft_GeneralContractorId = sqlReader.GetValue(10) as Guid?;
            if (cmdsoft_GeneralContractorId != null)
            {
                opportunity.cmdsoft_GeneralContractor = new EntityReference("account", cmdsoft_GeneralContractorId.Value);
            }
            return opportunity;
        }
        
        protected override void ChangeByRules(IEnumerable<Opportunity> opportunities)
        {
            var random = new Random();
            var shuffleReasonsForTheLoss = new ShuffleFieldValues<string>("mcdsoft_reason_for_the_loss");

            foreach (var opportunity in opportunities)
            {
                // А. Если значение поля «Ручной ввод скидки»(mcdsoft_discount) = «Да» [1], то 
                // заполнить поля «Основная скидка СМ»(cmdsoft_standartdiscount), «% Основная скидка Чиллера»(mcdsoft_standartdiscount_chiller %), 
                // «Гарантия, %»(cmdsoft_warranty) = Random(Тип - число в плавающей точкой, точность - 2, 0 - 100, 00)
                if (opportunity.mcdsoft_discount != null && (bool)opportunity.mcdsoft_discount)
                {
                    decimal i = random.Next(0, 100);
                    decimal c = i + (decimal)random.NextDouble();
                    opportunity.cmdsoft_standartdiscount = c;
                    opportunity.mcdsoft_standartdiscount_chiller = c;
                    opportunity.cmdsoft_warranty = c;
                }

                // B. В тех проектах, где значение поля «Результат»(cmdsoft_result) = «Проигран» [289 540 002], 
                // копировать в отдельную таблицу значения полей «Причина проигрыша» (mcdsoft_reason_for_the_loss), 
                // потом из этой таблицы случайным образом вставить(переписать) значения в другой проект(то есть перетасовать в проигранных проектах «Причины проигрыша»)
                if (opportunity.cmdsoft_Result != null && opportunity.cmdsoft_Result.Value == 289540002)
                {
                    shuffleReasonsForTheLoss.AddEntity(opportunity);
                    shuffleReasonsForTheLoss.AddValue(opportunity.mcdsoft_reason_for_the_loss);
                }
            }

            // B. Дополнение (см. выше)
            shuffleReasonsForTheLoss.Process();

            // C. Все что есть в примечаниях (Notes) и действиях (actions), связанных с проектами, удалить (сообщения, эл. почта, прикрепленный файлы)
            var opportunityGuids = opportunities.Select(e => e.Id).ToArray();

            // Удаление задач
            var relatedTaskDeleter = new RelatedTaskDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedTaskDeleter.Process();

            // Удаление факсов
            var relatedFaxDeleter = new RelatedFaxDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedFaxDeleter.Process();

            // Удаление звонков
            var relatedPhoneCallDeleter = new RelatedPhoneCallDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedPhoneCallDeleter.Process();

            // Удаление эмеилов
            var relatedEmailDeleter = new RelatedEmailDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedEmailDeleter.Process();

            // Удаление писем
            var relatedLetterDeleter = new RelatedLetterDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedLetterDeleter.Process();

            // Удаление встреч
            var relatedAppointmentDeleter = new RelatedAppointmentDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedAppointmentDeleter.Process();

            // Удаление действий сервиса
            var relatedServiceAppointmentDeleter = new RelatedServiceAppointmentDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedServiceAppointmentDeleter.Process();

            // Удаление откликов от кампании
            var relatedCampaignResponseDeleter = new RelatedCampaignResponseDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedCampaignResponseDeleter.Process();

            // Удаление повторяющихся встреч
            var relatedRecurringAppointmentMasterDeleter = new RelatedRecurringAppointmentMasterDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedRecurringAppointmentMasterDeleter.Process();

            // Удаление примечаний
            var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, opportunityGuids);
            annotationDeleter.Process();

            // D. 2. Меняем связанные с изменяемыми проектами записи сущности «Доля ответственного» (cmdsoft_part_of_owner), 
            // связь по полю cmdsoft_part_of_owner.cmdsoft_ref_opportunity:
            // В изменяемых записях меняем значения поля «Доля %»(cmdsoft_part) = Random(Тип - integer, 0 - 100), 
            // таким образом, чтобы по каждому проекту сумма Полей «Доля» СУММА(cmdsoft_part_of_owner.cmdsoft_part по каждому проекту) = 100.
            var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_orgService, _sqlConnection, opportunityGuids);
            partOfOwnerUpdater.Process();

            // D. 3. Меняем связанные с изменяемыми проектами записи сущности Составы продаж (cmdsoft_orderlinenav), меняем поля:
            // С каждой записью «Составы продаж», взять Var_Rand_n = Random(Тип – Целое число, 0 - 9) и поделить все 
            // изменяемые поля на это число(важно, чтобы случайное число у каждой отдельной записи «Состава продаж» было одно)...
            var orderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_orgService, _sqlConnection, opportunityGuids);
            orderlineNavUpdater.Process();

            // 4.	Меняем связанные с изменяемыми проектами записи сущности «Организация»(account), связи по полям «Заказчик»(customerid) и 
            // «Проектная Организация»(cmdsoft_project_agency), «Эксплуатирующая организация»(mcdsoft_ref_account), «Ген. подрядчик»(cmdsoft_generalcontractor)...
            var accountUpdater = new AccountUpdater(_orgService, _sqlConnection, opportunities);
            accountUpdater.Process();
        }
    }
}