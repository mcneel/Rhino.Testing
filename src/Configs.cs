using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Rhino.Testing
{
    [Serializable]
    [XmlRoot("Settings")]
    public sealed class Configs
    {
        public static T Deserialize<T>(string settingsFile) => Deserialize<T>(new XmlSerializer(typeof(T)), settingsFile);

        public static T Deserialize<T>(XmlSerializer serializer, string settingsFile)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            using FileStream fstream = new FileStream(settingsFile, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fstream);
            return (T)serializer.Deserialize(reader);
        }

        public static Configs Current { get; } = new Configs();

        [XmlElement("RhinoSystemDirectory")]
        public string RhinoSystemDir { get; set; } = string.Empty;

        [XmlElement]
        public bool LoadEto { get; set; } = false;

        [XmlElement]
        public bool LoadRDK { get; set; } = false;

        [XmlElement]
        public bool LoadGrasshopper { get; set; } = false;

        [XmlIgnore]
        public string SettingsDir { get; } = string.Empty;

        [XmlIgnore]
        public string SettingsFile { get; } = string.Empty;

        public Configs()
        {
            SettingsFile = GetConfigsFile();
            SettingsDir = Path.GetDirectoryName(SettingsFile);
        }

        static Configs()
        {
            string cfgFile = GetConfigsFile();

            if (File.Exists(cfgFile))
            {
                Current = Deserialize<Configs>(new XmlSerializer(typeof(Configs)), cfgFile);

                if (Path.IsPathRooted(Current.RhinoSystemDir))
                {
                    return;
                }

                Current.RhinoSystemDir = Path.GetFullPath(
                        Path.Combine(Path.GetDirectoryName(cfgFile), Current.RhinoSystemDir)
                    );
            }
        }

        static string GetConfigsFile()
        {
            Assembly s_assembly = typeof(Configs).Assembly;
            string s_settingsFileName = $"{s_assembly.GetName().Name}.Configs.xml";
            string settingsDir = Path.GetDirectoryName(s_assembly.Location);
            return Path.Combine(settingsDir, s_settingsFileName);
        }
    }
}
