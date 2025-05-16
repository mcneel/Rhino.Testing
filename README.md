# Rhino.Testing

NUnit dotnet unit testing for Rhino3D

## Setting Up Your Project

### Package References

Add these package references to your project (.csproj). These references ensure your tests are discoverable by the test runner:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.6.0" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="Rhino.Testing" Version="8.0.*-beta" />
  </ItemGroup>
```

### Rhino.Testing Configuration

Rhino.Testing will use `Rhino.Testing.Configs.xml` file to read `RhinoSystemDirectory` and setup necessary assembly resolvers for the target Rhino.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <RhinoSystemDirectory>C:\Program Files\Rhino 8\System</RhinoSystemDirectory>
</Settings>
```

When initializing Rhino, an empty headless document is created. You can disable this behaviour:

```xml
<CreateRhinoDoc>false</CreateRhinoDoc>
```

The default document will be assigned an active view as well. You can disable this behaviour:

```xml
<CreateRhinoView>false</CreateRhinoView>
```

Specify Eto or RDK to be loaded (if `LoadGrasshopper` is specified, Eto and RDK will be automatically loaded):

```xml
<LoadEto>true</LoadEto>

<LoadRDK>true</LoadRDK>
```

If you want Rhino.Testing to load Eto you must not include Eto in your build directory or both assemblies will try to load, and it will fail.
Below the Eto.Forms nuget is set to not copy to the build direcotry
``` xml
<PackageReference Include="Eto" Version="2.8.4" Private="False" PrivateAssets="all" />
```

Specify Legacy IronPython to be loaded:

```xml
<LoadLegacyIronPython>true</LoadLegacyIronPython>
```

Specify list of plugins to be loaded (These plugins are always loaded before Grasshopper)

```xml
  <LoadPlugins>
    <!-- Legacy IronPython -->
    <Plugin Location="Plug-ins\IronPython\RhinoDLR_Python.rhp" />

    <Plugin Location="MyPlugins\MyRhinoPlugin.rhp" />
  </LoadPlugins>
```

Specify Grasshopper to be loaded:

```xml
<LoadGrasshopper>true</LoadGrasshopper>
```

Specify Grasshopper 2 to be loaded:

```xml
<LoadGrasshopper2>true</LoadGrasshopper2>
```

Make sure this file is copied onto the build folder (where `Rhino.Testing.dll` exists):

```xml
  <ItemGroup>
    <None Update="Rhino.Testing.Configs.xml" CopyToOutputDirectory="always" />
  </ItemGroup>
```

`Rhino.Testing.Configs.xml` can also contains any other configuration you want for your project. You can deserialize the xml file into your own settings:

```csharp
[Serializable]
[XmlRoot("Settings")]
public sealed class MyTestSettings
{
    [XmlElement]
    public string MySetting { get; set; }
}

// use the default settings file (or your own xml file)
string settingsFile = Rhino.Testing.Configs.SettingsFile;

// create an xml serializer for your settings type
XmlSerializer serializer = new XmlSerializer(typeof(MyTestSettings));

// deserialize your settings
var settings = Rhino.Testing.Configs.Deserialize<MyTestSettings>(serializer, settingsFile);

```

### Test Setup Fixture

There are two ways to ensure Rhino is loaded and ready before your tests are executed:

- Applying `Rhino.Testing.Fixtures.RhinoTestFixtureAttribute` to your test fixtures
- Deriving your test fixtures from `Rhino.Testing.Fixtures.RhinoSetupFixture`

Using `RhinoTestFixtureAttribute` is usually better since it allows using RhinoCommon data types during test discovery. For example, without using the attribute, the test below fails during discovery since test-host does not have access to RhinoCommon. Using the `RhinoTestFixtureAttribute` however makes sure that Rhino is loaded during test discovery in the host:

```csharp
    [RhinoTestFixture]      // remove this attribute and test fails to load
    public class Point3dTest
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
```

Is also means you do not have to add an implementation of `RhinoSetupFixture` in your tests to initialize Rhino.

