using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AnnotationDeleter : BaseDeleter
    {
        public AnnotationDeleter(IOrganizationService organizationService, IEnumerable<Guid> Ids) : base(organizationService)
        {
            var mainQuery = new QueryExpression("annotation");
            mainQuery.Criteria.AddCondition("objectid", ConditionOperator.In, Ids);
            _mainQuery = mainQuery;
        }
    }
}