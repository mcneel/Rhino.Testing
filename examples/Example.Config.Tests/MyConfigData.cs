using System.Xml.Serialization;

namespace Example.Tests
{
    [Serializable]
    [XmlRoot("Settings")]
    public sealed class CustomTestConfigData
    {

        [XmlElement]
        public double Tolerance { get; set; }

        [XmlElement]
        public int MaxIterations { get; set; }


        public static CustomTestConfigData Current { get; }

        static CustomTestConfigData()
        {
            string settingsFile = Rhino.Testing.Configs.Current.SettingsFile;

            // deserialize your settings
            Current = Rhino.Testing.Configs.Deserialize<CustomTestConfigData>(settingsFile);
        }
    }

}
