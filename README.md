# Rhino.Testing

NUnit dotnet unit testing for Rhino3D

## Setting Up Your Project

### Package References

Add these package references to your project (.csproj). These references ensure your tests are discoverable by the test runner:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
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

### Setup Fixture

Implement the `Rhino.Testing.Fixtures.RhinoSetupFixture` abstract class in your test library to setup and teardown your testing fixture:

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

### Loading GHAs for Testing

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

### Test Fixture

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

The `GHReport` data structure contains any component messages after definition is solved.

### Test Attribute

Alternatively you can use the RhinoTestAttribute. This attribute does not require you to inherit from the TestFixture class, or create a SetUpFixture. Another advantage is that by using it, you can use Rhino classes in TestDataSource.

Implement the `Rhino.Testing.Fixtures.RhinoTestFixture` attribute above each test class in your test library, add methods for each of your test and make sure to add the `[Test]` attribute to these methods:

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
