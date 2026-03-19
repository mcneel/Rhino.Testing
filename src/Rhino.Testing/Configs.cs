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

        [XmlElement]
        public bool WindowedRhino { get; set; } = false;


        [XmlArray("LoadPlugins")]
        [XmlArrayItem("Plugin")]
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
        public List<PluginConfigs> LoadPlugins { get; set; } = new List<PluginConfigs>();
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists

        [XmlArray("PackageDirectories")]
        [XmlArrayItem("Directory")]
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
        public List<DirectoryConfigs> PackageDirectories { get; set; } = new List<DirectoryConfigs>();
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

        /// <summary>
        /// Support for using environment variables in the rhino system directory, package directories, and plugin paths, in the form of ${env:VAR_NAME}
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplaceEnvVars(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            int startIndex = 0;
            while (true)
            {
                int envStart = path.IndexOf("${env:", startIndex, StringComparison.OrdinalIgnoreCase);
                if (envStart == -1)
                    break;
                int envEnd = path.IndexOf('}', envStart);
                if (envEnd == -1)
                    break;
                string envVar = path.Substring(envStart + 6, envEnd - envStart - 6);
                string envVal = Environment.GetEnvironmentVariable(envVar) ?? string.Empty;
#if NET8_0_OR_GREATER
                path = path[..envStart] + envVal + path[(envEnd + 1)..];
#else
                path = path.Substring(0, envStart) + envVal + path.Substring(envEnd + 1);
#endif
                startIndex = envStart + envVal.Length;
            }
            return path;
        }

        static Configs()
        {
            string cfgFile = GetConfigsFile();

            if (File.Exists(cfgFile))
            {
                Current = Deserialize<Configs>(new XmlSerializer(typeof(Configs)), cfgFile);
                Current.RhinoSystemDir = ReplaceEnvVars(Current.RhinoSystemDir);

                // separate our the Package Directories by the path separator and trim whitespace
                List<DirectoryConfigs> dirs = new List<DirectoryConfigs>();
                foreach (var dir in Current.PackageDirectories)
                {
                    var path = ReplaceEnvVars(dir.Path.Trim());
                    string[] splitDirs = path.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var splitDir in splitDirs)
                    {
                        dirs.Add(new DirectoryConfigs() { Path = splitDir.Trim() });
                    }
                }
                Current.PackageDirectories = dirs;

                foreach (var plugin in Current.LoadPlugins)
                {
                    plugin.Location = ReplaceEnvVars(plugin.Location);
                }

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

    [Serializable]
    [XmlRoot("Directory")]
    public sealed class DirectoryConfigs
    {
        [XmlAttribute]
        public string Path { get; set; } = string.Empty;
    }
}
