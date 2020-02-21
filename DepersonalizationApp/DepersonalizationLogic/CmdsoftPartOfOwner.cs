using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftPartOfOwner : BaseUpdater<cmdsoft_part_of_owner>
    {
        protected IEnumerable<Guid> _cmdsoftRefOpportunityIds;

        public CmdsoftPartOfOwner(OrganizationServiceCtx serviceContext, IEnumerable<Guid> cmdsoftRefOpportunityIds) : base(serviceContext)
        {
            _cmdsoftRefOpportunityIds = cmdsoftRefOpportunityIds;
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_part_of_owner> cmdsoftPartOfOwners)
        {
            var random = new Random();
            var newParts = new int[cmdsoftPartOfOwners.Count()];
            var maxRandVal = newParts.Length / 100;
            for (int i = 0; i < newParts.Length; i++)
            {
                newParts[i] = random.Next(0, maxRandVal);
            }
        }

        private void GetNewRandomizeParts(int amountOfParts)
        {

        }

        public override void Process()
        {
            foreach (var opportunityId in _cmdsoftRefOpportunityIds)
            {
                var cmdsoftPartOfOwners = (from partOfOwner in _serviceContext.cmdsoft_part_of_ownerSet
                                           where partOfOwner.cmdsoft_ref_opportunity != null && partOfOwner.cmdsoft_ref_opportunity.Id == opportunityId
                                           select partOfOwner).ToArray();
                ChangeByRules(cmdsoftPartOfOwners);
                AllUpdate(cmdsoftPartOfOwners);
            }
        }
    }
}