using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.IGroundPlane
{
    public class DynamicImageLibraryEntry
    {
        public Texture2D texture = null;
        public string id = null;

        /// <summary>
        /// if not set - Ar Foundation will calculate size from texture
        /// </summary>
        public float? widthInMeters = null;
    }
}