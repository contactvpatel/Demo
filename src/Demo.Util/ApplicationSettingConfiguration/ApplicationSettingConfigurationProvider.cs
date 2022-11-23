﻿#nullable enable
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.Primitives;

 namespace Demo.Util.ApplicationSettingConfiguration
{
    /// <summary>
    /// A <see cref="T:Microsoft.Extensions.Configuration.ConfigurationProvider" /> that uses a directory's files as configuration key/values.
    /// </summary>
    public class ApplicationSettingConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IDisposable? _changeTokenRegistration;

        ApplicationSettingConfigurationSource Source { get; set; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="source">The settings.</param>
        public ApplicationSettingConfigurationProvider(ApplicationSettingConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));

            if (Source.ReloadOnChange && Source.FileProvider != null)
            {
                _changeTokenRegistration = ChangeToken.OnChange(
                    () => Source.FileProvider.Watch("*"),
                    () =>
                    {
                        Thread.Sleep(Source.ReloadDelay);
                        Load(reload: true);
                    });
            }

        }

        private string NormalizeKey(string key)
            => key.Replace(Source.SectionDelimiter, ConfigurationPath.KeyDelimiter);

        private static string TrimNewLine(string value)
        {
            return value.Substring(1, value.Length - 2);
        }

        /// <summary>
        /// Loads the configuration values.
        /// </summary>
        public override void Load()
        {
            Load(reload: false);
        }

        private void Load(bool reload)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (Source.FileProvider == null)
            {
                if (Source.Optional || reload) // Always optional on reload
                {
                    Data = data;
                    OnReload();
                    return;
                }

                throw new DirectoryNotFoundException(
                    "A Secret Key Value File directory is required when this source is not optional.");
            }

            var directory = Source.FileProvider.GetDirectoryContents("/");
            if (!directory.Exists)
            {
                if (Source.Optional || reload) // Always optional on reload
                {
                    Data = data;
                    OnReload();
                    return;
                }

                throw new DirectoryNotFoundException(
                    "The root directory for the FileProvider doesn't exist and is not optional.");
            }

            foreach (var file in directory)
            {
                if (file.IsDirectory)
                {
                    continue;
                }

                if (Source.IgnoreCondition == null || !Source.IgnoreCondition(file.Name))
                {
                    var lines = File.ReadAllLines(file.PhysicalPath);

                    // Get the position of the = sign within each line
                    var pairs = lines.Where(x => !string.IsNullOrEmpty(x)).Select(l => new { Line = l, Pos = l.IndexOf("=") });

                    // Build a dictionary of key/value pairs by splitting the string at the = sign
                    var dictionary = pairs.ToDictionary(p => NormalizeKey(p.Line.Substring(0, p.Pos)), p => p.Line.Substring(p.Pos + 1));

                    foreach (var current in dictionary)
                    {
                        data.Add(current.Key, TrimNewLine(current.Value));
                    }
                }
            }

            Data = data;
            OnReload();
        }

        private string GetDirectoryName()
            => Source.FileProvider?.GetFileInfo("/")?.PhysicalPath ?? "<Unknown>";

        /// <summary>
        /// Generates a string representing this provider name and relevant details.
        /// </summary>
        /// <returns>The configuration name.</returns>
        public override string ToString()
            => $"{GetType().Name} for files in '{GetDirectoryName()}' ({(Source.Optional ? "Optional" : "Required")})";

        /// <inheritdoc />
        public void Dispose()
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}