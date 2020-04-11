using System.Collections;
using System.Collections.Generic;
using Scripts.Services;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadAssetBundles : DownloadController<AssetBundle>
{
    protected override bool IsUnityObject => true;

    protected override AssetBundle ParseFromHandler(DownloadHandler handler)
    {
        return null;
    }
}
