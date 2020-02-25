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

        public void Execute(Dictionary<string, Guid[]> data)
        {
            foreach (var item in data)
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