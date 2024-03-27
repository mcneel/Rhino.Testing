# Rhino.Testing

NUnit dotnet unit testing for Rhino3D

## Settin Up Your Project

### Package References

Add these package references to your project (.csproj). These references ensure your tests are discoverable by the test runner:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="Rhino.Testing" Version="8.0.10-beta" />
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

Specify Eto or RDK to be loaded (if `LoadGrasshopper` is specified, Eto and RDK will be automatically loaded):

```xml
<LoadEto>true</LoadEto>

<LoadRDK>true</LoadRDK>
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
