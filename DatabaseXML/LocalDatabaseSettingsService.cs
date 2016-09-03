using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseXML
{
    public class LocalDatabaseSettingsService
    {
        LocalDatabaseSettingsService()
        {
            SettingsDocument = XDocument.Load(SettingsPath);
            IsInstalled = Convert.ToBoolean(SettingsDocument.Descendants("Installer").First().Element("Installed").Value);
        }

        private static readonly Lazy<LocalDatabaseSettingsService> Instance = new Lazy<LocalDatabaseSettingsService>(() => new LocalDatabaseSettingsService());

        public static LocalDatabaseSettingsService Manager { get { return Instance.Value; } }

        private  XDocument SettingsDocument { get; set; }

        private string SettingsPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory) + "App_Data/Settings.xml"; }
        }

        public void SetToInstalled()
        {
            IsInstalled = true;
            SettingsDocument.Descendants("Installer").First().Element("Installed").SetValue("True");
            SettingsDocument.Save(SettingsPath);
        }

        public bool IsInstalled { get; private set; }

        public string Version
        {
            get { return SettingsDocument.Root.Element("Version").Value; }
        }

        public string GetDataModelConnectionString()
        {
            var database = GetDatabaseSettings();

            return string.Format("metadata=res://*/Entities.DataModel.csdl|res://*/Entities.DataModel.ssdl|res://*/Entities.DataModel.msl;provider=System.Data.SqlClient;provider connection string='data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework'", database.Host, database.Name, database.Username, database.Password);
        }

        public string GetConnectionString()
        {
            var database = GetDatabaseSettings();

            return string.Format("Server={0};Database={1};User Id={2};Password={3};", database.Host, database.Name,
           database.Username, database.Password);
        }

        public void UpdateDatabase(string host, string name, string username, string password)
        {
            var database = SettingsDocument.Descendants("Database").First();

            database.Element("Host").SetValue(host);
            database.Element("Name").SetValue(name);
            database.Element("Username").SetValue(username);
            database.Element("Password").SetValue(password);

            SettingsDocument.Save(SettingsPath);
        }

        public DatabaseSettings GetDatabaseSettings()
        {
            var database = SettingsDocument.Descendants("Database").First();

            return new DatabaseSettings
            {
                Host = database.Element("Host").Value,
                Name = database.Element("Name").Value,
                Username = database.Element("Username").Value,
                Password = database.Element("Password").Value
            };
        }
    }
}
