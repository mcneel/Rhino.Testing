using System.Collections;
using NUnit.Framework;
using Rhino.Geometry;

namespace Example.Tests;

[TestFixture]
public class SimpleTests : Rhino.Testing.Fixtures.RhinoTestFixture
{

    public string CircleAreaScript
    {
        get
        {
            var assemLocation = typeof(SetupFixture).Assembly.Location;
            var assemDir = System.IO.Path.GetDirectoryName(assemLocation);
            return Path.Combine(assemDir, "Scripts", "CircleArea.gh");
        }
    }

    [Test]
    [TestCaseSource(nameof(AreaPairs))]
    public bool CircleAreas(decimal radius, decimal area)
    {
        var doc = ScriptExtensions.LoadDocument(CircleAreaScript);
        doc.ExpireSolution();

        ScriptExtensions.SetValueOfSlider(doc, "RADIUS", radius);
        ScriptExtensions.SetValueOfSlider(doc, "AREA", area);

        ScriptExtensions.Solve(doc);

        return ScriptExtensions.GetAssertion(doc);
    }

    public static IEnumerable AreaPairs()
    {
        // Passes
        yield return new TestCaseData(100m, 20_000m).Returns(true);
        yield return new TestCaseData(100m, 10_000m).Returns(true);

        // Fails
        yield return new TestCaseData(50m, 10_000m).Returns(false);
        yield return new TestCaseData(100m, 50_000m).Returns(false);
    }

}
