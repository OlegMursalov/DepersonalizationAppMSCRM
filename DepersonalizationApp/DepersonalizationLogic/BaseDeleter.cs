using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

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
            var entityName = nameof(T);
            foreach (var entity in entities)
            {
                try
                {
                    _serviceContext.DeleteObject(entity);
                    _serviceContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not deleted", ex);
                }
            }
        }
    }
}