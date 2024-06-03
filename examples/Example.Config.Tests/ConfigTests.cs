using System.Collections;
using NUnit.Framework;
using Rhino.Geometry;

namespace Example.Tests;

[TestFixture]
public class ConfigTests : Rhino.Testing.Fixtures.RhinoTestFixture
{

    [Test]
    [TestCaseSource(nameof(BoxSets))]
    public void IterativeIntersections(Box b1, Box b2)
    {
        Brep brep1 = b1.ToBrep();
        Brep brep2 = b2.ToBrep();

        var iterationCount = CustomTestConfigData.Current.MaxIterations;
        Assert.That(iterationCount, Is.GreaterThan(0));
        for (int it = 0; it < iterationCount; it++)
        {
            Brep[] breps = Brep.CreateBooleanDifference(brep1, brep2, CustomTestConfigData.Current.Tolerance);
            Assert.That(breps, Is.Not.Empty);
        }
    }

    public IEnumerable BoxSets()
    {
        yield return new TestCaseData(new Box(new BoundingBox(0, 0, 0, 100, 100, 100)), new Box(new BoundingBox(50, 50, 50, 150, 150, 150))).Returns(true);
        yield return new TestCaseData(new Box(new BoundingBox(0, 0, 0, 20, 100, 100)), new Box(new BoundingBox(50, 30, 50, 150, 150, 150))).Returns(true);
        yield return new TestCaseData(new Box(new BoundingBox(0, 0, 0, 1, 2, 3)), new Box(new BoundingBox(50, 30, 50, 150, 150, 150))).Returns(false);
    }

}
