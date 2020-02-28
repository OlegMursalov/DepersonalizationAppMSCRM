using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class Retriever
    {
        private SqlConnection _sqlConnection;

        public Retriever(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        private Dictionary<string, List<Guid>> AllDistinct(Dictionary<string, List<Guid>> allRetrieved)
        {
            var allRetrievedDistinct = new Dictionary<string, List<Guid>>();
            foreach (var item in allRetrieved)
            {
                allRetrievedDistinct.Add(item.Key, item.Value.Distinct().ToList());
            }
            return allRetrievedDistinct;
        }

        private void ExecuteForOpportunity_Part1(Dictionary<string, List<Guid>> allRetrieved, IEnumerable<Guid> opportunityIds = null)
        {
            var opportunityRetriever = opportunityIds != null ? new OpportunityRetriever(_sqlConnection, opportunityIds) : new OpportunityRetriever(_sqlConnection);
            var retrievedOpportunityLinks = opportunityRetriever.Process().Take(2000);

            if (retrievedOpportunityLinks != null && retrievedOpportunityLinks.Count() > 0)
            {
                var retrievedOpportunityIds = retrievedOpportunityLinks.Select(e => e.Id).Distinct();
                allRetrieved["opportunity"].AddRange(retrievedOpportunityIds);

                var partOfOwnerRetriever = new CmdsoftPartOfOwnerRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedPartOfOwnerIds = partOfOwnerRetriever.Process().Distinct();

                if (retrievedPartOfOwnerIds != null && retrievedPartOfOwnerIds.Count() > 0)
                {
                    allRetrieved["cmdsoft_part_of_owner"].AddRange(retrievedPartOfOwnerIds);
                }

                var orderNavRetriever = new CmdsoftOrderNavRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedOrderNavIds = orderNavRetriever.Process().Distinct();

                if (retrievedOrderNavIds != null && retrievedOrderNavIds.Count() > 0)
                {
                    allRetrieved["cmdsoft_ordernav"].AddRange(retrievedOrderNavIds);

                    var cmdsoftOrderLineNavRetriever = new CmdsoftOrderLineNavRetriever(_sqlConnection, retrievedOrderNavIds);
                    var retrievedCmdsoftOrderLineNavLinks = cmdsoftOrderLineNavRetriever.Process().Distinct();

                    if (retrievedCmdsoftOrderLineNavLinks != null && retrievedCmdsoftOrderLineNavLinks.Count() > 0)
                    {
                        allRetrieved["cmdsoft_orderlinenav"].AddRange(retrievedCmdsoftOrderLineNavLinks.Select(e => e.Id).Distinct());
                        allRetrieved["mcdsoft_sales_appeal"].AddRange(retrievedCmdsoftOrderLineNavLinks.Where(e => e.McdsoftRefOrderlineNav != null).Select(e => e.McdsoftRefOrderlineNav.Value).Distinct());
                    }
                }

                var accountRetriever = new AccountRetriever(_sqlConnection, retrievedOpportunityLinks);
                var retrievedAccountLinks = accountRetriever.Process();

                if (retrievedAccountLinks != null && retrievedAccountLinks.Count() > 0)
                {
                    allRetrieved["account"].AddRange(retrievedAccountLinks.Select(e => e.Id).Distinct());
                    allRetrieved["contact"].AddRange(retrievedAccountLinks.Where(e => e.PrimaryContactId != null).Select(e => e.PrimaryContactId.Value).Distinct());
                }

                var contactRetriever = new ContactRetriever(_sqlConnection, retrievedOpportunityLinks);
                var retrievedContactLinks = contactRetriever.Process();

                if (retrievedContactLinks != null && retrievedContactLinks.Count() > 0)
                {
                    allRetrieved["contact"].AddRange(retrievedContactLinks.Select(e => e.Id).Distinct());
                    allRetrieved["account"].AddRange(retrievedContactLinks.Where(e => e.ParentCustomerId != null).Select(e => e.ParentCustomerId.Value).Distinct());
                }

                var specificationRetriever = new CmdsoftSpecificationRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedSpecificationLinks = specificationRetriever.Process();

                if (retrievedSpecificationLinks != null && retrievedSpecificationLinks.Count() > 0)
                {
                    var retrievedSpecificationIds = retrievedSpecificationLinks.Select(e => e.Id).Distinct();
                    allRetrieved["cmdsoft_specification"].AddRange(retrievedSpecificationIds);

                    var listSpecificationRetriever = new CmdsoftListSpecificationRetriever(_sqlConnection, retrievedSpecificationIds);
                    var retrievedListSpecificationIds = listSpecificationRetriever.Process().Distinct();

                    if (retrievedListSpecificationIds != null && retrievedListSpecificationIds.Count() > 0)
                    {
                        allRetrieved["cmdsoft_listspecification"].AddRange(retrievedListSpecificationIds);
                    }

                    var offerRetriever = new CmdsoftOfferRetriever(_sqlConnection, retrievedSpecificationIds);
                    var retrievedOfferIds = offerRetriever.Process().Distinct();

                    if (retrievedOfferIds != null && retrievedOfferIds.Count() > 0)
                    {
                        allRetrieved["cmdsoft_offer"].AddRange(retrievedOfferIds);
                    }

                    var retrievedSalesPriceIds = retrievedSpecificationLinks.Where(e => e.YolvaSalesPrice != null).Select(e => e.YolvaSalesPrice.Value).Distinct();

                    if (retrievedSalesPriceIds != null && retrievedSalesPriceIds.Count() > 0)
                    {
                        allRetrieved["yolva_salesprice"].AddRange(retrievedSalesPriceIds);

                        var salespriceLineRetriever = new YolvaSalespriceLineRetriever(_sqlConnection, retrievedSalesPriceIds);
                        var retrievedSalespriceLineIds = salespriceLineRetriever.Process().Distinct();

                        if (retrievedSalespriceLineIds != null && retrievedSalespriceLineIds.Count() > 0)
                        {
                            allRetrieved["yolva_salespriceline"].AddRange(retrievedSalespriceLineIds);
                        }
                    }
                }
            }
        }

        private void ExecuteForMcdsoftEvent_Part2(Dictionary<string, List<Guid>> allRetrieved)
        {
            var eventRetriever = new McdsoftEventRetriever(_sqlConnection);
            var retrievedEventIds = eventRetriever.Process().Distinct().Take(10);

            if (retrievedEventIds != null && retrievedEventIds.Count() > 0)
            {
                allRetrieved["mcdsoft_event"].AddRange(retrievedEventIds);

                var eventsParticipantsRetriever = new YolvaEventsParticipantsRetriever(_sqlConnection, retrievedEventIds);
                var retrievedEventsParticipantsLinks = eventsParticipantsRetriever.Process();

                if (retrievedEventsParticipantsLinks != null && retrievedEventsParticipantsLinks.Count() > 0)
                {
                    var retrievedEventsParticipantsIds = retrievedEventsParticipantsLinks.Select(e => e.Id).Distinct();
                    allRetrieved["yolva_events_participants"].AddRange(retrievedEventsParticipantsIds);

                    var accountRetriever = new AccountRetriever(_sqlConnection, retrievedEventsParticipantsLinks);
                    var retrievedAccountLinks = accountRetriever.Process();

                    if (retrievedAccountLinks != null && retrievedAccountLinks.Count() > 0)
                    {
                        allRetrieved["account"].AddRange(retrievedAccountLinks.Select(e => e.Id).Distinct());
                        allRetrieved["contact"].AddRange(retrievedAccountLinks.Where(e => e.PrimaryContactId != null).Select(e => e.PrimaryContactId.Value).Distinct());
                    }

                    var contactRetriever = new ContactRetriever(_sqlConnection, retrievedEventsParticipantsLinks);
                    var retrievedContactLinks = contactRetriever.Process();

                    if (retrievedContactLinks != null && retrievedContactLinks.Count() > 0)
                    {
                        allRetrieved["contact"].AddRange(retrievedContactLinks.Select(e => e.Id).Distinct());
                        allRetrieved["account"].AddRange(retrievedContactLinks.Where(e => e.ParentCustomerId != null).Select(e => e.ParentCustomerId.Value).Distinct());
                    }
                }
            }
        }

        private void ExecuteForMcdsoftSalesAppeal_Part3(Dictionary<string, List<Guid>> allRetrieved)
        {
            var salesAppealRetriever = new McdsoftSalesAppealRetriever(_sqlConnection);
            var retrievedSalesAppealLinks = salesAppealRetriever.Process().Take(50);

            if (retrievedSalesAppealLinks != null && retrievedSalesAppealLinks.Count() > 0)
            {
                var retrievedSalesAppealIds = retrievedSalesAppealLinks.Select(e => e.Id).Distinct();
                allRetrieved["mcdsoft_sales_appeal"].AddRange(retrievedSalesAppealIds);

                var accountRetriever = new AccountRetriever(_sqlConnection, retrievedSalesAppealLinks);
                var retrievedAccountLinks = accountRetriever.Process();

                if (retrievedAccountLinks != null && retrievedAccountLinks.Count() > 0)
                {
                    allRetrieved["account"].AddRange(retrievedAccountLinks.Select(e => e.Id).Distinct());
                    allRetrieved["contact"].AddRange(retrievedAccountLinks.Where(e => e.PrimaryContactId != null).Select(e => e.PrimaryContactId.Value).Distinct());
                }

                var contactRetriever = new ContactRetriever(_sqlConnection, retrievedSalesAppealLinks);
                var retrievedContactLinks = contactRetriever.Process();

                if (retrievedContactLinks != null && retrievedContactLinks.Count() > 0)
                {
                    allRetrieved["contact"].AddRange(retrievedContactLinks.Select(e => e.Id).Distinct());
                    allRetrieved["account"].AddRange(retrievedContactLinks.Where(e => e.ParentCustomerId != null).Select(e => e.ParentCustomerId.Value).Distinct());
                }

                var opportunityIds = retrievedSalesAppealLinks.Where(e => e.McdsoftRefOpportunity != null).Select(e => e.McdsoftRefOpportunity.Value).Distinct();
                if (opportunityIds != null && opportunityIds.Count() > 0)
                {
                    ExecuteForOpportunity_Part1(allRetrieved, opportunityIds);
                }

                var cmdsoftRefOrderlinenavIds = retrievedSalesAppealLinks.Where(e => e.CmdsoftRefOrderlinenav != null).Select(e => e.CmdsoftRefOrderlinenav.Value).Distinct();
                if (cmdsoftRefOrderlinenavIds != null && cmdsoftRefOrderlinenavIds.Count() > 0)
                {
                    var cmdsoftOrderLineNavRetriever = new CmdsoftOrderLineNavRetriever(_sqlConnection, cmdsoftRefOrderlinenavIds);
                    var retrievedCmdsoftOrderLineNavLinks = cmdsoftOrderLineNavRetriever.Process().Distinct();

                    if (retrievedCmdsoftOrderLineNavLinks != null && retrievedCmdsoftOrderLineNavLinks.Count() > 0)
                    {
                        allRetrieved["cmdsoft_orderlinenav"].AddRange(retrievedCmdsoftOrderLineNavLinks.Select(e => e.Id).Distinct());
                        allRetrieved["mcdsoft_sales_appeal"].AddRange(retrievedCmdsoftOrderLineNavLinks.Where(e => e.McdsoftRefOrderlineNav != null).Select(e => e.McdsoftRefOrderlineNav.Value).Distinct());
                    }
                }

                var mcdsoftRefOrderlinenavIds = retrievedSalesAppealLinks.Where(e => e.McdsoftRefOrderlinenav != null).Select(e => e.McdsoftRefOrderlinenav.Value).Distinct();
                if (mcdsoftRefOrderlinenavIds != null && mcdsoftRefOrderlinenavIds.Count() > 0)
                {
                    var mcdsoftOrderLineNavRetriever = new McdsoftOrderLineNavRetriever(_sqlConnection, cmdsoftRefOrderlinenavIds);
                    var retrievedMcdsoftOrderLineNavLinks = mcdsoftOrderLineNavRetriever.Process().Distinct();

                    if (retrievedMcdsoftOrderLineNavLinks != null && retrievedMcdsoftOrderLineNavLinks.Count() > 0)
                    {
                        allRetrieved["mcdsoft_orderlinenav"].AddRange(retrievedMcdsoftOrderLineNavLinks.Select(e => e.Id).Distinct());
                        allRetrieved["cmdsoft_sales_appeal"].AddRange(retrievedMcdsoftOrderLineNavLinks.Where(e => e.CmdsoftRefOrderlineNav != null).Select(e => e.CmdsoftRefOrderlineNav.Value).Distinct());
                    }
                }

                var cmdsoftOrderLineNavRetrieverNew = new CmdsoftOrderLineNavRetriever(_sqlConnection, retrievedSalesAppealLinks);
                var retrievedCmdsoftOrderLineNavNewIds = cmdsoftOrderLineNavRetrieverNew.Process().Distinct();
            }
        }

        public Dictionary<string, List<Guid>> Execute()
        {
            var allRetrieved = new Dictionary<string, List<Guid>>();

            allRetrieved.Add("opportunity", new List<Guid>());
            allRetrieved.Add("cmdsoft_part_of_owner", new List<Guid>());
            allRetrieved.Add("cmdsoft_ordernav", new List<Guid>());
            allRetrieved.Add("cmdsoft_orderlinenav", new List<Guid>());
            allRetrieved.Add("account", new List<Guid>());
            allRetrieved.Add("contact", new List<Guid>());
            allRetrieved.Add("cmdsoft_specification", new List<Guid>());
            allRetrieved.Add("cmdsoft_listspecification", new List<Guid>());
            allRetrieved.Add("cmdsoft_offer", new List<Guid>());
            allRetrieved.Add("yolva_salesprice", new List<Guid>());
            allRetrieved.Add("yolva_salespriceline", new List<Guid>());
            allRetrieved.Add("mcdsoft_event", new List<Guid>());
            allRetrieved.Add("yolva_events_participants", new List<Guid>());
            allRetrieved.Add("mcdsoft_sales_appeal", new List<Guid>());
            allRetrieved.Add("mcdsoft_orderlinenav", new List<Guid>());

            ExecuteForOpportunity_Part1(allRetrieved);

            ExecuteForMcdsoftEvent_Part2(allRetrieved);

            ExecuteForMcdsoftSalesAppeal_Part3(allRetrieved);

            return AllDistinct(allRetrieved);
        }
    }
}