using System.Collections;
using Rhino.Testing.Fixtures;
using Rhino.Testing.Tests.Utils;

namespace Rhino.Testing.Tests
{
    public class DataSourceTest : RhinoTestFixture
    {
        [Test]
        [TestCaseSource(nameof(PointList))]
        public void Test(double x, double y, double z)
        {
            var p = RhinoCommonAccess.CreatePoint(x, y, z);
            Assert.That(p.X, Is.GreaterThan(0));
            Assert.That(p.Y, Is.GreaterThan(0));
            Assert.That(p.Z, Is.GreaterThan(0));
        }

        public static IEnumerable PointList()
        {
            yield return new TestCaseData(1, 2, 3);
            yield return new TestCaseData(4, 5, 6);
            yield return new TestCaseData(7, 8, 9);
        }
    }
}
