using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных звонков
    /// </summary>
    public class RelatedPhoneCallDeleter : BaseDeleter<PhoneCall>
    {
        public RelatedPhoneCallDeleter(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "phonecall";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.PhoneCall", regardingObjectIds);
        }
    }
}