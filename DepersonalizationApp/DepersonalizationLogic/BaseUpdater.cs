using UpdaterApp.Log;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk.Client;
using CRMEntities;

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

        public void Process()
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
        private void AllUpdate(IEnumerable<T> entities)
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
                        // _organizationService.Update(record);
                        amountOfSuccessful++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not updated", ex);
                    }
                }
                _logger.Info($"Successful updated '{amountOfSuccessful}' and '{entities.Count() - amountOfSuccessful}' are failed records '{entityName}'");
            }
        }
    }
}