using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AnnotationDeleter : BaseDeleter<Annotation>
    {
        public AnnotationDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> objectIds) : base(serviceContext)
        {
            _mainQuery = from annotation in _serviceContext.AnnotationSet
                         join objId in objectIds on annotation.ObjectId.Id equals objId
                         select annotation;
        }
    }
}