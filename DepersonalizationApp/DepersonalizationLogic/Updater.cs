using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class Updater
    {
        private IOrganizationService _orgService;
        private SqlConnection _sqlConnection;

        public Updater(IOrganizationService orgService, SqlConnection sqlConnection)
        {
            _orgService = orgService;
            _sqlConnection = sqlConnection;
        }

        /// <summary>
        /// Обновление всех сущностей. Возвращает словарь из Guid'ов всех обновленных записей
        /// </summary>
        public Dictionary<string, Guid[]> Execute()
        {
            var allUpdated = new Dictionary<string, Guid[]>();

            // D. 1. Обновляем проекты
            var opportunityUpdater = new OpportunityUpdater(_orgService, _sqlConnection);
            var updatedOpportunities = opportunityUpdater.Process();
            var updatedOpportunityIds = updatedOpportunities.Select(e => e.Id).ToArray();
            allUpdated.Add("opportunity", updatedOpportunityIds);

            // D. 2. Меняем связанные с изменяемыми проектами записи сущности «Доля ответственного» (cmdsoft_part_of_owner), 
            // связь по полю cmdsoft_part_of_owner.cmdsoft_ref_opportunity:
            // В изменяемых записях меняем значения поля «Доля %»(cmdsoft_part) = Random(Тип - integer, 0 - 100), 
            // таким образом, чтобы по каждому проекту сумма Полей «Доля» СУММА(cmdsoft_part_of_owner.cmdsoft_part по каждому проекту) = 100.
            var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_orgService, _sqlConnection, updatedOpportunityIds);
            partOfOwnerUpdater.Process();

            // 3. Продажи
            // Меняем связанные с изменяемыми проектами записи сущности «Продажи»(cmdsoft_ordernav)по полю «Название проекта»(cmdsoft_ordernav.cmdsoft_navid).
            var orderNavUpdater = new CmdsoftOrderNavUpdater(_orgService, _sqlConnection, updatedOpportunityIds);
            var orderNavUpdated = orderNavUpdater.Process();
            var orderNavUpdatedIds = orderNavUpdated.Select(e => e.Id).ToArray();

            // 4. Меняем связанные с изменяемыми проектами записи сущности Составы продаж (cmdsoft_orderlinenav), меняем поля:
            // С каждой записью «Составы продаж», взять Var_Rand_n = Random(Тип – Целое число, 0 - 9) и поделить все 
            // изменяемые поля на это число(важно, чтобы случайное число у каждой отдельной записи «Состава продаж» было одно)...
            var orderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_orgService, _sqlConnection, orderNavUpdatedIds);
            orderlineNavUpdater.Process();

            // Сущности Проекты (Opportunity), организации (Account) и контакты (Contact) перевязаны друг с другом, достаем разницу
            var accountRetriever = new AccountRetriever(_sqlConnection, updatedOpportunities);
            var accountSimples = accountRetriever.Process();

            var contactRetriever = new ContactRetriever(_sqlConnection, updatedOpportunities);
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
            var updatedAccounts = accountUpdater.Process();
            allUpdated.Add("account", updatedAccounts.Select(e => e.Id).ToArray());

            // 6. Контакты(contact)
            // Меняем связанные с изменяемыми проектами и организациями записи сущности «Контакты», 
            // связи по полям «Менеджер заказчика(Проект)»(opportunity.cmdsoft_managerproject), «Дилер(Проект)» (opportunity.cmdsoft_dealer), 
            // Проектировщик(Проект)»(opportunity.cmdsoft_contact_project_agency) «Контакт эксплуатирующей организации(Проект)»(opportunity.mcdsoft_ref_contact)...
            var contactUpdater = new ContactUpdater(_orgService, _sqlConnection, uniqueContactGuids);
            var updatedContacts = contactUpdater.Process();
            allUpdated.Add("contact", updatedContacts.Select(e => e.Id).ToArray());

            // 7. Спецификация(cmdsoft_specification)
            // Меняем связанные с изменяемыми проектами записи сущности «Спецификации», связь по полю «Проект(Спецификация)»(cmdsoft_specification.cmdsoft_spprojectnumber).
            // Поля не меняем.
            var specificationUpdater = new CmdsoftSpecificationUpdater(_orgService, _sqlConnection, updatedOpportunityIds);
            var updatedSpecifications = specificationUpdater.Process();
            var updatedSpecificationIds = updatedSpecifications.Select(e => e.Id).ToArray();

            // 8. Состав спецификации»(cmdsoft_listspecification)
            // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Состав спецификации»(cmdsoft_listspecification), 
            // связь по полю «Спецификация(Состав спецификации)» (cmdsoft_listspecification.cmdsoft_specification).
            // Поля не меняем.
            var listSpecificationUpdater = new CmdsoftListSpecificationUpdater(_orgService, _sqlConnection, updatedSpecificationIds);
            listSpecificationUpdater.Process();

            // 9. «Коммерческой предложение»(cmdsoft_offer)
            // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Коммерческой предложение»(cmdsoft_offer), 
            // связь по полю «Спецификация(Коммерческой предложение)» (cmdsoft_offer.mcdsoft_offer2).
            // Очистить поле «Прочие условия», все остальные поля не меняем.
            var offerUpdater = new CmdsoftOfferUpdater(_orgService, _sqlConnection, updatedSpecificationIds);
            offerUpdater.Process();

            // 10. «Прайс-лист NAV»(yolva_salesprice)
            // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Состав спецификации»(cmdsoft_listspecification), 
            // связь по полю «Спецификация(Состав спецификации)» (cmdsoft_listspecification.cmdsoft_specification).
            // Меняем только поле «Описание» (yolva_salesprice.yolva_description) - стираем значение..
            var yolvaSalespriceUpdater = new YolvaSalespriceUpdater(_orgService, _sqlConnection, null);
            var updatedYolvaSalesprices = yolvaSalespriceUpdater.Process();
            var updatedYolvaSalespriceIds = updatedYolvaSalesprices.Select(e => e.Id).ToArray();

            // 11. «Строка Прайс-Листа»(yolva_salespriceline)
            // Меняем связанные с изменяемыми записями «Прайс - лист NAV» записи сущности «Строка Прайс-Листа»(yolva_salespriceline), 
            // связь по полю «Прайс - лист(Строка Прайс - Листа)»(yolva_salespriceline.yolva_salespriceid).
            // Меняем только поле «Сумма»(yolva_salespriceline.yolva_amount) = Random(число с плавающей точкой, точность – 2, значения 1000 - 1000)
            var yolvaSalespriceLineUpdater = new YolvaSalespriceLineUpdater(_orgService, _sqlConnection, updatedYolvaSalespriceIds);
            yolvaSalespriceLineUpdater.Process();

            return allUpdated;
        }
    }
}