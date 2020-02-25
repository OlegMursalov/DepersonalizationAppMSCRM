using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace UpdaterApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновление проектов
    /// </summary>
    public class OpportunityUpdater : BaseUpdater<Opportunity>
    {
        public OpportunityUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select opp.OpportunityId, opp.mcdsoft_discount, opp.cmdsoft_standartdiscount, opp.mcdsoft_standartdiscount_chiller,");
            sb.AppendLine(" opp.cmdsoft_warranty, opp.cmdsoft_Result, opp.mcdsoft_reason_for_the_loss, opp.CustomerId, opp.cmdsoft_project_agency,");
            sb.AppendLine(" opp.mcdsoft_ref_account, opp.cmdsoft_GeneralContractor, opp.cmdsoft_Managerproject, opp.cmdsoft_Dealer,");
            sb.AppendLine(" opp.cmdsoft_contact_project_agency, opp.mcdsoft_ref_contact");
            sb.AppendLine(" from Opportunity as opp");
            sb.AppendLine(" where opp.OpportunityId in (select oppIn.OpportunityId");
            sb.AppendLine("  from dbo.Opportunity as oppIn");
            sb.AppendLine("  order by oppIn.CreatedOn desc");
            sb.AppendLine("  offset 0 rows");
            sb.AppendLine("  fetch next 500 rows only)");
            _retrieveSqlQuery = sb.ToString();
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
            var cmdsoft_ManagerprojectId = sqlReader.GetValue(11) as Guid?;
            if (cmdsoft_ManagerprojectId != null)
            {
                opportunity.cmdsoft_Managerproject = new EntityReference("contact", cmdsoft_ManagerprojectId.Value);
            }
            var cmdsoft_DealerId = sqlReader.GetValue(12) as Guid?;
            if (cmdsoft_DealerId != null)
            {
                opportunity.cmdsoft_Dealer = new EntityReference("contact", cmdsoft_DealerId.Value);
            }
            var cmdsoft_contact_project_agencyId = sqlReader.GetValue(13) as Guid?;
            if (cmdsoft_contact_project_agencyId != null)
            {
                opportunity.cmdsoft_contact_project_agency = new EntityReference("contact", cmdsoft_contact_project_agencyId.Value);
            }
            var mcdsoft_ref_contactId = sqlReader.GetValue(14) as Guid?;
            if (mcdsoft_ref_contactId != null)
            {
                opportunity.mcdsoft_ref_contact = new EntityReference("contact", mcdsoft_ref_contactId.Value);
            }
            return opportunity;
        }

        protected override void ChangeByRules(IEnumerable<Opportunity> opportunities)
        {
            var randomHelper = new RandomHelper();
            var shuffleReasonsForTheLoss = new ShuffleFieldValuesHelper<Opportunity, string>("mcdsoft_reason_for_the_loss");

            foreach (var opportunity in opportunities)
            {
                // А. Если значение поля «Ручной ввод скидки»(mcdsoft_discount) = «Да» [1], то 
                // заполнить поля «Основная скидка СМ»(cmdsoft_standartdiscount), «% Основная скидка Чиллера»(mcdsoft_standartdiscount_chiller %), 
                // «Гарантия, %»(cmdsoft_warranty) = Random(Тип - число в плавающей точкой, точность - 2, 0 - 100, 00)
                if (opportunity.mcdsoft_discount != null && (bool)opportunity.mcdsoft_discount)
                {
                    opportunity.cmdsoft_standartdiscount = randomHelper.GetDecimal(0, 100);
                    opportunity.mcdsoft_standartdiscount_chiller = randomHelper.GetDecimal(0, 100);
                    opportunity.cmdsoft_warranty = randomHelper.GetDecimal(0, 100);
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
            opportunities = shuffleReasonsForTheLoss.Process();

            // C. Все что есть в примечаниях (Notes) и действиях (actions), связанных с проектами, удалить (сообщения, эл. почта, прикрепленный файлы)
            var opportunityGuids = opportunities.Select(e => e.Id).ToArray();

            var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, opportunityGuids);
            relatedActivityDeleter.Process();

            // Удаление примечаний
            var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, opportunityGuids);
            annotationDeleter.Process();

            // D. 2. Меняем связанные с изменяемыми проектами записи сущности «Доля ответственного» (cmdsoft_part_of_owner), 
            // связь по полю cmdsoft_part_of_owner.cmdsoft_ref_opportunity:
            // В изменяемых записях меняем значения поля «Доля %»(cmdsoft_part) = Random(Тип - integer, 0 - 100), 
            // таким образом, чтобы по каждому проекту сумма Полей «Доля» СУММА(cmdsoft_part_of_owner.cmdsoft_part по каждому проекту) = 100.
            var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_orgService, _sqlConnection, opportunityGuids);
            partOfOwnerUpdater.Process();

            // 3. Продажи
            // Меняем связанные с изменяемыми проектами записи сущности «Продажи»(cmdsoft_ordernav)по полю «Название проекта»(cmdsoft_ordernav.cmdsoft_navid).
            var orderNavUpdater = new CmdsoftOrderNavUpdater(_orgService, _sqlConnection, opportunityGuids);
            var orderNavUpdatedGuids = orderNavUpdater.Process().ToArray();

            // 4. Меняем связанные с изменяемыми проектами записи сущности Составы продаж (cmdsoft_orderlinenav), меняем поля:
            // С каждой записью «Составы продаж», взять Var_Rand_n = Random(Тип – Целое число, 0 - 9) и поделить все 
            // изменяемые поля на это число(важно, чтобы случайное число у каждой отдельной записи «Состава продаж» было одно)...
            var orderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_orgService, _sqlConnection, orderNavUpdatedGuids);
            orderlineNavUpdater.Process();

            // Сущности Проекты (Opportunity), организации (Account) и контакты (Contact) перевязаны друг с другом, достаем разницу
            var accountRetriever = new AccountRetriever(_sqlConnection, opportunities);
            var accountSimples = accountRetriever.Process();

            var contactRetriever = new ContactRetriever(_sqlConnection, opportunities);
            var contactSimples = contactRetriever.Process();

            var accountGuids = new List<Guid>();
            accountGuids.AddRange(accountSimples.Select(accS => accS.AccountId));
            accountGuids.AddRange(contactSimples.Select(conS => conS.ParentCustomerId));
            var uniqueAccountGuids = accountGuids.Distinct().ToArray();

            var contactGuids = new List<Guid>();
            contactGuids.AddRange(contactSimples.Select(conS => conS.ContactId));
            contactGuids.AddRange(accountSimples.Select(accS => accS.PrimaryContactId));
            var uniqueContactGuids = contactGuids.Distinct().ToArray();

            // 5. Меняем связанные с изменяемыми проектами записи сущности «Организация»(account), связи по полям «Заказчик»(customerid) и 
            // «Проектная Организация»(cmdsoft_project_agency), «Эксплуатирующая организация»(mcdsoft_ref_account), «Ген. подрядчик»(cmdsoft_generalcontractor)...
            var accountUpdater = new AccountUpdater(_orgService, _sqlConnection, uniqueAccountGuids);
            var accountUpdatedGuids = accountUpdater.Process();

            // 6. Контакты(contact)
            // Меняем связанные с изменяемыми проектами и организациями записи сущности «Контакты», 
            // связи по полям «Менеджер заказчика(Проект)»(opportunity.cmdsoft_managerproject), «Дилер(Проект)» (opportunity.cmdsoft_dealer), 
            // Проектировщик(Проект)»(opportunity.cmdsoft_contact_project_agency) «Контакт эксплуатирующей организации(Проект)»(opportunity.mcdsoft_ref_contact)...
            var contactUpdater = new ContactUpdater(_orgService, _sqlConnection, uniqueContactGuids);
            contactUpdater.Process();
        }
    }
}