using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.Log;

namespace DepersonalizationApp.Helpers
{
    /// <summary>
    /// Класс для перетасовки значений полей сущностей между собой
    /// </summary>
    /// <typeparam name="T">Тип CRM поля</typeparam>
    public class ShuffleFieldValuesHelper<T>
    {
        private readonly ILogger _logger = CommonObjsHelper.Logger;

        private Random _random;
        private List<ValWrpr<T>> _valWrprs;
        private List<Entity> _needEntities;
        private string _fieldName;

        private const int MinRandRange = 0;
        private const int MaxRandRange = 0;

        public ShuffleFieldValuesHelper(string fieldName)
        {
            _random = new Random();
            _valWrprs = new List<ValWrpr<T>>();
            _needEntities = new List<Entity>();
            _fieldName = fieldName;
        }

        public void AddValue(T value)
        {
            _valWrprs.Add(new ValWrpr<T>
            {
                Value = value,
                Prefix = _random.Next(MinRandRange, MaxRandRange)
            });
        }

        public void AddEntity(Entity entity)
        {
            _needEntities.Add(entity);
        }

        /// <summary>
        /// Выполняет перетасовку (разбрасывает значения полей в сущностях)
        /// </summary>
        public void Process()
        {
            var valueArray = _valWrprs.OrderBy(e => e.Prefix).Select(e => e.Value).ToArray();

            if (valueArray.Length != _needEntities.Count)
            {
                _logger.Error("ShuffleFieldValuesHelper.Process - amount of values aren't equal amount of need entitites");
                return;
            }

            var i = 0;
            foreach (var entity in _needEntities)
            {
                entity[_fieldName] = valueArray[i];
                i++;
            }
        }

        private class ValWrpr<T>
        {
            public T Value;
            public int Prefix;
        }
    }
}