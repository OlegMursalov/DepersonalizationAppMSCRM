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
        /// Состояние деперсонализации объекта
        /// </summary>
        protected static readonly string _commonDepersonalizationNameField = "yolva_is_depersonalized";

        /// <summary>
        /// Каждый потомок определяет правила изменения экземпляра сущности
        /// </summary>
        protected abstract void ChangeByRules(IEnumerable<T> records);

        /// <summary>
        /// Для корректного обновления
        /// </summary>
        protected abstract Entity GetEntityForUpdate(T record);

        public BaseUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        /// <summary>
        /// Обновляет записи через CRM сервис и возвращает IEnumerable<Guid> успшно обновленных записей
        /// </summary>
        public IEnumerable<T> Process()
        {
            var entities = FastRetrieveAllItems();
            ChangeByRules(entities);
            return UpdateAll(entities);
        }

        protected IEnumerable<T> UpdateAll(IEnumerable<T> entities)
        {
            var entityName = typeof(T).Name;
            var updatedList = new List<T>();
            foreach (var entity in entities)
            {
                try
                {
                    var entityForUpdate = GetEntityForUpdate(entity);
                    _orgService.Update(entityForUpdate);
                    _logger.Info($"Record '{entityName}' with Id = '{entity.Id}' is updated");
                    updatedList.Add(entity);
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