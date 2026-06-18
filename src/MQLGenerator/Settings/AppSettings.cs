using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MqlGenerator
{
    internal sealed class AppSettings
    {
        public string Application = "Framework";
        public string Version = "R2026x";
        public string Installer = "CUST";
        public string CreatedBy = "USER";
        public string SaveFolder = @"..\..\01.Attribute";

        public static string ConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MQLGenerator.ini");
        }

        public static AppSettings Load()
        {
            var settings = new AppSettings();
            string path = ConfigPath();
            if (!File.Exists(path))
            {
                return settings;
            }

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                int index = line.IndexOf('=');
                if (index < 0)
                {
                    continue;
                }

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1);
                if (key.Equals("Application", StringComparison.OrdinalIgnoreCase)) settings.Application = value;
                if (key.Equals("Version", StringComparison.OrdinalIgnoreCase)) settings.Version = value;
                if (key.Equals("Installer", StringComparison.OrdinalIgnoreCase)) settings.Installer = value;
                if (key.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)) settings.CreatedBy = value;
                if (key.Equals("SaveFolder", StringComparison.OrdinalIgnoreCase)) settings.SaveFolder = value;
            }

            return settings;
        }

        public void Save()
        {
            var lines = new List<string>();
            lines.Add("Application=" + Application);
            lines.Add("Version=" + Version);
            lines.Add("Installer=" + Installer);
            lines.Add("CreatedBy=" + CreatedBy);
            lines.Add("SaveFolder=" + SaveFolder);
            File.WriteAllLines(ConfigPath(), lines.ToArray(), Encoding.UTF8);
        }
    }
}
