using System.Xml.Serialization;

namespace Example.Tests
{
    [Serializable]
    [XmlRoot("Settings")]
    public sealed class CustomTestConfigData
    {
        [XmlElement]
        public string MySettings { get; set; }

        [XmlElement]
        public double Tolerance { get; set; }

        [XmlElement]
        public int MaxIterations { get; set; }


        public static CustomTestConfigData Current { get; }

        static CustomTestConfigData()
        {
            string settingsFile = Rhino.Testing.Configs.Current.SettingsFile;

            // create an xml serializer for your settings type
            XmlSerializer serializer = new XmlSerializer(typeof(CustomTestConfigData));

            // deserialize your settings
            Current = Rhino.Testing.Configs.Deserialize<CustomTestConfigData>(serializer, settingsFile);
        }
    }

}
