using FluentAssertions;
using NUnit.Framework;
using Packem.Data.Interfaces;
using Packem.Domain.Models;
using System.Threading.Tasks;

namespace Packem.IntegrationTests
{
    using static Testing;

    public class Tests : BaseTestFixture
    {
        [Test]
        public async Task Test1()
        {
            var service = GetService<IAuthService>();
            var result = await service.AuthenticateUserAsync(new UserLoginModel
            {
                Username = "adminUsername",
                Password = "strongPassword!123@Az"
            });

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }
    }
}