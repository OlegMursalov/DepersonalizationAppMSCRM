using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace UpdaterApp.DepersonalizationLogic
{
    public abstract class BaseUpdater<T> : Base<T> where T : Entity
    {
        /// <summary>
        /// Каждый subclass пусть сам определяет, что ему сделать с пачкой записей
        /// </summary>
        protected abstract void ChangeByRules(IEnumerable<T> records);

        public BaseUpdater(OrganizationServiceCtx serviceContext) : base(serviceContext)
        {
        }

        public virtual void Process()
        {
            RetrieveAll((entities) =>
            {
                ChangeByRules(entities);
                AllUpdate(entities);
            });
        }

        /// <summary>
        /// Обновить измененные записи
        /// </summary>
        protected void AllUpdate(IEnumerable<T> entities)
        {
            var entityName = nameof(T);
            foreach (var entity in entities)
            {
                try
                {
                    _serviceContext.UpdateObject(entity);
                    _serviceContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not updated", ex);
                }
            }
        }
    }
}