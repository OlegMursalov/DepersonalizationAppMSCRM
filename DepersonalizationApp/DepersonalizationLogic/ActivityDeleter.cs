using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ActivityDeleter : BaseDeleter
    {
        public ActivityDeleter(IOrganizationService organizationService, IEnumerable<Guid> Ids) : base(organizationService)
        {
            var mainQuery = new QueryExpression("activity");
            mainQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.In, Ids);
            _mainQuery = mainQuery;
        }
    }
}