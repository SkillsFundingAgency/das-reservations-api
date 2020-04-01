using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Account.Queries.GetAccount;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Queries.GetAccount
{
    public class WhenValidatingTheQuery
    {
        private GetAccountQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountQueryValidator();
        }

        [Test]
        public async Task Then_The_Query_Is_Invalid_If_The_Required_Fields_Are_Not_Passed_And_Validation_Errors_Returned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("Id has not been supplied"));
        }

        [Test]
        public async Task Then_The_Query_Is_Valid_If_The_Values_Are_Valid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountQuery { Id = 99432 });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}