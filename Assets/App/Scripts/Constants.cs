using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class PhysicsConstants
    {
        private static int defaultLayer = -1;
        private static int transparentFX = -1;
        private static int ignoreRaycast = -1;
        private static int layer3 = -1;
        private static int water = -1;
        private static int ui = -1;
        private static int layer6 = -1;
        private static int layer7 = -1;

        public static int Default
        {
            get
            {
                if (defaultLayer == -1)
                {
                    defaultLayer = LayerMask.NameToLayer("Default");
                }
                return defaultLayer;
            }
        }

        public static int TransparentFX
        {
            get
            {
                if (transparentFX == -1)
                {
                    transparentFX = LayerMask.NameToLayer("TransparentFX");
                }
                return transparentFX;
            }
        }

        public static int IgnoreRaycast
        {
            get
            {
                if (ignoreRaycast == -1)
                {
                    ignoreRaycast = 2;
                }
                return ignoreRaycast;
            }
        }
        public static int Layer3
        {
            get
            {
                if (layer3 == -1)
                {
                    layer3 = 3;
                }
                return layer3;
            }
        }

        public static int Water
        {
            get
            {
                if (water == -1)
                {
                    water = LayerMask.NameToLayer("Water");
                }
                return water;
            }
        }

        public static int UI
        {
            get
            {
                if (ui == -1)
                {
                    ui = LayerMask.NameToLayer("UI");
                }
                return ui;
            }
        }

        public static int Layer6
        {
            get
            {
                if (layer6 == -1)
                {
                    layer6 = 6;
                }
                return layer6;
            }
        }

        public static int Layer7
        {
            get
            {
                if (layer7 == -1)
                {
                    layer7 = 7;
                }
                return layer7;
            }
        }
    }

    public static class UISortingLayers
    {
        private static int defaultUI = -1;
        public static int Default
        {
            get
            {
                if (defaultUI == -1)
                {
                    defaultUI = SortingLayer.GetLayerValueFromName("Default");
                }
                return defaultUI;
            }
        }
    }

    public static class Tags
    {
        public const string UNTAGGED = "Untagged";
        public const string FINISH = "Finish";
        public const string CAMERA_MAIN = "MainCamera";
        public const string RESPAWN = "Respawn";
        public const string EDITOR_ONLY = "EditorOnly";
        public const string PLAYER = "Player";
        public const string GAME_CONTROLLER = "GameController";
    }

    public static class Shaders
    {
        public const string MainTexture = "_MainTex";
    }

    public static class Services
    {
        public const string WriteFile = "app.dat";
        public const string URL_MAIN = "";
    }

    public static class Resources
    {

    }
}
