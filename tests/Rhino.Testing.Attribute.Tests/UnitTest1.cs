using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Attribute.Tests
{

    [RhinoTestFixture]
    public class Tests
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