#### Using Setup Attribute

Add `RhinoTestFixtureAttribute` to any class to ensure Rhino is initialized for your test:

```csharp
    [RhinoTestFixture]
    public sealed class PrimitivesFixture
    {
        [Test]
        public void YourRhinoTest()
        {
            // you rhino test
        }
    }
```

Note that your test case does not need to be derived from any specific base class. However deriving from `Rhino.Testing.Fixtures.RhinoTestFixture` gives you access to extra functionality discussed below.

#### Using Setup Fixture

Implement the `Rhino.Testing.Fixtures.RhinoSetupFixture` abstract class in your test library to setup and teardown your testing fixture. This ensures Rhino is setup before testing starts and torn down after tests are done:

```csharp
    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            // your custom setup
        }

        public override void OneTimeTearDown()
        {
            base.OneTimeTearDown();

            // you custom teardown
        }
    }
```

#### Mixing Setup Fixture and Attribute

It is okay to mix using `RhinoSetupFixture` and `RhinoTestFixtureAttribute` in your test projects:

- Ensure `RhinoTestFixtureAttribute` is added to all test fixtures that use RhinoCommon data types to ensure they are correctly processed during test discovery
- Derive from `RhinoSetupFixture` to add custom initialization logic that is necessary before tests are actually executed (this is after the test discovery step). For example you can make sure a few needed Rhino or Grasshopper plugins are loaded before testing starts. Loading of these plugins is most probably not necessary during test discovery.

#### Loading GHAs for Testing

If your tests required a Grasshopper definition to be loaded, you can use `RhinoSetupFixture.LoadGHA(IEnumerable<string> ghaPaths)` method:

```csharp
    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            LoadGHA(new string[] {
              @"/path/to/gh1plugins/FirstTestPlugin.gha",
              @"/path/to/gh1plugins/SecondTestPlugin.gha",
            });
        }
    }
```

These GH plugins will be loaded during startup.

### Test Fixtures

Implement the `Rhino.Testing.Fixtures.RhinoTestFixture` abstract class in your test library, add methods for each of your test and make sure to add the `[Test]` attribute to these methods:

```csharp
    [TestFixture]
    public sealed class PrimitivesFixture : Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void YourRhinoTest()
        {
            // you rhino test
        }
    }
```

### Testing Grasshopper Files

You can run Grasshopper definitions as tests. If definition contains a *Context Bake* component that is named `result`, the data structure in that definition is checked for *GH_Boolean* values and the result is returned.

Notice the *Context Bake* component at the right side of this definition:

![](docs/test_grasshopper_circle.png)

This is how the fixture would run the definition:

```csharp
    [TestFixture]
    public sealed class PrimitivesFixture : Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestCircle()
        {
            const string ghFilePath = @"Files\circle.gh";

            RhinoTestFixture.TestGrasshopper(ghFilePath, out bool result, out GHReport report);
            
            // check if result is true
            Assert.IsTrue(result);

            // and report has no errors
            Assert.IsFalse(report.HasErrors);
        }
    }
```

The `GHReport` data structure contains any component messages after definition is solved. You can filter the level of messages by passing `GHMessageLevel reportLevel` argument:

```csharp
    [TestFixture]
    public sealed class PrimitivesFixture : Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestCircle()
        {
            const string ghFilePath = @"Files\circle.gh";

            RhinoTestFixture.TestGrasshopper(ghFilePath,
                                             out bool result,
                                             out GHReport report,
                                             GHMessageLevel.Warning);
        }
    }
```


You can also use the simpler `RhinoTestFixture.RunGrasshopper` method that does not expect a *Context Bake* component named `result`. This way you can implement your own testing logic. The function still returns a `GHReport report`.

```csharp
    [TestFixture]
    public sealed class PrimitivesFixture : Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestCircle()
        {
            const string ghFilePath = @"Files\circle.gh";

            RhinoTestFixture.RunGrasshopper(ghFilePath,
                                            out GHReport report,
                                            GHMessageLevel.Warning);
        }
    }
```
