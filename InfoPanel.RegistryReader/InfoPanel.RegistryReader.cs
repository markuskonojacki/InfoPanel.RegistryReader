using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using InfoPanel.Plugins;
using IniParser;
using IniParser.Model;
using Microsoft.Win32;

namespace InfoPanel.RegistryReader
{
    public class RegistryReaderPlugin : BasePlugin
    {
        public override string? ConfigFilePath => _configFilePath;

        // UI display elements for InfoPanel
        private readonly PluginText _regValueString01 = new("String01", "Registry string value 01", "-");
        private readonly PluginText _regValueString02 = new("String02", "Registry string value 02", "-");
        private readonly PluginText _regValueString03 = new("String03", "Registry string value 03", "-");
        private readonly PluginText _regValueString04 = new("String04", "Registry string value 04", "-");
        private readonly PluginText _regValueString05 = new("String05", "Registry string value 05", "-");

        private readonly PluginSensor _regValueDWord01 = new("Dword01", "Registry number value 01", 0);
        private readonly PluginSensor _regValueDWord02 = new("Dword02", "Registry number value 02", 0);
        private readonly PluginSensor _regValueDWord03 = new("Dword03", "Registry number value 03", 0);
        private readonly PluginSensor _regValueDWord04 = new("Dword04", "Registry number value 04", 0);
        private readonly PluginSensor _regValueDWord05 = new("Dword05", "Registry number value 05", 0);

        // Configurable settings
        private string? _configFilePath;
        private double _registryReaderRefreshTimer = 10;

        private string _regValueString01Path;
        private string _regValueString02Path;
        private string _regValueString03Path;
        private string _regValueString04Path;
        private string _regValueString05Path;

        private string _regValueDWord01Path;
        private string _regValueDWord02Path;
        private string _regValueDWord03Path;
        private string _regValueDWord04Path;
        private string _regValueDWord05Path;

        // Constants for timing and detection thresholds
        public override TimeSpan UpdateInterval => TimeSpan.FromSeconds(1);
        private DateTime _lastRegistryReaderCallTime = DateTime.MinValue;

        // Constructor: Initializes the plugin with metadata
        public RegistryReaderPlugin()
            : base("RegistryReader-plugin", "InfoPanel.RegistryReader", "Displays your average RegistryReader to a list of servers in ms")
        { }

        public override void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string basePath = assembly.ManifestModule.FullyQualifiedName;
            _configFilePath = $"{basePath}.ini";

