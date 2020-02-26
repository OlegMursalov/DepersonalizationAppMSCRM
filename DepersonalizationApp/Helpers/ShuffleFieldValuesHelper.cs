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
    public class ShuffleFieldValuesHelper<U> where U : Entity
    {
        private Random _random;
        private List<U> _needEntities;
        private Dictionary<string, List<ValWrpr>> _fieldNameValWrps;

        private const int MinRandRange = int.MinValue;
        private const int MaxRandRange = int.MaxValue;

        public ShuffleFieldValuesHelper()
        {
            _random = new Random();
            _needEntities = new List<U>();
            _fieldNameValWrps = new Dictionary<string, List<ValWrpr>>();
        }

        public void AddValue(string fieldName, object value)
        {
            var valWrpr = new ValWrpr
            {
                Value = value,
                Prefix = _random.Next(MinRandRange, MaxRandRange)
            };
            if (!_fieldNameValWrps.ContainsKey(fieldName))
            {
                _fieldNameValWrps.Add(fieldName, new List<ValWrpr> { valWrpr });
            }
            else
            {
                _fieldNameValWrps[fieldName].Add(valWrpr);
            }
        }

        public void AddEntity(U entity)
        {
            _needEntities.Add(entity);
        }

        /// <summary>
        /// Выполняет перетасовку (разбрасывает значения полей в сущностях)
        /// </summary>
        public void Process()
        {
            foreach (var block in _fieldNameValWrps)
            {
                var nameField = block.Key;
                var valWrprs = block.Value;
                var orderedValArr = valWrprs.OrderBy(e => e.Prefix).Select(e => e.Value).ToArray();
                int i = 0;
                foreach (var entity in _needEntities)
                {
                    entity[nameField] = orderedValArr[i];
                    i++;
                }
            }
        }

        private class ValWrpr
        {
            public object Value;
            public int Prefix;
        }
    }
}