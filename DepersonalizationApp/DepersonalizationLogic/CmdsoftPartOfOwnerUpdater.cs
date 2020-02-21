using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftPartOfOwnerUpdater : BaseUpdater<cmdsoft_part_of_owner>
    {
        protected IEnumerable<Guid> _cmdsoftRefOpportunityIds;

        public CmdsoftPartOfOwnerUpdater(OrganizationServiceCtx serviceContext, IEnumerable<Guid> cmdsoftRefOpportunityIds) : base(serviceContext)
        {
            _cmdsoftRefOpportunityIds = cmdsoftRefOpportunityIds;
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_part_of_owner> cmdsoftPartOfOwners)
        {
            int i = 0;
            var array = RandomRangeHelper.Get(cmdsoftPartOfOwners.Count(), 100);
            foreach (var partOfOwner in cmdsoftPartOfOwners)
            {
                partOfOwner.cmdsoft_part = array[i];
                i++;
            }
        }

        public override void Process()
        {
            foreach (var opportunityId in _cmdsoftRefOpportunityIds)
            {
                cmdsoft_part_of_owner[] cmdsoftPartOfOwners = null;
                try
                {
                    cmdsoftPartOfOwners = (from partOfOwner in _serviceContext.cmdsoft_part_of_ownerSet
                                           where partOfOwner.cmdsoft_ref_opportunity != null && partOfOwner.cmdsoft_ref_opportunity.Id == opportunityId
                                           select partOfOwner).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("CmdsoftPartOfOwnerUpdater.Process query is failed", ex);
                }
                if (cmdsoftPartOfOwners != null)
                {
                    ChangeByRules(cmdsoftPartOfOwners);
                    AllUpdate(cmdsoftPartOfOwners);
                }
            }
        }
    }
}