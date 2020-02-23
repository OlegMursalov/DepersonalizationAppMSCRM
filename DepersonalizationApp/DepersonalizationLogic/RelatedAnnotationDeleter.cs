using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Данный класс общий
    /// </summary>
    public class RelatedAnnotationDeleter : BaseDeleter<Annotation>
    {
        public RelatedAnnotationDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] objectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "annotation";
            var sb = new StringBuilder();
            sb.AppendLine("select distinct ann.AnnotationId");
            sb.AppendLine(" from dbo.Annotation as ann");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ann.ObjectId", objectIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }
    }
}