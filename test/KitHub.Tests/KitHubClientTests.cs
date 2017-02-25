using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KitHub.Tests
{
    [TestClass]
    [TestCategory("KitHubClientTests")]
    public class KitHubClientTests
    {
        internal KitHubClient Client { get; set; }

        [TestInitialize]
        public void ClassInitialize()
        {
            Client = new KitHubClient(null);
        }
    }

    [TestClass]
    public class KitHubClientGetAsyncTests : KitHubClientTests
    {
        [TestMethod]
        public async Task LinkHeaderCorrect()
        {
            KitHubRequest request = new KitHubRequest()
            {
                Uri = new Uri("/events", UriKind.Relative)
            };

            KitHubResponse response = await Client.GetAsync(request, default(CancellationToken));
            Assert.IsNotNull(response.Links);
            Assert.IsTrue(response.Links.ContainsKey("last"));
        }
    }
}
