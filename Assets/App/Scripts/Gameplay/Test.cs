using System.Collections;
using System.Collections.Generic;
using Scripts;
using Scripts.Gameplay.Models;
using Scripts.Services;
using Scripts.Utils.Async;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private bool WriteImageToBase = false;
    [SerializeField] private Texture2D convertTo = null;
    [SerializeField] private TextAsset saveToBase64 = null;
    [SerializeField] private bool TestRawImage = false;
    [SerializeField] private RawImage imageTOPassIn = null;

    [SerializeField] private bool WriteUrls = false;
    [SerializeField] private MasterSlaveUrl modelUrlLists = null;
    [SerializeField] private TextAsset saveTo = null;
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (WriteImageToBase)
        {
            WriteImageToBase = false;
            ThreadTools.StartCoroutine(writeAsBase64());
        }

        if (WriteUrls)
        {
            WriteUrls = false;
            ThreadTools.StartCoroutine(writToCor());
        }

        if (TestRawImage)
        {
            TestRawImage = false;
            ThreadTools.StartCoroutine(testImage());
        }

        // Google Drive
        // foreach (var item in modelUrlLists.Urls)
        // {
        //     if (!string.IsNullOrEmpty(item.Url))
        //     {
        //         // parse
        //         item.Url = item.Url.Replace("open?id", "uc?export=download&id");
        //     }
        // }
    }

    private IEnumerator writToCor()
    {
        yield return null;
        var asConverterJsonUtility = App.Services.JsonConverter as ConverterJsonUtility;
        asConverterJsonUtility.WriteToLocalTextAsset(App.Services.JsonConverter.ToJson(modelUrlLists), saveTo);
    }

    private IEnumerator writeAsBase64()
    {
        yield return null;
        var newString = LoaderTextures.ParseToBase64(convertTo);

        var asConverterJsonUtility = App.Services.JsonConverter as ConverterJsonUtility;
        asConverterJsonUtility.WriteToLocalTextAsset(newString, saveToBase64);
        // Debug.LogWarning(newString);
        // App.SceneService.LoadSceneWithVideo(nextScene, null, 2);

        runAsync();
    }

    private IEnumerator testImage()
    {
        yield return null;
        var asConverterJsonUtility = App.Services.JsonConverter as ConverterJsonUtility;
        var stringToUse = asConverterJsonUtility.ReadAllFromTExtAsset(saveToBase64);
        var texture = LoaderTextures.ParseToTexture(stringToUse);
        if (imageTOPassIn != null)
        {
            imageTOPassIn.texture = texture;
        }
    }

    private async void runAsync()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning(" No Internet");
            return;
        }
        var DisposableObject = new IDisposableObject();
        var yieldFor = await App.Services.WebLoader.LoadData(DisposableObject, (msg) =>
       {
           Debug.LogError(" Error " + msg);
       },
        App.Services.WebLoader.MainUrl //+ "/pub?output=txt"
        );
        // Debug.LogWarning(yieldFor);
        var texture = LoaderTextures.ParseToTexture(yieldFor);
        if (imageTOPassIn != null)
        {
            imageTOPassIn.texture = texture;
        }
    }
#endif
}
