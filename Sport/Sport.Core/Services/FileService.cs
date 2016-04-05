using System;
using System.Collections.Generic;
using MvvmCross.Plugins.File;
using Newtonsoft.Json;
using Sport.Core.Models;
using Sport.Core.Models.EndPoints;
using Sport.Core.Services.Interfaces;

namespace Sport.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly string AppName = "Banaca";

        public FileService(IMvxFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public Dictionary<string, object> LoadLocal()
        {
            var r = Load<Dictionary<string, object>>("_login.dat");
            if (r != null)
            {
                if (!string.IsNullOrEmpty(r["session"]?.ToString()))
                    App.Data.Session = JsonConvert.DeserializeObject<Session>(r["session"].ToString());
                App.Data.UserName = r["username"].ToString();
                App.Data.Password = r["password"].ToString();
            }
            return r;
        }


        public void SaveLocal(Session session, string username, string password, bool remember)
        {
            App.Data.Session = session;
            App.Data.UserName = username;
            App.Data.Password = password;
            App.Data.Remember = remember;
            if (remember)
                Save("_login.dat",
                    new Dictionary<string, object>
                    {
                        {"username", username},
                        {"password", password},
                        {"session", App.Data.Session}
                    });
            else
            {
                Save("_login.dat",
                    new Dictionary<string, object>
                    {
                        {"username", ""},
                        {"password", ""},
                        {"session", App.Data.Session}
                    });
            }
        }

        public void Save(string filePath, object data)
        {
            var path = _fileStore.PathCombine(AppName, filePath);
            _fileStore.EnsureFolderExists(AppName);
            _fileStore.WriteFile(path, JsonConvert.SerializeObject(data));
        }

        public T Load<T>(string filePath)
        {
            var path = _fileStore.PathCombine(AppName, filePath);
            var content = "";
            _fileStore.TryReadTextFile(path, out content);
            if (!string.IsNullOrEmpty(content))
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default(T);
        }

        public void Delete(string file)
        {
            try
            {
                var path = _fileStore.PathCombine(AppName, file);
                _fileStore.DeleteFile(path);
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public void SaveSettings()
        {
            Save("settings.dat", App.Data.Settings);
        }

        public Settings LoadSettings()
        {
            App.Data.Settings = Load<Settings>("settings.dat");
            if (App.Data.Settings == null)
                App.Data.Settings = new Settings();
            App.Data.Settings.PropertyChanged += (s, e) => { SaveSettings(); };
            return App.Data.Settings;
        }

        public void DeleteLocal()
        {
            Delete("_login.dat");
        }
    }
}