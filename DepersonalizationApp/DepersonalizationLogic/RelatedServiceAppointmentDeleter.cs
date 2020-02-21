using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedServiceAppointmentDeleter : BaseDeleter<ServiceAppointment>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedServiceAppointmentDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            ServiceAppointment[] serviceAppointments = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    serviceAppointments = (from serviceAppointment in _serviceContext.ServiceAppointmentSet
                                           where serviceAppointment.RegardingObjectId != null && serviceAppointment.RegardingObjectId.Id == regObjId
                                           select serviceAppointment).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedServiceAppointmentDeleter.Process query is failed", ex);
                }
                if (serviceAppointments != null && serviceAppointments.Length > 0)
                {
                    AllDelete(serviceAppointments);
                }
            }
        }
    }
}