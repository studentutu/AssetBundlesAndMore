using Scripts;
using UnityEngine;

namespace Services.Serializer
{
    /// <summary> Each Primitive should be used in Here </summary>
    public class AppServiceSerializerString : IDataSerializer<string>
    {
        public override bool isSaveExist(string from) { return PlayerPrefs.HasKey(from); }

        protected override void SaveT(string data, string from)
        {
            PlayerPrefs.SetString(from, data);
            PlayerPrefs.Save();
        }

        protected override string LoadT(string from)
        {
            return PlayerPrefs.GetString(from, null);
        }

        public override void Delete(string from)
        {
            if (isSaveExist(from))
            {
                PlayerPrefs.DeleteKey(from);
            }
        }

    }
    /// <summary> Each Primitive should be used in Here </summary>
    public class AppServiceSerializerInt : IDataSerializer<int>
    {

        public override bool isSaveExist(string from) { return PlayerPrefs.HasKey(from); }

        protected override void SaveT(int data, string from)
        {
            PlayerPrefs.SetString(from, data.ToString());
            PlayerPrefs.Save();
        }

        protected override int LoadT(string from)
        {
            return int.Parse(PlayerPrefs.GetString(from, "0"));
        }

        public override void Delete(string from)
        {
            if (isSaveExist(from))
            {
                PlayerPrefs.DeleteKey(from);
            }
        }

    }

    /// <summary> Sadly doesn't work with primitive types! </summary>
    public class AppServiceSerializer<W> : IDataSerializer<W>
        where W : new()
    {

        public override bool isSaveExist(string from) { return PlayerPrefs.HasKey(from); }

        protected override void SaveT(W data, string from)
        {
            string dataJson = App.Services.JsonConverter.ToJson(data);
            PlayerPrefs.SetString(from, dataJson);
            PlayerPrefs.Save();
        }

        protected override W LoadT(string from)
        {
            W data = default(W);
            if (isSaveExist(from))
            {
                string dataJson = PlayerPrefs.GetString(from);
                if (!string.IsNullOrEmpty(dataJson))
                {
                    data = App.Services.JsonConverter.FromJson<W>(dataJson);
                }

            }
            return data;
        }

        public override void Delete(string from)
        {
            if (isSaveExist(from))
            {
                PlayerPrefs.DeleteKey(from);
            }
        }
    }
}