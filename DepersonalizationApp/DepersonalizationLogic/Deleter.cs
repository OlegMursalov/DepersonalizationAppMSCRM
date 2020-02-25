using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class Deleter
    {
        private IOrganizationService _orgService;
        private SqlConnection _sqlConnection;

        public Deleter(IOrganizationService orgService, SqlConnection sqlConnection)
        {
            _orgService = orgService;
            _sqlConnection = sqlConnection;
        }

        /// <summary>
        /// Удаление всего, что нужно
        /// </summary>
        public void Execute(Dictionary<string, Guid[]> data)
        {
            foreach (var item in data)
            {
                var entityName = item.Key;
                if (entityName == "opportunity" || entityName == "account" || entityName == "contact") // Удаление примечаний и действий для нужных сущностей
                {
                    var ids = item.Value;
                    var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, ids);
                    relatedActivityDeleter.Process();
                    var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, ids);
                    annotationDeleter.Process();
                }
            }
        }
    }
}