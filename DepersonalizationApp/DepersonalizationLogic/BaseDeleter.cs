using CRMEntities;
using Microsoft.Xrm.Sdk;
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

        protected void AllDelete(IEnumerable<T> entities)
        {
            int successfulAmount = 0;
            var entityName = typeof(T).Name;
            foreach (var entity in entities)
            {
                try
                {
                    /*_serviceContext.DeleteObject(entity);
                    _serviceContext.SaveChanges();*/
                    // _logger.Info($"Record '{entityName}' with Id = '{entity.Id}' is deleted");
                    successfulAmount++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not deleted", ex);
                }
            }
            _logger.Info($"{successfulAmount} records '{entityName}' are deleted, {entities.Count() - successfulAmount} are failed");
        }
    }
}