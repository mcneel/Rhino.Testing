using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Rhino.Testing
{
    public sealed class Configs
    {
#if DEBUG
        const string CONFIGURATION = "Debug";
#else
        const string CONFIGURATION = "Release";
#endif
        static readonly Assembly s_assembly = typeof(Configs).Assembly;
        static readonly string s_settingsFileName = $"{s_assembly.GetName().Name}.Configs.xml";
        readonly XDocument _xml;

        public static Configs Current { get; } = new Configs();

        public string RhinoSystemDir { get; private set; } = string.Empty;
        public string SettingsDir { get; private set; } = string.Empty;
        public string SettingsFile { get; private set; } = string.Empty;

        public bool TryGetConfig<T>(string name, out T value)
        {
            value = default;

            if (_xml is null)
            {
                return false;
            }

            object v = _xml.Descendants(name).FirstOrDefault()?.Value;

            if (v is not null
                    && typeof(T).IsAssignableFrom(v.GetType()))
            {
                value = (T)v;
                return true;
            }

            return false;
        }

        public Configs()
        {
            RhinoSystemDir = string.Empty;
            SettingsDir = Path.GetDirectoryName(s_assembly.Location);
            SettingsFile = Path.Combine(SettingsDir, s_settingsFileName);

            if (File.Exists(SettingsFile))
            {
                _xml = XDocument.Load(SettingsFile);
                RhinoSystemDir = _xml.Descendants("RhinoSystemDirectory").FirstOrDefault()?.Value ?? null;

                RhinoSystemDir = RhinoSystemDir.Replace("$(Configuration)", CONFIGURATION
#if NET7_0_OR_GREATER
                , StringComparison.OrdinalIgnoreCase);
#else
                );
#endif
                if (!Path.IsPathRooted(RhinoSystemDir))
                {
                    RhinoSystemDir = Path.GetFullPath(Path.Combine(SettingsDir, RhinoSystemDir));
                }
            }
            else
            {
                throw new FileLoadException($"Can not find {typeof(Configs).Assembly.GetName().Name}.Configs.xml configuration file");
            }
        }
    }
}
