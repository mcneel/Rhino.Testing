using System.Collections;
using Rhino.Geometry;
using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Attribute.Tests
{

    [RhinoTestFixture]
    public class DataSourceTest
    {

        [Test]
        [TestCaseSource(nameof(PointList))]
        public void Test(Point3d point)
        {
            Assert.That(point.X, Is.GreaterThan(0));
            Assert.That(point.Y, Is.GreaterThan(0));
            Assert.That(point.Z, Is.GreaterThan(0));
        }

        public static IEnumerable PointList()
        {
            yield return new TestCaseData(new Point3d(1, 2, 3));
            yield return new TestCaseData(new Point3d(4, 5, 6));
            yield return new TestCaseData(new Point3d(7, 8, 9));
        }
    }
}
