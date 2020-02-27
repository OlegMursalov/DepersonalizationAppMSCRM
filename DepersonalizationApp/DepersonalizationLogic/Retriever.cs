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

        public Dictionary<string, List<Guid>> Execute()
        {
            var allRetrieved = new Dictionary<string, List<Guid>>();

            var opportunityRetriever = new OpportunityRetriever(_sqlConnection);
            var retrievedOpportunityLinks = opportunityRetriever.Process();

            if (retrievedOpportunityLinks != null && retrievedOpportunityLinks.Count() > 0)
            {
                var retrievedOpportunityIds = retrievedOpportunityLinks.Select(e => e.Id);
                allRetrieved["opportunity"].AddRange(retrievedOpportunityIds);

                var partOfOwnerRetriever = new CmdsoftPartOfOwnerRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedPartOfOwnerIds = partOfOwnerRetriever.Process();

                if (retrievedPartOfOwnerIds != null && retrievedPartOfOwnerIds.Count() > 0)
                {
                    allRetrieved["cmdsoft_part_of_owner"].AddRange(retrievedPartOfOwnerIds);
                }

                var orderNavRetriever = new CmdsoftOrderNavRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedOrderNavIds = orderNavRetriever.Process();

                if (retrievedOrderNavIds != null && retrievedOrderNavIds.Count() > 0)
                {
                    allRetrieved["cmdsoft_ordernav"].AddRange(retrievedOrderNavIds);

                    var orderLineNavRetriever = new CmdsoftOrderLineNavRetriever(_sqlConnection, retrievedOrderNavIds);
                    var retrievedOrderLineNavIds = orderLineNavRetriever.Process();

                    if (retrievedOrderLineNavIds != null && retrievedOrderLineNavIds.Count() > 0)
                    {
                        allRetrieved["cmdsoft_orderlinenav"].AddRange(retrievedOrderLineNavIds);
                    }
                }

                var accountRetriever = new AccountRetriever(_sqlConnection, retrievedOpportunityLinks);
                var retrievedAccountLinks = accountRetriever.Process();

                if (retrievedAccountLinks != null && retrievedAccountLinks.Count() > 0)
                {
                    allRetrieved["account"].AddRange(retrievedAccountLinks.Select(e => e.Id));
                    allRetrieved["contact"].AddRange(retrievedAccountLinks.Where(e => e.PrimaryContactId != null).Select(e => e.PrimaryContactId.Value));
                }

                var contactRetriever = new ContactRetriever(_sqlConnection, retrievedOpportunityLinks);
                var retrievedContactLinks = contactRetriever.Process();

                if (retrievedContactLinks != null && retrievedContactLinks.Count() > 0)
                {
                    allRetrieved["contact"].AddRange(retrievedContactLinks.Select(e => e.Id));
                    allRetrieved["account"].AddRange(retrievedContactLinks.Where(e => e.ParentCustomerId != null).Select(e => e.ParentCustomerId.Value));
                }

                var specificationRetriever = new CmdsoftSpecificationRetriever(_sqlConnection, retrievedOpportunityIds);
                var retrievedSpecificationLinks = specificationRetriever.Process();

                if (retrievedSpecificationLinks != null && retrievedSpecificationLinks.Count() > 0)
                {
                    var retrievedSpecificationIds = retrievedSpecificationLinks.Select(e => e.Id);
                    allRetrieved["cmdsoft_specification"].AddRange(retrievedSpecificationIds);

                    var listSpecificationRetriever = new CmdsoftListSpecificationRetriever(_sqlConnection, retrievedSpecificationIds);
                    var retrievedListSpecificationIds = listSpecificationRetriever.Process();

                    if (retrievedListSpecificationIds != null && retrievedListSpecificationIds.Count() > 0)
                    {
                        allRetrieved["cmdsoft_listspecification"].AddRange(retrievedListSpecificationIds);
                    }
                    
                    var offerRetriever = new CmdsoftOfferRetriever(_sqlConnection, retrievedSpecificationIds);
                    var retrievedOfferIds = offerRetriever.Process();

                    if (retrievedOfferIds != null && retrievedOfferIds.Count() > 0)
                    {
                        allRetrieved["cmdsoft_offer"].AddRange(retrievedOfferIds);
                    }
                    
                    var retrievedSalesPriceIds = retrievedSpecificationLinks.Where(e => e.YolvaSalesPrice != null).Select(e => e.YolvaSalesPrice.Value);
                    
                    if (retrievedSalesPriceIds != null && retrievedSalesPriceIds.Count() > 0)
                    {
                        allRetrieved["yolva_salesprice"].AddRange(retrievedSalesPriceIds);

                        var salespriceLineRetriever = new YolvaSalespriceLineRetriever(_sqlConnection, retrievedSalesPriceIds);
                        var retrievedSalespriceLineIds = salespriceLineRetriever.Process();

                        if (retrievedSalespriceLineIds != null && retrievedSalespriceLineIds.Count() > 0)
                        {
                            allRetrieved["yolva_salespriceline"].AddRange(retrievedSalespriceLineIds);
                        }
                    }
                }
            }

            var eventRetriever = new McdsoftEventRetriever(_sqlConnection);
            var retrievedEventIds = eventRetriever.Process();

            if (retrievedEventIds != null && retrievedEventIds.Count() > 0)
            {
                allRetrieved["mcdsoft_event"].AddRange(retrievedEventIds);

                var eventsParticipantsRetriever = new YolvaEventsParticipantsRetriever(_sqlConnection, retrievedEventIds);
                var retrievedEventsParticipantsLinks = eventsParticipantsRetriever.Process();

                if (retrievedEventsParticipantsLinks != null && retrievedEventsParticipantsLinks.Count() > 0)
                {
                    var retrievedEventsParticipantsIds = retrievedEventsParticipantsLinks.Select(e => e.Id);
                    allRetrieved["yolva_events_participants"].AddRange(retrievedEventsParticipantsIds);

                    var accountRetriever = new AccountRetriever(_sqlConnection, retrievedEventsParticipantsLinks);
                    var retrievedAccountLinks = accountRetriever.Process();

                    if (retrievedAccountLinks != null && retrievedAccountLinks.Count() > 0)
                    {
                        allRetrieved["account"].AddRange(retrievedAccountLinks.Select(e => e.Id));
                        allRetrieved["contact"].AddRange(retrievedAccountLinks.Where(e => e.PrimaryContactId != null).Select(e => e.PrimaryContactId.Value));
                    }

                    var contactRetriever = new ContactRetriever(_sqlConnection, retrievedEventsParticipantsLinks);
                    var retrievedContactLinks = contactRetriever.Process();

                    if (retrievedContactLinks != null && retrievedContactLinks.Count() > 0)
                    {
                        allRetrieved["contact"].AddRange(retrievedContactLinks.Select(e => e.Id));
                        allRetrieved["account"].AddRange(retrievedContactLinks.Where(e => e.ParentCustomerId != null).Select(e => e.ParentCustomerId.Value));
                    }
                }
            }

            var salesAppealRetriever = new McdsoftSalesAppealRetriever(_sqlConnection);
            var salesAppealLinks = salesAppealRetriever.Process();

            if (salesAppealLinks != null && salesAppealLinks.Count() > 0)
            {
                
            }
        }
    }
}