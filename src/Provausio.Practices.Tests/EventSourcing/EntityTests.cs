using System;
using Provausio.Practices.EventSourcing;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class EntityTests
    {
        [Fact]
        public void Equals_DifferentIds_NotEqual()
        {
            // arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var e1 = new TestEntity(id1);
            var e2 = new TestEntity(id2);

            // act
            var areEqual = e1.Equals(e2);

            // assert
            Assert.False(areEqual);
        }

        [Fact]
        public void Equals_SameIds_AreEqual()
        {
            // arrange
            var id1 = Guid.NewGuid();
            var e1 = new TestEntity(id1);
            var e2 = new TestEntity(id1);

            // act
            var areEqual = e1.Equals(e2);

            // assert
            Assert.True(areEqual);
        }

        [Fact]
        public void EqualsOperator_DifferentIds_NotEqual()
        {
            // arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var e1 = new TestEntity(id1);
            var e2 = new TestEntity(id2);

            // act
            var areEqual = e1 == e2;

            // assert
            Assert.False(areEqual);
        }

        [Fact]
        public void EqualsOperator_SameIds_AreEqual()
        {
            // arrange
            var id1 = Guid.NewGuid();
            var e1 = new TestEntity(id1);
            var e2 = new TestEntity(id1);

            // act
            var areEqual = e1 == e2;

            // assert
            Assert.True(areEqual);
        }

        [Fact]
        public void EqualsOperator_NullRightComparison_NotEqual()
        {
            // arrange
            var id1 = Guid.NewGuid();
            var e1 = new TestEntity(id1);

            // act
            var areEqual = e1 == null;

            // assert
            Assert.False(areEqual);
        }

        [Fact]
        public void EqualsOperator_NullLeftComparison_AreEqual()
        {
            // arrange
            TestEntity e1 = null;

            // act
            var areEqual = e1 == null;

            // assert
            Assert.True(areEqual);
        }

        [Fact]
        public void InequalityOperator_NullLeft_IsNotEqual()
        {
            // arrange
            TestEntity e1 = null;
            TestEntity e2 = new TestEntity(Guid.NewGuid());

            // act
            var areNotEqual = e1 != e2;

            // assert
            Assert.True(areNotEqual);
        }

        [Fact]
        public void InequalityOperator_NullRight_IsNotEqual()
        {
            // arrange
            TestEntity e1 = null;
            TestEntity e2 = new TestEntity(Guid.NewGuid());

            // act
            var areNotEqual = e2 != e1;

            // assert
            Assert.True(areNotEqual);
        }

        private class TestEntity : Entity<Guid>
        {
            public TestEntity(Guid id)
            {
                Id = id;
            }
        }
    }
}
