using NUnit.Framework;
using Rhino.Geometry;

namespace Example.Tests;

[TestFixture]
public class SimpleTests : Rhino.Testing.Fixtures.RhinoTestFixture
{

    [Test]
    public void BooleanOperation()
    {
        Box b1 = new Box(new BoundingBox(0, 0, 0, 100, 100, 100));
        Box b2 = new Box(new BoundingBox(50, 50, 50, 150, 150, 150));

        Brep brep1 = b1.ToBrep();
        Brep brep2 = b2.ToBrep();

        Brep[] breps = Brep.CreateBooleanDifference(brep1, brep2, 0.001);
        Assert.That(breps, Is.Not.Empty);
    }

}
