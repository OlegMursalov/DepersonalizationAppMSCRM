using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace UpdaterApp.DepersonalizationLogic
{
    public abstract class BaseUpdater<T> : Base<T> where T : Entity
    {
        protected abstract void ChangeByRules(IEnumerable<T> records);

        public BaseUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        public virtual void Process()
        {
            var entities = FastRetrieveAll(_retrieveSqlQuery);
            ChangeByRules(entities);
            AllUpdate(entities);
        }

        /// <summary>
        /// Обновить измененные записи
        /// </summary>
        protected void AllUpdate(IEnumerable<T> entities)
        {
            int successfulAmount = 0;
            var entityName = typeof(T).Name;
            foreach (var entity in entities)
            {
                try
                {
                    _orgService.Update(entity);
                    _logger.Info($"Record '{entityName}' with Id = '{entity.Id}' is updated");
                    successfulAmount++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not updated", ex);
                }
            }
            _logger.Info($"{successfulAmount} records '{entityName}' are updated, {entities.Count() - successfulAmount} are failed");
        }
    }
}