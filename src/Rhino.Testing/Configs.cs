using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;

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

#pragma warning disable IDE0090 // Use 'new(...)'
            using FileStream fstream = new FileStream(settingsFile, FileMode.Open);
            using XmlReader reader = XmlReader.Create(fstream);
            return (T)serializer.Deserialize(reader);
#pragma warning restore IDE0090 // Use 'new(...)'
        }

        public static Configs Current { get; } = new Configs();

        [XmlElement("RhinoSystemDirectory")]
        public string RhinoSystemDir { get; set; } = string.Empty;

        [XmlElement]
        public bool LoadEto { get; set; } = false;

        [XmlElement]
        public bool LoadRDK { get; set; } = false;

        [XmlElement]
        public bool CreateRhinoDoc { get; set; } = true;

        [XmlElement]
        public bool CreateRhinoView { get; set; } = true;

        [XmlElement]
        public bool LoadLegacyIronPython { get; set; } = false;

        [XmlElement]
        public bool LoadGrasshopper { get; set; } = false;

        [XmlArray("LoadPlugins")]
        [XmlArrayItem("Plugin")]
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
        public List<PluginConfigs> LoadPlugins { get; set; } = new List<PluginConfigs>();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

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

    [Serializable]
    [XmlRoot("Plugin")]
    public sealed class PluginConfigs
    {
        [XmlAttribute]
        public string Location { get; set; } = string.Empty;
    }
}
