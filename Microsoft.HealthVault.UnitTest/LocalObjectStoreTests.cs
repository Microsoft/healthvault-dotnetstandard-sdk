using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.UnitTest.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest
{
    [TestClass]
    public class LocalObjectStoreTests
    {
        private const string ServiceInstanceKey = "Test";

        private const string ServiceInstanceSampleFile = "StoredServiceInstance.json";

        private ISecretStore _subSecretStore;

        [TestInitialize]
        public void TestInitialize()
        {
            _subSecretStore = Substitute.For<ISecretStore>();
        }

        [TestMethod]
        public async Task NormalWrite()
        {
            var serviceInstance = new HealthServiceInstance
            {
                Id = "test",
                Name = "Test",
                Description = "description",
                HealthServiceUrl = new Uri("http://contoso.com"),
                ShellUrl = new Uri("http://contoso.com/shell"),
            };

            LocalObjectStore localObjectStore = CreateLocalObjectStore();
            await localObjectStore.WriteAsync(ServiceInstanceKey, serviceInstance);

            await _subSecretStore.Received().WriteAsync(ServiceInstanceKey, SampleUtils.GetSampleContent(ServiceInstanceSampleFile));
        }

        [TestMethod]
        public async Task NormalRead()
        {
            _subSecretStore.ReadAsync(ServiceInstanceKey).Returns(SampleUtils.GetSampleContent(ServiceInstanceSampleFile));

            LocalObjectStore localObjectStore = CreateLocalObjectStore();
            HealthServiceInstance serviceInstance = await localObjectStore.ReadAsync<HealthServiceInstance>(ServiceInstanceKey);

            Assert.AreEqual("test", serviceInstance.Id);
            Assert.AreEqual("Test", serviceInstance.Name);
            Assert.AreEqual("description", serviceInstance.Description);
            Assert.AreEqual(new Uri("http://contoso.com"), serviceInstance.HealthServiceUrl);
            Assert.AreEqual(new Uri("http://contoso.com/shell"), serviceInstance.ShellUrl);
        }

        private LocalObjectStore CreateLocalObjectStore()
        {
            return new LocalObjectStore(
                _subSecretStore);
        }
    }
}
