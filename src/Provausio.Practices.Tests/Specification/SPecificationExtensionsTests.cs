using System.Collections.Generic;
using System.Linq;
using Provausio.Practices.Specification;
using Provausio.Practices.Specification.Provausio.Practices.Patterns;
using Xunit;

namespace Provausio.Practices.Tests.Specification
{
    public class SpecificationExtensionsTests
    {
        [Fact]
        public void Where_Compound_ReturnsExpected()
        {
            // arrange
            var requiresEnhancedInsurance = new RequiresEnhancedInsuranceSpecification();
            var customers = new List<Customer>
            {
                new Customer("A", 17), 
                new Customer("B", 21), // expected
                new Customer("C", 26), 
                new Customer("D", 34)
            };

            // act
            var highRiskCustomers = customers.Where(requiresEnhancedInsurance).ToList();

            // assert
            Assert.Equal(1, highRiskCustomers.Count);
            Assert.Equal(1, highRiskCustomers.Count(c => c.Name == "B"));
        }

        private class IsOldEnoughToDrinkSpecification : Specification<Customer>
        {
            public override bool IsSatisfiedBy(Customer target)
            {
                return target.Age >= 21;
            }
        }

        private class IsRentalMinor : Specification<Customer>
        {
            public override bool IsSatisfiedBy(Customer target)
            {
                return target.Age < 25;
            }
        }

        private class CanRentACarSpecification : Specification<Customer>
        {
            public override bool IsSatisfiedBy(Customer target)
            {
                return target.Age >= 18;
            }
        }

        private class RequiresEnhancedInsuranceSpecification : Specification<Customer>
        {
            private readonly IsOldEnoughToDrinkSpecification _canDrink;
            private readonly IsRentalMinor _isRentalMinor;
            private readonly CanRentACarSpecification _canRentCar;

            public RequiresEnhancedInsuranceSpecification()
            {
                _canDrink = new IsOldEnoughToDrinkSpecification();
                _isRentalMinor = new IsRentalMinor();
                _canRentCar = new CanRentACarSpecification();
            }

            public override bool IsSatisfiedBy(Customer target)
            {
                var spec = _canRentCar.And(_canDrink).And(_isRentalMinor);
                return spec.IsSatisfiedBy(target);
            }
        }

        private class Customer
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public Customer(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }
    }
}
