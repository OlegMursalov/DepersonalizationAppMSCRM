using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаляет записи по QueryExpression
    /// </summary>
    public class BaseDeleter : Base
    {
        public BaseDeleter(IOrganizationService organizationService, QueryExpression mainQuery) : base(organizationService, mainQuery)
        {
        }

        public BaseDeleter(IOrganizationService organizationService) : base(organizationService)
        {
        }

        public void Process()
        {
            RetrieveAll((entities) =>
            {
                AllDelete(entities);
            });
        }

        private void AllDelete(IEnumerable<Entity> records)
        {
            var amountOfSuccessful = 0;
            var entityName = _mainQuery.EntityName;
            foreach (var record in records)
            {
                try
                {
                    // _organizationService.Delete(entityName, record.Id);
                    amountOfSuccessful++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{record.Id}' is not deleted", ex);
                }
            }
            _logger.Info($"Successful deleted '{amountOfSuccessful}' and '{records.Count() - amountOfSuccessful}' are failed records '{entityName}'");
        }
    }
}