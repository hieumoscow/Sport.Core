using System.Collections.Generic;
using Sport.Core.Models;
using Sport.Core.Models.EndPoints;

namespace Sport.Core.Services.Interfaces
{
    public interface IFileService
    {
        void Save(string file, object data);
        T Load<T>(string file);

        void Delete(string file);
        Dictionary<string, object> LoadLocal();
        void SaveLocal(Session session, string username, string password, bool remember);
        void SaveSettings();
        Settings LoadSettings();
        void DeleteLocal();
    }
}