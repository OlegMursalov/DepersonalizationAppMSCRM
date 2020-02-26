using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DepersonalizationApp.Tests
{
    public class ShuffleFieldValuesHelperTest
    {
        private class TestEntity : Entity
        {
            public TestEntity(float someField)
            {
                SomeField = someField;
            }

            public float SomeField { get; set; }
        }

        private class TestEntityComparer : IEqualityComparer<TestEntity>
        {
            public bool Equals(TestEntity x, TestEntity y)
            {
                return x.SomeField == y.SomeField;
            }

            public int GetHashCode(TestEntity testEntity)
            {
                return testEntity.GetHashCode();
            }
        }

        [Fact]
        public void Is_Random_Order()
        {
            IEnumerable<TestEntity> entities = new TestEntity[]
            {
                new TestEntity(23.3245F),
                new TestEntity(56.12345F),
                new TestEntity(77.89F),
                new TestEntity(0),
                new TestEntity(0.4563467F),
                new TestEntity(456.23234F),
                new TestEntity(4.124F),
                new TestEntity(0.00045634F),
                new TestEntity(777.777F),
                new TestEntity(-235.235325326464F),
            };
            var oldEntities = entities.Select(e => new TestEntity(e.SomeField)).ToArray();

            var shuffleFieldValuesHelper = new ShuffleFieldValuesHelper<TestEntity>();
            foreach (var entity in entities)
            {
                shuffleFieldValuesHelper.AddEntity(entity);
                shuffleFieldValuesHelper.AddValue("SomeField", entity.SomeField);
            }
            shuffleFieldValuesHelper.Process();

            Assert.Equal(oldEntities, entities, new TestEntityComparer());
        }
    }
}