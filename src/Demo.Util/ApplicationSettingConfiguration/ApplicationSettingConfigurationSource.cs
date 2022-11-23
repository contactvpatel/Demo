﻿#nullable enable
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.FileProviders;

 namespace Demo.Util.ApplicationSettingConfiguration
{
    /// <summary>
    /// An <see cref="T:Microsoft.Extensions.Configuration.IConfigurationSource" /> used to configure <see cref="T:ASM.Api.Extensions.KeyValueFileConfiguration.KeyValueFileConfigurationProvider" />.
    /// </summary>
    public class ApplicationSettingConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Constructor;
        /// </summary>
        public ApplicationSettingConfigurationSource()
            => IgnoreCondition = s => s.StartsWith(IgnorePrefix, StringComparison.Ordinal);

        /// <summary>
        /// The FileProvider whos root "/" directory files will be used as configuration data.
        /// </summary>
        public IFileProvider? FileProvider { get; set; }

        /// <summary>
        /// Files that start with this prefix will be excluded.
        /// Defaults to "ignore.".
        /// </summary>
        public string IgnorePrefix { get; set; } = "ignore.";

        /// <summary>
        /// Used to determine if a file should be ignored using its name.
        /// Defaults to using the IgnorePrefix.
        /// </summary>
        public Func<string, bool> IgnoreCondition { get; set; }

        /// <summary>
        /// If false, will throw if the directory doesn't exist.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Determines whether the source will be loaded if the underlying file changes.
        /// </summary>
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// Number of milliseconds that reload will wait before calling Load.  This helps
        /// avoid triggering reload before a file is completely written. Default is 250.
        /// </summary>
        public int ReloadDelay { get; set; } = 250;

        /// <summary>
        /// The delimiter used to separate individual keys in a path.
        /// </summary>
        /// <value>Default is <c>__</c>.</value>
        public string SectionDelimiter { get; set; } = "__";

        /// <summary>
        /// Builds the <see cref="ApplicationSettingConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ApplicationSettingConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new ApplicationSettingConfigurationProvider(this);
    }
}