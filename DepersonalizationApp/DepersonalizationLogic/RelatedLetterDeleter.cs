using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedLetterDeleter : BaseDeleter<Letter>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedLetterDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "letter";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Letter", regardingObjectIds);
        }
    }
}