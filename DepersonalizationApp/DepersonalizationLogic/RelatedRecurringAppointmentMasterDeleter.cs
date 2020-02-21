using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedRecurringAppointmentMasterDeleter : BaseDeleter<RecurringAppointmentMaster>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedRecurringAppointmentMasterDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            RecurringAppointmentMaster[] recurringAppointmentMasters = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    recurringAppointmentMasters = (from recurringAppointmentMaster in _serviceContext.RecurringAppointmentMasterSet
                                                   where recurringAppointmentMaster.RegardingObjectId != null && recurringAppointmentMaster.RegardingObjectId.Id == regObjId
                                                   select recurringAppointmentMaster).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedRecurringAppointmentMasterDeleter.Process query is failed", ex);
                }
                if (recurringAppointmentMasters != null && recurringAppointmentMasters.Length > 0)
                {
                    AllDelete(recurringAppointmentMasters);
                }
            }
        }
    }
}