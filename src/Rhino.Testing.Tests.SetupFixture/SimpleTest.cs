using Rhino.Geometry;
using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Tests
{
    public class SimpleTest : RhinoTestFixture
    {
        [Test]
        public void Test()
        {
            Point3d point = new(1, 2, 3);
            Assert.That(point.X, Is.GreaterThan(0));
            Assert.That(point.Y, Is.GreaterThan(0));
            Assert.That(point.Z, Is.GreaterThan(0));
        }
    }
}
