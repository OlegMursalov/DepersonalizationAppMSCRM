using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных recurringappointmentmasters
    /// </summary>
    public class RelatedRecurringAppointmentMasterDeleter : BaseDeleter<RecurringAppointmentMaster>
    {
        public RelatedRecurringAppointmentMasterDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "recurringappointmentmaster";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.RecurringAppointmentMaster", regardingObjectIds);
        }
    }
}