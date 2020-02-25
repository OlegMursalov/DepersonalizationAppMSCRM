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
        /// <summary>
        /// Каждый потомок определяет правила изменения экземпляра сущности
        /// </summary>
        protected abstract void ChangeByRules(IEnumerable<T> records);

        public BaseUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        /// <summary>
        /// Обновляет записи через CRM сервис и возвращает IEnumerable<Guid> успшно обновленных записей
        /// </summary>
        public IEnumerable<Guid> Process()
        {
            var entities = FastRetrieveAllItems();
            ChangeByRules(entities);
            return UpdateAll(entities);
        }

        protected IEnumerable<Guid> UpdateAll(IEnumerable<T> entities)
        {
            var entityName = typeof(T).Name;
            var updatedList = new List<Guid>();
            foreach (var entity in entities)
            {
                try
                {
                    // _orgService.Update(entity);
                    _logger.Info($"Record '{entityName}' with Id = '{entity.Id}' is updated");
                    updatedList.Add(entity.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not updated", ex);
                }
            }
            _logger.Info($"{updatedList.Count} records '{entityName}' are updated, {entities.Count() - updatedList.Count} are failed");
            return updatedList;
        }
    }
}