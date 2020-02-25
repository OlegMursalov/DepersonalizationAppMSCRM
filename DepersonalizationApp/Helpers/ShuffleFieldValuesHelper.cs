using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.Helpers
{
    /// <summary>
    /// Класс для перетасовки значений полей сущностей между собой
    /// </summary>
    /// <typeparam name="U">Тип CRM сущности</typeparam>
    /// <typeparam name="T">Тип CRM поля</typeparam>
    public class ShuffleFieldValuesHelper<U, T> where U : Entity
    {
        private Random _random;
        private List<ValWrpr<T>> _valWrprs;
        private List<U> _needEntities;
        private string _fieldName;

        private const int MinRandRange = int.MinValue;
        private const int MaxRandRange = int.MaxValue;

        public ShuffleFieldValuesHelper(string fieldName)
        {
            _random = new Random();
            _valWrprs = new List<ValWrpr<T>>();
            _needEntities = new List<U>();
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

        public void AddEntity(U entity)
        {
            _needEntities.Add(entity);
        }

        /// <summary>
        /// Выполняет перетасовку (разбрасывает значения полей в сущностях) и возвращает новый IEnumerable<Entity>
        /// </summary>
        public IEnumerable<U> Process()
        {
            var valueArray = _valWrprs.OrderBy(e => e.Prefix).Select(e => e.Value).ToArray();
            var entities = _needEntities.ToArray();
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i][_fieldName] = valueArray[i];
            }
            return entities;
        }

        private class ValWrpr<T>
        {
            public T Value;
            public int Prefix;
        }
    }
}