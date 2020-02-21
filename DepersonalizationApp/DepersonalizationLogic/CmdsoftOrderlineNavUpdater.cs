using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOrderlineNavUpdater : BaseUpdater<cmdsoft_orderlinenav>
    {
        protected IEnumerable<Guid> _cmdsoftRefOpportunityIds;

        public CmdsoftOrderlineNavUpdater(OrganizationServiceCtx serviceContext, IEnumerable<Guid> cmdsoftRefOpportunityIds) : base(serviceContext)
        {
            _cmdsoftRefOpportunityIds = cmdsoftRefOpportunityIds;
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_orderlinenav> cmdsoftOrderineNavs)
        {
            var randN = new Random().Next(1, 10);
            foreach (var orderineNav in cmdsoftOrderineNavs)
            {
                if (orderineNav.mcdsoft_price_discount_with_VAT != null)
                {
                    orderineNav.mcdsoft_price_discount_with_VAT /= randN;
                }
                if (orderineNav.mcdsoft_price_discount_without_VAT != null)
                {
                    orderineNav.mcdsoft_price_discount_without_VAT /= randN;
                }
                if (orderineNav.mcdsoft_price_without_vat != null)
                {
                    orderineNav.mcdsoft_price_without_vat /= randN;
                }
                if (orderineNav.cmdsoft_amountsalesvat != null)
                {
                    orderineNav.cmdsoft_amountsalesvat /= randN;
                }
                if (orderineNav.cmdsoft_amountsale != null)
                {
                    orderineNav.cmdsoft_amountsale /= randN;
                }
            }
        }

        public override void Process()
        {
            foreach (var opportunityId in _cmdsoftRefOpportunityIds)
            {
                var cmdsoftOrderineNavs = (from orderineNav in _serviceContext.cmdsoft_orderlinenavSet
                                           where orderineNav.cmdsoft_ref_opportunity != null && orderineNav.cmdsoft_ref_opportunity.Id == opportunityId
                                           select orderineNav).ToArray();
                ChangeByRules(cmdsoftOrderineNavs);
                AllUpdate(cmdsoftOrderineNavs);
            }
        }
    }
}