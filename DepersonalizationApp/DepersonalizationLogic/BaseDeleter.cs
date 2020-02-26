using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаляет записи по QueryExpression
    /// </summary>
    public abstract class BaseDeleter<T> : Base<Guid>
    {
        protected string _entityLogicalName;

        protected IEnumerable<Guid> _allRetrievedIds;
        protected IEnumerable<Guid> _allDeletedIds;

        /// <summary>
        /// Возвращает все извлеченные записи после отработки Process
        /// </summary>
        public IEnumerable<Guid> AllRetrievedIds => _allRetrievedIds;

        /// <summary>
        /// Возвращает все удаленные идентификаторы записей после отработки Process
        /// </summary>
        public IEnumerable<Guid> AllDeletedIds => _allDeletedIds;

        public BaseDeleter(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        protected override Guid ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return (Guid)sqlReader.GetValue(0);
        }

        public void Process()
        {
            var entityName = typeof(T).Name;
            _allRetrievedIds = FastRetrieveAllItems();
            if (_allRetrievedIds != null && _allRetrievedIds.Count() > 0)
            {
                DeleteAll(_allRetrievedIds);
            }
            else
            {
                _logger.Info($"Records '{entityName}' by guids are not found for deleting");
            }
        }

        protected void DeleteAll(IEnumerable<Guid> guids)
        {
            int successfulAmount = 0;
            var entityName = typeof(T).Name;
            foreach (var id in guids)
            {
                try
                {
                    _orgService.Delete(_entityLogicalName, id);
                    _logger.Info($"Record '{entityName}' with Id = '{id}' is deleted");
                    successfulAmount++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{id}' is not deleted", ex);
                }
            }
            _logger.Info($"{successfulAmount} records '{entityName}' are deleted, {guids.Count() - successfulAmount} are failed");
        }
    }
}