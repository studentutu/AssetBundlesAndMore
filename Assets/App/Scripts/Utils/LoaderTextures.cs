using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scripts.Services;
using UnityEngine;

public static class LoaderTextures
{
    /// <summary>
    /// Parse text in base64 string into Texture2D
    /// </summary>
    public static Texture2D ParseToTexture(string initialBase64data)
    {
        string textureBase64 = TrimMetaDAta(initialBase64data);
        byte[] imageBytes = System.Convert.FromBase64String(textureBase64);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// Parse text in base64 string into Texture2D
    /// </summary>
    public static string ParseToBase64(Texture2D initialImage)
    {
        return Convert.ToBase64String(initialImage.EncodeToPNG());
    }

    private static string TrimMetaDAta(string jsonBase64)
    {
        int indeOfComma = jsonBase64.IndexOf(',');
        indeOfComma++;
        var result = jsonBase64.Substring(indeOfComma); // not include comma!
                                                        // result = result.Trim();
        return result;
    }
}
