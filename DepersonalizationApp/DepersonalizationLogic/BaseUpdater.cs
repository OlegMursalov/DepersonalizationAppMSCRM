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
        protected static readonly string _isDepersonalizationFieldName = "yolva_is_depersonalized";

        /// <summary>
        /// Каждый потомок определяет правила изменения экземпляра сущности
        /// </summary>
        protected abstract IEnumerable<T> ChangeByRules(IEnumerable<T> records);

        /// <summary>
        /// Для корректного обновления
        /// </summary>
        protected abstract Entity GetEntityForUpdate(T recordForUpdate);

        protected IEnumerable<T> _allRetrievedEntities;
        protected IEnumerable<T> _allUpdatesEntities;

        /// <summary>
        /// Возвращает все извлеченные записи после отработки Process
        /// </summary>
        public IEnumerable<T> AllRetrievedEntities => _allRetrievedEntities;

        /// <summary>
        /// Возвращает все обновленные записи после отработки Process
        /// </summary>
        public IEnumerable<T> AllUpdatesEntities => _allUpdatesEntities;

        public BaseUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        /// <summary>
        /// Обновляет необезличенные записи через CRM сервис и возвращает IEnumerable<Guid> успшно обновленных записей
        /// </summary>
        public void Process()
        {
            var entityName = typeof(T).Name;
            _allRetrievedEntities = FastRetrieveAllItems();
            if (_allRetrievedEntities != null && _allRetrievedEntities.Count() > 0)
            {
                var filteredEntities = GetOnlyUndepersonalizationEntities(_allRetrievedEntities);
                if (filteredEntities != null && filteredEntities.Count() > 0)
                {
                    var changedEntities = ChangeByRules(filteredEntities);
                    _allUpdatesEntities = UpdateAll(changedEntities);
                }
                else
                {
                    _logger.Info($"Undepersonalizated records '{entityName}' are not found for updating");
                }
            }
            else
            {
                _logger.Info($"Records '{entityName}' are not found for filtering by {_isDepersonalizationFieldName} field");
            }
        }

        /// <summary>
        /// Метод возвращает только необезличенные записи
        /// </summary>
        private IEnumerable<T> GetOnlyUndepersonalizationEntities(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.Attributes.Contains(_isDepersonalizationFieldName))
                {
                    var yolvaIsDepersonalized = entity.Attributes[_isDepersonalizationFieldName] as bool?;
                    if (yolvaIsDepersonalized != null && yolvaIsDepersonalized.Value)
                    {
                        yield return entity;
                    }
                }
            }
        }

        private Entity SetIsDepersonalizationFlag(Entity entity)
        {
            entity[_isDepersonalizationFieldName] = true;
            return entity;
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