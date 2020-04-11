using System.Collections;
using System.Collections.Generic;
using Services.Serializer;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Completely separate from any of the managers, code and so on!
    private IDataSerializerBase _saverForLocalData;
    public IDataSerializerBase saverForLocalData
    {
        get
        {
            if (_saverForLocalData == null)
            {
                _saverForLocalData = returnPlayerSaverSerializer();
                InitActionsForDataSerializer();
            }
            return _saverForLocalData;
        }
        set
        {
            _saverForLocalData = value;
            InitActionsForDataSerializer();
        }
    }

    /// <summary> Default - Json. Only string Serializer will pass if using Json/XML serializer </summary>
    protected virtual IDataSerializerBase returnPlayerSaverSerializer()
    {
        return new PlayerPrefsSerializerPrimitiveString();
    }

    private void InitActionsForDataSerializer()
    {
        saverForLocalData.OnLoad -= OnLoadFromSerializerPlayerData;
        saverForLocalData.OnLoad += OnLoadFromSerializerPlayerData;
    }

    /// <summary> Default Deserialization - from Json string to models! </summary>
    protected virtual void OnLoadFromSerializerPlayerData(System.Object data)
    {
        if (data != null && !string.IsNullOrEmpty(data.ToString()))
        {
            Parse(data.ToString());
        }
    }


    private void Parse(string json)
    {

    }
}
