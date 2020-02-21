using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedTaskDeleter : BaseDeleter<Task>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedTaskDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            Task[] tasks = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    tasks = (from task in _serviceContext.TaskSet
                             where task.RegardingObjectId != null && task.RegardingObjectId.Id == regObjId
                             select task).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedTaskDeleter.Process query is failed", ex);
                }
                if (tasks != null && tasks.Length > 0)
                {
                    AllDelete(tasks);
                }
            }
        }
    }
}