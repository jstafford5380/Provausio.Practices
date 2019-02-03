using System;
using DAS.Infrastructure;
using Provausio.Practices.DDD;
using Xunit;

namespace Provausio.Practices.Tests.DDD
{
    public class ValueObjectTests
    {
        [Fact]
        public void AddressEqualsWorksWithIdenticalAddresses()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");
            
            // act

            // assert
            Assert.True(address.Equals(address2));
        }

        [Fact]
        public void AddressEqualsWorksWithNonIdenticalAddresses()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            // act

            // assert
            Assert.False(address.Equals(address2));
        }

        [Fact]
        public void AddressEqualsWorksWithNulls()
        {
            // arranage
            var address = new Address(null, "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            // act

            // assert
            Assert.False(address.Equals(address2));
        }
        
        [Fact]
        public void AddressEqualsWorksWithNullsOnOtherObject()
        {
            // arrange
            var address = new Address("Address2", "Austin", "TX");
            var address2 = new Address("Address2", null, "TX");

            // act

            // assert
            Assert.False(address.Equals(address2));
        }



        [Fact]
        public void AddressEqualsIsReflexive()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");

            // act

            // assert
            Assert.True(address.Equals(address));
        }



        [Fact]
        public void AddressEqualsIsSymmetric()
        {
            // arrange
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");

            // act

            // assert
            Assert.False(address.Equals(address2));
            Assert.False(address2.Equals(address));
        }

        [Fact]
        public void AddressEqualsIsTransitive()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");
            var address3 = new Address("Address1", "Austin", "TX");

            // act

            // assert
            Assert.True(address.Equals(address2));
            Assert.True(address2.Equals(address3));
            Assert.True(address.Equals(address3));
        }

        [Fact]
        public void AddressOperatorsWork()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");
            var address3 = new Address("Address2", "Austin", "TX");

            // act

            // assert
            Assert.True(address == address2);
            Assert.True(address2 != address3);
        }

        [Fact]
        public void DerivedTypesBehaveCorrectly()
        {
            // arrange
            Address address = new Address("Address1", "Austin", "TX");
            ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            // act

            // assert
            Assert.False(address.Equals(address2));
            Assert.False(address == address2);
        }

        [Fact]
        public void EqualValueObjectsHaveSameHashCode()
        {
            // arrange
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");

            // act

            // assert
            Assert.Equal(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesGiveDifferentHashCodes()
        {
            // arrange
            var address = new Address(null, "Austin", "TX");
            var address2 = new Address("TX", "Austin", null);

            // act

            // assert
            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void UnequalValueObjectsHaveDifferentHashCodes()
        {
            // arrange
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");
            
            // act

            // assert
            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }
        
        [Fact]
        public void TransposedValuesOfFieldNamesGivesDifferentHashCodes()
        {
            // arrange
            var address = new Address("_city", null, null);
            var address2 = new Address(null, "_address1", null);

            // act

            // assert
            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }
        
        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly()
        {
            // arrange
            var address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            var address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            // act

            // assert
            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void ReconstructedObjectIsStillEqual()
        {
            // arrange
            var dt = Timestamp.UtcNow();
            var expected = new TimestampObj{ Timestamp = dt, Prop1 = "foo"};
            var asString = expected.ToString();

            // act
            var reconstructed = TimestampObj.FromString(asString);

            // assert
            Assert.True(expected.Equals(reconstructed));
        }

        private class TimestampObj : ValueObject<TimestampObj>
        {
            public string Prop1 { get; set; }

            public DateTimeOffset Timestamp { get; set; }

            public override string ToString()
            {
                var coll = new ObjectPropertyCollection(this);
                return coll.ToString();
            }

            public static TimestampObj FromString(string input)
            {
                var coll = ObjectPropertyCollection.FromKvpString(input);
                return new TimestampObj
                {
                    Timestamp = DAS.Infrastructure.Timestamp.FromMilliseconds(coll[nameof(Timestamp)]),
                    Prop1 = coll[nameof(Prop1)]
                };
            }
        }

        private class Address : ValueObject<Address>
        {
            private string Address1 { get; set; }
            private string City { get; set; }
            private string State { get; set; }

            public Address(string address1, string city, string state)
            {
                Address1 = address1;
                City = city;
                State = state;
            }

            public override string ToString()
            {
                var coll = new ObjectPropertyCollection(this);
                return coll.ToString();
            }

            public static Address FromString(string kvpString)
            {
                var coll = ObjectPropertyCollection.FromKvpString(kvpString);
                return new Address(
                    coll[nameof(Address1)],
                    coll[nameof(City)],
                    coll[nameof(State)]);
            }
        }

        private class ExpandedAddress : Address
        {
            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                Address2 = address2;
            }

            private string Address2 { get; }
        }
    }
}
