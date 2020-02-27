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
        public Dictionary<string, IEnumerable<Guid>> Execute(Dictionary<string, IEnumerable<Guid>> allRetrieved)
        {
            var allUpdated = new Dictionary<string, IEnumerable<Guid>>();

            if (allRetrieved.ContainsKey("opportunity"))
            {
                var opportunityUpdater = new OpportunityUpdater(_orgService, _sqlConnection, allRetrieved["opportunity"]);
                var updatedOpportunities = opportunityUpdater.Process();
                allUpdated.Add("opportunity", updatedOpportunities.Select(e => e.Id));
            }
            if (allRetrieved.ContainsKey("cmdsoft_part_of_owner"))
            {
                var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_part_of_owner"]);
                var updatedPartOfOwners = partOfOwnerUpdater.Process();
                allUpdated.Add("cmdsoft_part_of_owner", updatedPartOfOwners.Select(e => e.Id));
            }
            if (allRetrieved.ContainsKey("cmdsoft_ordernav"))
            {
                var cmdsoftOrderNavUpdater = new CmdsoftOrderNavUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_ordernav"]);
                var updatedCmdsoftOrderNavs = cmdsoftOrderNavUpdater.Process();
                allUpdated.Add("cmdsoft_ordernav", updatedCmdsoftOrderNavs.Select(e => e.Id));
            }
            if (allRetrieved.ContainsKey("cmdsoft_orderlinenav"))
            {
                var cmdsoftOrderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_orderlinenav"]);
                var updatedCmdsoftOrderlineNavs = cmdsoftOrderlineNavUpdater.Process();
                allUpdated.Add("cmdsoft_orderlinenav", updatedCmdsoftOrderlineNavs.Select(e => e.Id));
            }
            if (allRetrieved.ContainsKey("account"))
            {
                var accountUpdater = new AccountUpdater(_orgService, _sqlConnection, allRetrieved["account"]);
                var updatedAccounts = accountUpdater.Process();
                allUpdated.Add("account", updatedAccounts.Select(e => e.Id));
            }
            if (allRetrieved.ContainsKey("contact"))
            {
                var contactUpdater = new ContactUpdater(_orgService, _sqlConnection, allRetrieved["contact"]);
                var updatedContacts = contactUpdater.Process();
                allUpdated.Add("contact", updatedContacts.Select(e => e.Id));
            }
            if ()
            {

            }

            return allUpdated;




                // 6. Контакты(contact)
                // Меняем связанные с изменяемыми проектами и организациями записи сущности «Контакты», 
                // связи по полям «Менеджер заказчика(Проект)»(opportunity.cmdsoft_managerproject), «Дилер(Проект)» (opportunity.cmdsoft_dealer), 
                // Проектировщик(Проект)»(opportunity.cmdsoft_contact_project_agency) «Контакт эксплуатирующей организации(Проект)»(opportunity.mcdsoft_ref_contact)...
                if (uniqueContactGuids.Length > 0)
                {
                    var contactUpdater = new ContactUpdater(_orgService, _sqlConnection, uniqueContactGuids);
                    contactUpdater.Process();
                    allUpdated.Add("contact", contactUpdater.AllRetrievedEntities.Select(e => e.Id).ToArray());
                }

                // 7. Спецификация(cmdsoft_specification)
                // Меняем связанные с изменяемыми проектами записи сущности «Спецификации», связь по полю «Проект(Спецификация)»(cmdsoft_specification.cmdsoft_spprojectnumber).
                // Поля не меняем.
                var specificationUpdater = new CmdsoftSpecificationUpdater(_orgService, _sqlConnection, allRetrievedOpportunityIds);
                specificationUpdater.Process();
                var retrievedSpecificationIds = specificationUpdater.AllRetrievedEntities.Select(e => e.Id).ToArray();

                // 8. Состав спецификации»(cmdsoft_listspecification)
                // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Состав спецификации»(cmdsoft_listspecification), 
                // связь по полю «Спецификация(Состав спецификации)» (cmdsoft_listspecification.cmdsoft_specification).
                // Поля не меняем.
                if (retrievedSpecificationIds.Length > 0)
                {
                    var listSpecificationUpdater = new CmdsoftListSpecificationUpdater(_orgService, _sqlConnection, retrievedSpecificationIds);
                    listSpecificationUpdater.Process();

                    // 9. «Коммерческой предложение»(cmdsoft_offer)
                    // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Коммерческой предложение»(cmdsoft_offer), 
                    // связь по полю «Спецификация(Коммерческой предложение)» (cmdsoft_offer.mcdsoft_offer2).
                    // Очистить поле «Прочие условия», все остальные поля не меняем.
                    var offerUpdater = new CmdsoftOfferUpdater(_orgService, _sqlConnection, retrievedSpecificationIds);
                    offerUpdater.Process();

                    var allRetrievedSpecifications = specificationUpdater.AllRetrievedEntities;

                    // 10. «Прайс-лист NAV»(yolva_salesprice)
                    // Меняем связанные с изменяемыми записями «спецификация» записи сущности «Состав спецификации»(cmdsoft_listspecification), 
                    // связь по полю «Спецификация(Состав спецификации)» (cmdsoft_listspecification.cmdsoft_specification).
                    // Меняем только поле «Описание» (yolva_salesprice.yolva_description) - стираем значение..
                    var salespricenavIds = allRetrievedSpecifications.Where(e => e.yolva_salespricenav != null).Select(e => e.yolva_salespricenav.Id).ToArray();
                    var yolvaSalespriceUpdater = new YolvaSalespriceUpdater(_orgService, _sqlConnection, salespricenavIds);
                    yolvaSalespriceUpdater.Process();
                    var retrievedYolvaSalespriceIds = yolvaSalespriceUpdater.AllRetrievedEntities.Select(e => e.Id).ToArray();

                    // 11. «Строка Прайс-Листа»(yolva_salespriceline)
                    // Меняем связанные с изменяемыми записями «Прайс - лист NAV» записи сущности «Строка Прайс-Листа»(yolva_salespriceline), 
                    // связь по полю «Прайс - лист(Строка Прайс - Листа)»(yolva_salespriceline.yolva_salespriceid).
                    // Меняем только поле «Сумма»(yolva_salespriceline.yolva_amount) = Random(число с плавающей точкой, точность – 2, значения 1000 - 1000)
                    if (retrievedYolvaSalespriceIds.Length > 0)
                    {
                        var yolvaSalespriceLineUpdater = new YolvaSalespriceLineUpdater(_orgService, _sqlConnection, retrievedYolvaSalespriceIds);
                        yolvaSalespriceLineUpdater.Process();
                    }
                }
            }

            // 12. «Мероприятия»(mcdsoft_event)
            // Берем последние по дате создания(поле «СreatedOn») 500 записей сущности «Мероприятие»(mcdsoft_event), и «обезличиваем» по заданному алгоритму ниже.
            var eventUpdater = new McdsoftEventUpdater(_orgService, _sqlConnection);
            eventUpdater.Process();
            var allRetrievedMcdsoftEventIds = eventUpdater.AllRetrievedEntities.Select(e => e.Id).ToArray();

            // В связанных записях сущности «участник мероприятия», по полю «Мероприятие(участник мероприятия)»(yolva_events_participants.yolva_event), поменять поля...
            if (allRetrievedMcdsoftEventIds.Length > 0)
            {
                var eventsParticipantsUpdater = new YolvaEventsParticipantsUpdater(_orgService, _sqlConnection, allRetrievedMcdsoftEventIds);
                eventsParticipantsUpdater.Process();
            }

            // 14. «Сервисное обращение»(mcdsoft_sales_appeal)
            // Берем последние по дате создания(поле «СreatedOn»)  500 записей сущности «Сервисное обращение»(mcdsoft_sales_appeal), и «обезличиваем» по заданному алгоритму ниже.
            var salesAppealUpdater = new McdsoftSalesAppealUpdater(_orgService, _sqlConnection);
            salesAppealUpdater.Process();
            
            return allUpdated;
        }
    }
}