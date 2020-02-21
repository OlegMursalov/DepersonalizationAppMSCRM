using CRMEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AnnotationDeleter : BaseDeleter<Annotation>
    {
        public AnnotationDeleter(OrganizationServiceCtx serviceContext) : base(serviceContext)
        {
        }
    }
}