            var parser = new FileIniDataParser();
            IniData config;
            if (!File.Exists(_configFilePath))
            {
                config = new IniData();

                config["RegistryReader Plugin"]["RefreshTimer"] = "10";

                config["RegistryReader Plugin"]["String01Path"] = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProductName";
                config["RegistryReader Plugin"]["String02Path"] = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\DisplayVersion";
                config["RegistryReader Plugin"]["String03Path"] = "";
                config["RegistryReader Plugin"]["String04Path"] = "";
                config["RegistryReader Plugin"]["String05Path"] = "";

                config["RegistryReader Plugin"]["DWord01Path"] = "";
                config["RegistryReader Plugin"]["DWord02Path"] = "";
                config["RegistryReader Plugin"]["DWord03Path"] = "";
                config["RegistryReader Plugin"]["DWord04Path"] = "";
                config["RegistryReader Plugin"]["DWord05Path"] = "";

                parser.WriteFile(_configFilePath, config);

                _regValueString01Path = config["RegistryReader Plugin"]["String01Path"];
                _regValueDWord01Path = config["RegistryReader Plugin"]["DWord01Path"];
            }
            else
            {
                try
                {
                    using (FileStream fileStream = new FileStream(_configFilePath!, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string fileContent = reader.ReadToEnd();
                        config = parser.Parser.Parse(fileContent);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("String01Path"))
                    {
                        _regValueString01Path = config["RegistryReader Plugin"]["String01Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["String01Path"] = "Computer\\HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProductName";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("String02Path"))
                    {
                        _regValueString02Path = config["RegistryReader Plugin"]["String02Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["String02Path"] = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\DisplayVersion";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("String03Path"))
                    {
                        _regValueString03Path = config["RegistryReader Plugin"]["String03Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["String03Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("String04Path"))
                    {
                        _regValueString04Path = config["RegistryReader Plugin"]["String04Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["String04Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("String05Path"))
                    {
                        _regValueString05Path = config["RegistryReader Plugin"]["String05Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["String05Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("DWord01Path"))
                    {
                        _regValueDWord01Path = config["RegistryReader Plugin"]["DWord01Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["DWord01Path"] = "Computer\\HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\InstallDate";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("DWord02Path"))
                    {
                        _regValueDWord02Path = config["RegistryReader Plugin"]["DWord02Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["DWord02Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("DWord03Path"))
                    {
                        _regValueDWord03Path = config["RegistryReader Plugin"]["DWord03Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["DWord03Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("DWord04Path"))
                    {
                        _regValueDWord04Path = config["RegistryReader Plugin"]["DWord04Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["DWord04Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    if (config["RegistryReader Plugin"].ContainsKey("DWord05Path"))
                    {
                        _regValueDWord05Path = config["RegistryReader Plugin"]["DWord05Path"];
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["DWord05Path"] = "";
                        parser.WriteFile(_configFilePath, config);
                    }

                    // parse refresh timer
                    if (config["RegistryReader Plugin"].ContainsKey("RefreshTimer") &&
                        double.TryParse(config["RegistryReader Plugin"]["RefreshTimer"], out double registryReaderRefreshTimer) &&
                        registryReaderRefreshTimer > 0)
                    {
                        _registryReaderRefreshTimer = registryReaderRefreshTimer;
                    }
                    else
                    {
                        config["RegistryReader Plugin"]["RefreshTimer"] = "10";
                        _registryReaderRefreshTimer = 10;
                        parser.WriteFile(_configFilePath, config);
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        // Loads UI containers as required by BasePlugin
        public override void Load(List<IPluginContainer> containers)
        {
            var container = new PluginContainer("RegistryReader");
            container.Entries.AddRange([_regValueString01, _regValueString02, _regValueString03, _regValueString04, _regValueString05,
                                        _regValueDWord01, _regValueDWord02, _regValueDWord03, _regValueDWord04, _regValueDWord05]);
            containers.Add(container);
        }

        // Cleans up resources when the plugin is closed
        public override void Close()
        { }

        // Synchronous update method required by BasePlugin
        public override void Update()
        {
            UpdateAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public override async Task UpdateAsync(CancellationToken cancellationToken)
        {

            var now = DateTime.UtcNow;
            var timeSinceLastCall = (now - _lastRegistryReaderCallTime).TotalSeconds;

            if (timeSinceLastCall > _registryReaderRefreshTimer)
            {
                _regValueString01.Value = await GetRegistryStringValueAsync(_regValueString01Path);
                _regValueString02.Value = await GetRegistryStringValueAsync(_regValueString02Path);
                _regValueString03.Value = await GetRegistryStringValueAsync(_regValueString03Path);
                _regValueString04.Value = await GetRegistryStringValueAsync(_regValueString04Path);
                _regValueString05.Value = await GetRegistryStringValueAsync(_regValueString05Path);

                _regValueDWord01.Value = await GetRegistryDWordValueAsync(_regValueDWord01Path);
                _regValueDWord02.Value = await GetRegistryDWordValueAsync(_regValueDWord02Path);
                _regValueDWord03.Value = await GetRegistryDWordValueAsync(_regValueDWord03Path);
                _regValueDWord04.Value = await GetRegistryDWordValueAsync(_regValueDWord04Path);
                _regValueDWord05.Value = await GetRegistryDWordValueAsync(_regValueDWord05Path);

                _lastRegistryReaderCallTime = now;
            }
        }

        // Retrieves a String value from the registry asynchronously
        public static async Task<string> GetRegistryStringValueAsync(string fullKey)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(fullKey))
                {
                    return "-";
                }

                var lastIndex = fullKey.LastIndexOf("\\");

                if (lastIndex == -1)
                {
                    return "Invalid key format";
                }

                var keyPath = fullKey.Substring(0, lastIndex);
                var valueName = fullKey.Substring(lastIndex + 1);

                // Retrieve the value from the registry
                string value = (string)Registry.GetValue(keyPath, valueName, "Value not found");

                return value is string stringValue ? stringValue : "Value not found";
            });
        }

        // Retrieves a DWORD value from the registry asynchronously
        public static async Task<float> GetRegistryDWordValueAsync(string fullKey)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(fullKey))
                {
                    return -1;
                }

                var lastIndex = fullKey.LastIndexOf("\\");

                if (lastIndex == -1)
                {
                    return -1;
                }

                var keyPath = fullKey.Substring(0, lastIndex);
                var valueName = fullKey.Substring(lastIndex + 1);

                // Retrieve the value from the registry
                var value = Registry.GetValue(keyPath, valueName, -1);

                if (value == null || value is not int)
                {
                    return -1; // Return -1 if the value is not found or not an integer
                }

                return Convert.ToSingle(value);
            });
        }

        // Logs errors and updates UI with error message
        private void HandleError(string errorMessage)
        { }
    }
}