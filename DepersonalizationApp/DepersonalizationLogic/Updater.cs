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
        public Dictionary<string, List<Guid>> Execute(Dictionary<string, List<Guid>> allRetrieved)
        {
            var allUpdated = new Dictionary<string, List<Guid>>();

            if (allRetrieved.ContainsKey("opportunity") && allRetrieved["opportunity"].Count > 0)
            {
                var opportunityUpdater = new OpportunityUpdater(_orgService, _sqlConnection, allRetrieved["opportunity"]);
                var updatedOpportunities = opportunityUpdater.Process();
                allUpdated.Add("opportunity", updatedOpportunities.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_part_of_owner") && allRetrieved["cmdsoft_part_of_owner"].Count > 0)
            {
                var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_part_of_owner"]);
                var updatedPartOfOwners = partOfOwnerUpdater.Process();
                allUpdated.Add("cmdsoft_part_of_owner", updatedPartOfOwners.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_ordernav") && allRetrieved["cmdsoft_ordernav"].Count > 0)
            {
                var cmdsoftOrderNavUpdater = new CmdsoftOrderNavUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_ordernav"]);
                var updatedCmdsoftOrderNavs = cmdsoftOrderNavUpdater.Process();
                allUpdated.Add("cmdsoft_ordernav", updatedCmdsoftOrderNavs.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_orderlinenav") && allRetrieved["cmdsoft_orderlinenav"].Count > 0)
            {
                var cmdsoftOrderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_orderlinenav"]);
                var updatedCmdsoftOrderlineNavs = cmdsoftOrderlineNavUpdater.Process();
                allUpdated.Add("cmdsoft_orderlinenav", updatedCmdsoftOrderlineNavs.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("account") && allRetrieved["account"].Count > 0)
            {
                var accountUpdater = new AccountUpdater(_orgService, _sqlConnection, allRetrieved["account"]);
                var updatedAccounts = accountUpdater.Process();
                allUpdated.Add("account", updatedAccounts.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("contact") && allRetrieved["contact"].Count > 0)
            {
                var contactUpdater = new ContactUpdater(_orgService, _sqlConnection, allRetrieved["contact"]);
                var updatedContacts = contactUpdater.Process();
                allUpdated.Add("contact", updatedContacts.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_specification") && allRetrieved["cmdsoft_specification"].Count > 0)
            {
                var specificationUpdater = new CmdsoftSpecificationUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_specification"]);
                var updatedSpecifications = specificationUpdater.Process();
                allUpdated.Add("cmdsoft_specification", updatedSpecifications.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_listspecification") && allRetrieved["cmdsoft_listspecification"].Count > 0)
            {
                var listSpecificationUpdater = new CmdsoftListSpecificationUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_listspecification"]);
                var updatedListSpecificaions = listSpecificationUpdater.Process();
                allUpdated.Add("cmdsoft_listspecification", updatedListSpecificaions.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("cmdsoft_offer") && allRetrieved["cmdsoft_offer"].Count > 0)
            {
                var offerUpdater = new CmdsoftOfferUpdater(_orgService, _sqlConnection, allRetrieved["cmdsoft_offer"]);
                var updatedOffers = offerUpdater.Process();
                allUpdated.Add("cmdsoft_offer", updatedOffers.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("yolva_salesprice") && allRetrieved["yolva_salesprice"].Count > 0)
            {
                var yolvaSalespriceUpdater = new YolvaSalespriceUpdater(_orgService, _sqlConnection, allRetrieved["yolva_salesprice"]);
                var updatedSalesprices = yolvaSalespriceUpdater.Process();
                allUpdated.Add("yolva_salesprice", updatedSalesprices.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("yolva_salespriceline") && allRetrieved["yolva_salespriceline"].Count > 0)
            {
                var yolvaSalespriceLineUpdater = new YolvaSalespriceLineUpdater(_orgService, _sqlConnection, allRetrieved["yolva_salespriceline"]);
                var updatedSalespriceLines = yolvaSalespriceLineUpdater.Process();
                allUpdated.Add("yolva_salespriceline", updatedSalespriceLines.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("mcdsoft_event") && allRetrieved["mcdsoft_event"].Count > 0)
            {
                var eventUpdater = new McdsoftEventUpdater(_orgService, _sqlConnection, allRetrieved["mcdsoft_event"]);
                var updatedEvents = eventUpdater.Process();
                allUpdated.Add("mcdsoft_event", updatedEvents.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("yolva_events_participants") && allRetrieved["yolva_events_participants"].Count > 0)
            {
                var eventsParticipantsUpdater = new YolvaEventsParticipantsUpdater(_orgService, _sqlConnection, allRetrieved["yolva_events_participants"]);
                var updatedParticipants = eventsParticipantsUpdater.Process();
                allUpdated.Add("yolva_events_participants", updatedParticipants.Select(e => e.Id).ToList());
            }
            if (allRetrieved.ContainsKey("mcdsoft_sales_appeal") && allRetrieved["mcdsoft_sales_appeal"].Count > 0)
            {
                var salesAppealUpdater = new McdsoftSalesAppealUpdater(_orgService, _sqlConnection, allRetrieved["mcdsoft_sales_appeal"]);
                var updatedSalesAppeals = salesAppealUpdater.Process();
                allUpdated.Add("mcdsoft_sales_appeal", updatedSalesAppeals.Select(e => e.Id).ToList());
            }

            return allUpdated;
        }
    }
}