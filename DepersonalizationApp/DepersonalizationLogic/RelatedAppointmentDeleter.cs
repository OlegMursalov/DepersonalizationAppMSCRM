using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedAppointmentDeleter : BaseDeleter<Appointment>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedAppointmentDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            Appointment[] appointments = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    appointments = (from appointment in _serviceContext.AppointmentSet
                                    where appointment.RegardingObjectId != null && appointment.RegardingObjectId.Id == regObjId
                                    select appointment).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedAppointmentDeleter.Process query is failed", ex);
                }
                if (appointments != null && appointments.Length > 0)
                {
                    AllDelete(appointments);
                }
            }
        }
    }
}