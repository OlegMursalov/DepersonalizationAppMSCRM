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
        public void Execute(Dictionary<string, List<Guid>> allRetrieved)
        {
            if (allRetrieved.ContainsKey("opportunity"))
            {
                var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, allRetrieved["opportunity"]);
                relatedActivityDeleter.Process();
                var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, allRetrieved["opportunity"]);
                annotationDeleter.Process();
            }
            if (allRetrieved.ContainsKey("account"))
            {
                var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, allRetrieved["account"]);
                relatedActivityDeleter.Process();
                var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, allRetrieved["account"]);
                annotationDeleter.Process();
            }
            if (allRetrieved.ContainsKey("contact"))
            {
                var relatedActivityDeleter = new RelatedActivityDeleter(_orgService, _sqlConnection, allRetrieved["contact"]);
                relatedActivityDeleter.Process();
                var annotationDeleter = new RelatedAnnotationDeleter(_orgService, _sqlConnection, allRetrieved["contact"]);
                annotationDeleter.Process();
            }
        }
    }
}