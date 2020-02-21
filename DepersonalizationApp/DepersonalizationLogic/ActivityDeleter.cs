using CRMEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ActivityDeleter : BaseDeleter<ActivityPointer>
    {
        public ActivityDeleter(OrganizationServiceCtx serviceContext) : base(serviceContext)
        {
        }
    }
}