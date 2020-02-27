using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных писем
    /// </summary>
    public class RelatedLetterDeleter : BaseDeleter<Letter>
    {
        public RelatedLetterDeleter(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "letter";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Letter", regardingObjectIds);
        }
    }
}