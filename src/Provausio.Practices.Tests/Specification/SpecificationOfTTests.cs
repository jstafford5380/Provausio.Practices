﻿using System;
using System.Threading.Tasks;
using Provausio.Practices.Specification.Provausio.Practices.Patterns;
using Xunit;

namespace Provausio.Practices.Tests.Specification
{
    public class SpecificationOfTTests
    {
        [Fact]
        public void And_IsTrue()
        {
            // arrange
            var target = new Person { Age = 22, Name = "John", Gender = "M" };
            var isMale = new IsMale();
            var canDrink = new IsOldEnoughToDrink();

            // act
            var spec = isMale.And(canDrink);
            var isMaleOfAge = spec.IsSatisfiedBy(target);

            // assert
            Assert.True(isMaleOfAge);
        }

        [Fact]
        public void And_IsFalse()
        {
            // arrange
            var target = new Person { Age = 16, Name = "John", Gender = "M" };
            var isMale = new IsMale();
            var canDrink = new IsOldEnoughToDrink();

            // act
            var spec = isMale.And(canDrink);
            var isMaleOfAge = spec.IsSatisfiedBy(target);

            // assert
            Assert.False(isMaleOfAge);
        }

        [Fact]
        public void Or_IsTrue()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Jenn", Gender = "F", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var canDrink = new IsOldEnoughToDrink();

            // act
            var spec = canDrink.Or(isMale);
            var isMaleOrOldEnoughToDrink = spec.IsSatisfiedBy(target);

            // assert
            Assert.True(isMaleOrOldEnoughToDrink);
        }

        [Fact]
        public void Or_IsFalse()
        {
            // arrange
            var target = new Person { Age = 16, Name = "Jenn", Gender = "F", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var canDrink = new IsOldEnoughToDrink();

            // act
            var spec = canDrink.Or(isMale);
            var isMaleOrOldEnoughToDrink = spec.IsSatisfiedBy(target);

            // assert
            Assert.False(isMaleOrOldEnoughToDrink);
        }

        [Fact]
        public void OrNot_IsTrue()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Jenn", Gender = "F", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var isMember = new IsClubMember();

            // act
            var spec = isMember.OrNot(isMale);
            var isFemaleOrClubMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.True(isFemaleOrClubMember);
        }

        [Fact]
        public void OrNot_IsFalse()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Mike", Gender = "M" };
            var isMale = new IsMale();
            var isMember = new IsClubMember();

            // act
            var spec = isMember.OrNot(isMale);
            var isFemaleOrClubMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.False(isFemaleOrClubMember);
        }

        [Fact]
        public void Not_IsTrue()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Jenn", Gender = "F" };
            var isMale = new IsMale();
            var isClubMember = new IsClubMember();

            // act
            var spec = isClubMember.Not(isMale); // isMale is irrelevant
            var isFemaleOrClubMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.True(isFemaleOrClubMember);
        }

        [Fact]
        public void Not_IsFalse()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Mike", Gender = "M", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var isClubMember = new IsClubMember();

            // act
            var spec = isClubMember.Not(isMale); // isMale is irrelevant
            var isFemaleOrClubMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.False(isFemaleOrClubMember);
        }

        [Fact]
        public void AndNot_IsFalse()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Mike", Gender = "M", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var isClubMember = new IsClubMember();

            // act
            var spec = isClubMember.AndNot(isMale);
            var isFemaleMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.False(isFemaleMember);
        }

        [Fact]
        public void AndNot_IsTrue()
        {
            // arrange
            var target = new Person { Age = 21, Name = "Jenn", Gender = "F", MembershipNumber = "1234" };
            var isMale = new IsMale();
            var isClubMember = new IsClubMember();

            // act
            var spec = isClubMember.AndNot(isMale);
            var isFemaleMember = spec.IsSatisfiedBy(target);

            // assert
            Assert.True(isFemaleMember);
        }

        [Fact]
        public void EvalAndExecute_IsSatisfied_Runs()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person {Gender = "m"};

            // act
            isMale.EvalAndExecute(person, () => i++);

            // assert
            Assert.Equal(1, i);
        }

        [Fact]
        public void EvalAndExecute_Inverted_IsSatisfied_DoesntRun()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "m" };

            // act
            isMale.EvalAndExecute(person, () => i++, true);

            // assert
            Assert.Equal(0, i);
        }

        [Fact]
        public void EvalAndExecute_Inverted_IsNotSatisfied_Runs()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "f" };

            // act
            isMale.EvalAndExecute(person, () => i++, true);

            // assert
            Assert.Equal(1, i);
        }

        [Fact]
        public void EvalAndExecute_IsNotSatisfied_DoesntRun()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "f" };

            // act
            isMale.EvalAndExecute(person, () => i++);

            // assert
            Assert.Equal(0, i);
        }

        [Fact]
        public async Task EvalAndExecuteAsync_IsSatisfied_Runs()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "m" };

            // act
            await isMale.EvalAndExecuteAsync(person, () => Task.Run(async () =>
            {
                await Task.Delay(1000);
                i++;
            }));

            // assert
            Assert.Equal(1, i);
        }

        [Fact]
        public async Task EvalAndExecuteAsync_Inverted_IsSatisfied_DoesntRun()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "m" };

            // act
            await isMale.EvalAndExecuteAsync(person, () => Task.Run(async () =>
            {
                await Task.Delay(1000);
                i++;
            }), true);

            // assert
            Assert.Equal(0, i);
        }

        [Fact]
        public async Task EvalAndExecuteAsync_Inverted_IsNotSatisfied_Runs()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "f" };

            // act
            await isMale.EvalAndExecuteAsync(person, () => Task.Run(async () =>
            {
                await Task.Delay(1000);
                i++;
            }), true);

            // assert
            Assert.Equal(1, i);
        }

        [Fact]
        public async Task EvalAndExecuteAsync_IsNotSatisfied_DoesntRun()
        {
            // arrange
            var i = 0;
            var isMale = new IsMale();
            var person = new Person { Gender = "f" };

            // act
            await isMale.EvalAndExecuteAsync(person, () => Task.Run(async () =>
            {
                await Task.Delay(1000);
                i++;
            }));

            // assert
            Assert.Equal(0, i);
        }

        private class IsOldEnoughToDrink : Specification<Person>
        {
            public override bool IsSatisfiedBy(Person target)
            {
                return target.Age >= 21;
            }
        }

        private class IsMale : Specification<Person>
        {
            public override bool IsSatisfiedBy(Person target)
            {
                return target.Gender.Equals("m", StringComparison.OrdinalIgnoreCase);
            }
        }

        private class NameStartsWithJ : Specification<Person>
        {
            public override bool IsSatisfiedBy(Person target)
            {
                return target.Name.StartsWith("j", StringComparison.OrdinalIgnoreCase);
            }
        }

        private class IsClubMember : Specification<Person>
        {
            public override bool IsSatisfiedBy(Person target)
            {
                return !string.IsNullOrEmpty(target.MembershipNumber);
            }
        }

        private class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Gender { get; set; }

            public string MembershipNumber { get; set; }
        }
    }
}
