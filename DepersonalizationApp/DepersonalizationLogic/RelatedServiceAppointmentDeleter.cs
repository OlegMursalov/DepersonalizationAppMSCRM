using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных service appointments
    /// </summary>
    public class RelatedServiceAppointmentDeleter : BaseDeleter<ServiceAppointment>
    {
        public RelatedServiceAppointmentDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "serviceappointment";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.ServiceAppointment", regardingObjectIds);
        }
    }
}