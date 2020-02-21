using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedAnnotationDeleter : BaseDeleter<Annotation>
    {
        protected IEnumerable<Guid> _objectIds;

        public RelatedAnnotationDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> objectIds) : base(serviceContext)
        {
            _objectIds = objectIds;
        }

        public void Process()
        {
            foreach (var objId in _objectIds)
            {
                var relatedAnnotationsQuery = from annotation in _serviceContext.AnnotationSet
                                              where annotation.ObjectId != null && annotation.ObjectId.Id == objId
                                              select annotation;
                var annotations = RetrieveAll(relatedAnnotationsQuery);
                AllDelete(annotations);
            }
        }
    }
}