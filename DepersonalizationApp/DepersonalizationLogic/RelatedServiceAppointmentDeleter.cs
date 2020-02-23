using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedServiceAppointmentDeleter : BaseDeleter<ServiceAppointment>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedServiceAppointmentDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "serviceappointment";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.ServiceAppointment", regardingObjectIds);
        }
    }
}