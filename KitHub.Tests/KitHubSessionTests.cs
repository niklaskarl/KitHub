using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KitHub.Tests
{
    [TestClass]
    [TestCategory("KitHubSessionTests")]
    public class KitHubSessionTests
    {
        protected KitHubSession Session { get; set; }

        [TestInitialize]
        public void ClassInitialize()
        {
            Session = new KitHubSession();
        }
    }

    [TestClass]
    public class KitHubSessionGetAuthenticatedUserAsyncTests : KitHubSessionTests
    {
        [TestMethod]
        [ExpectedException(typeof(KitHubRequestException))]
        public async Task ThrowsOnNotAuthenticated()
        {
            await Session.GetAuthenticatedUserAsync();
        }
    }

    [TestClass]
    public class KitHubSessionGetUserAsyncTests : KitHubSessionTests
    {
        [TestMethod]
        public async Task ReturnsCorrectUser()
        {
            string login = "octocat";
            string email = "octocat@github.com";

            User user = await Session.GetUserAsync(login);
            Assert.AreEqual(login, user.Login);
            Assert.AreEqual(email, user.Email);
        }
    }
}
