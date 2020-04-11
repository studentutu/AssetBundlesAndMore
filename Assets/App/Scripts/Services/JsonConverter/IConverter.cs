using System.Collections;
using System.Collections.Generic;

namespace Scripts.Services
{
    public interface IConverter : IServices
    {
        string SaveFileWithExtension { get; }

        T FromJson<T>(string json);
        string ToJson<T>(T toJson);


        T LoadData<T>(string filePath);

        void SaveFile(string filePath, object data);
    }
}
