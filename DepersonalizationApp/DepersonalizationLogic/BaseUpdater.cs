using DepersonalizationApp.DepersonalizationLogic;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

        public BaseUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        /// <summary>
        /// Обновляет необезличенные записи через CRM сервис и возвращает IEnumerable<Guid> успшно обновленных записей
        /// </summary>
        public IEnumerable<T> Process()
        {
            var entityName = typeof(T).Name;
            var updatedList = new List<T>();
            var allRetrievedEntities = FastRetrieveAllItems();
            if (allRetrievedEntities != null && allRetrievedEntities.Count() > 0)
            {
                var filteredEntities = GetOnlyUndepersonalizationEntities(allRetrievedEntities);
                if (filteredEntities != null && filteredEntities.Count() > 0)
                {
                    var changedEntities = ChangeByRules(filteredEntities);
                    updatedList.AddRange(UpdateAll(changedEntities));
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
            return updatedList;
        }

        /// <summary>
        /// Метод возвращает только необезличенные записи
        /// </summary>
        private IEnumerable<T> GetOnlyUndepersonalizationEntities(IEnumerable<T> entities)
        {
            var entityName = typeof(T).Name;
            foreach (var entity in entities)
            {
                if (entity.Attributes.Contains(_isDepersonalizationFieldName))
                {
                    var yolvaIsDepersonalized = entity.Attributes[_isDepersonalizationFieldName] as bool?;
                    if (yolvaIsDepersonalized == null || !yolvaIsDepersonalized.Value)
                    {
                        yield return entity;
                    }
                }
                else
                {
                    var errMsg = $"Entity {entityName} has no {_isDepersonalizationFieldName} field";
                    _logger.Error(errMsg);
                    throw new InvalidOperationException(errMsg);
                }
            }
        }

        private Entity SetIsDepersonalizationFlag(Entity entity)
        {
            entity[_isDepersonalizationFieldName] = true;
            return entity;
        }

        protected IEnumerable<T> UpdateAllInParallel(IEnumerable<T> entities, int amounOfTasks)
        {
            int i = 0;
            var updatedList = new List<T>();
            var inputEntities = entities.ToArray();
            var partsOfEntities = EnumerableDeviderHelper.Split<T>(inputEntities, inputEntities.Length / amounOfTasks);
            var tasks = new Task<IEnumerable<T>>[partsOfEntities.Count()];
            foreach (var entitiesPart in partsOfEntities)
            {
                var task = new Task<IEnumerable<T>>(() =>
                {
                    return UpdateAll(entitiesPart);
                });
                task.Start();
                tasks[i] = task;
                i++;
            }
            Task.WaitAll(tasks);
            for (int j = 0; j < tasks.Length; j++)
            {
                try
                {
                    updatedList.AddRange(tasks[j].Result);
                }
                catch (AggregateException aggrEx)
                {
                    _logger.Error($"UpdateAllInParallel error [{j}]", aggrEx);
                }
            }
            return updatedList;
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
                    entityForUpdate = SetIsDepersonalizationFlag(entityForUpdate);
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