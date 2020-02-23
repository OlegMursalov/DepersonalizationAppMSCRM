using CRMEntities;
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
            sb.AppendLine(" where ann.ObjectId in (");
            for (int i = 0; i < objectIds.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"'{objectIds[i]}'");
                }
                else
                {
                    sb.Append($", '{objectIds[i]}'");
                }
            }
            sb.Append(")");

            _retrieveSqlQuery = sb.ToString();
        }
    }
}