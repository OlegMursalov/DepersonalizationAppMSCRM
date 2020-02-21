using CRMEntities;
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
    public class BaseDeleter<T> : Base<T> where T : Entity
    {
        public BaseDeleter(OrganizationServiceCtx serviceContext) : base(serviceContext)
        {
        }

        public void Process()
        {
            RetrieveAll((entities) =>
            {
                AllDelete(entities);
            });
        }

        private void AllDelete(IEnumerable<T> entities)
        {
            var firstRecord = entities.FirstOrDefault();
            if (firstRecord != null)
            {
                var entityName = firstRecord.LogicalName;
                var amountOfSuccessful = 0;
                foreach (var entity in entities)
                {
                    try
                    {
                        _serviceContext.DeleteObject(entity);
                        _serviceContext.SaveChanges();
                        amountOfSuccessful++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not deleted", ex);
                    }
                }
                _logger.Info($"Successful deleted '{amountOfSuccessful}' and '{entities.Count() - amountOfSuccessful}' are failed records '{entityName}'");
            }
        }
    }
}