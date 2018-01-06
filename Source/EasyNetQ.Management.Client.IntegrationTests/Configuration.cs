﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EasyNetQ.Management.Client.Model;
using Microsoft.Extensions.Configuration;

namespace EasyNetQ.Management.Client.IntegrationTests
{
    internal sealed class Configuration
    {
        private static readonly IList<OSPlatform> OperatingSystems = new List<OSPlatform>
        {
            OSPlatform.Windows,
            OSPlatform.Linux,
            OSPlatform.OSX
        };
        private static readonly Lazy<Configuration> LazyInstance = new Lazy<Configuration>(() => new Configuration());

        private readonly string dockerHttpApiUri;
        private readonly int rabbitMqClientPort;
        private readonly int rabbitMqManagementPort;
        private readonly string rabbitMqVirtualHostName;
        private readonly Vhost rabbitMqVirtualHost;
        private readonly string rabbitMqUser;
        private readonly string rabbitMqPassword;
        private readonly Dictionary<OSPlatform, IConfigurationSection> osSpecificSettings;

        private static Configuration Instance => LazyInstance.Value;

        public static string DockerHttpApiUri => Instance.dockerHttpApiUri;

        public static int RabbitMqClientPort => Instance.rabbitMqClientPort;

        public static int RabbitMqManagementPort => Instance.rabbitMqManagementPort;

        public static string RabbitMqVirtualHostName => Instance.rabbitMqVirtualHostName;

        public static Vhost RabbitMqVirtualHost => Instance.rabbitMqVirtualHost;

        public static string RabbitMqUser => Instance.rabbitMqUser;

        public static string RabbitMqPassword => Instance.rabbitMqPassword;

        public static string RabbitMQDockerImage(OSPlatform dockerEngineOsPlatform) =>
            Instance.osSpecificSettings[dockerEngineOsPlatform]["rabbitMQDockerImage"];

        private Configuration()
        {
            var settings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();
            dockerHttpApiUri = settings["dockerHttpApiUri"];
            rabbitMqClientPort = int.Parse(settings["rabbitMQClientPort"]);
            rabbitMqManagementPort = int.Parse(settings["rabbitMQManagementPort"]);
            rabbitMqVirtualHostName = settings["rabbitMQVirtualHost"];
            rabbitMqVirtualHost = new Vhost { Name = rabbitMqVirtualHostName, Tracing = false };
            rabbitMqUser = settings["rabbitMQUser"];
            rabbitMqPassword = settings["rabbitMQPassword"];
            osSpecificSettings = OperatingSystems
                .Select(x => new { Os = x, ConfigSection = settings.GetSection(x.ToString().ToLowerInvariant()) })
                .ToDictionary(x => x.Os, x => x.ConfigSection);
        }
    }
}