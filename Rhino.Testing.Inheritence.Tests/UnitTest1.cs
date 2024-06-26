using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Inheritence.Tests
{

    [TestFixture]
    public class Tests : RhinoTestFixture
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
