using UnityEditor;
using UnityEngine;

namespace KAUGamesLviv.Services.Bundles
{
    public struct EditorCustomStyles
    {
        private GUIStyle _header;
        public GUIStyle Header
        {
            get
            {
                if(_header == null)
                {
                    _header = new GUIStyle("box")
                    {
                        
                        fontSize = EditorStyles.boldLabel.fontSize,
                        fontStyle = EditorStyles.boldLabel.fontStyle,
                        font = EditorStyles.boldLabel.font,
                        alignment = TextAnchor.UpperLeft,
                        padding = new RectOffset(10, 0, 2, 0),
                        normal = {background = GUI.skin.GetStyle("ShurikenModuleTitle").normal.background}
                    };
                }
                return _header;
            }
        }

        private GUIStyle _boldText;
        public GUIStyle BoldText
        {
            get
            {
                if (_boldText == null)
                {
                    _boldText = new GUIStyle();
                    _boldText.fontStyle = FontStyle.Bold;
                }
                return _boldText;
            }
        }

        private GUIStyle _titleText;
        public GUIStyle TitleText
        {
            get
            {
                if (_titleText == null)
                {
                    _titleText = new GUIStyle(EditorStyles.boldLabel);
                    _titleText.fontSize = 19;
                    _titleText.alignment = TextAnchor.MiddleCenter;
                }
                return _titleText;
            }
        }
    }

    public class CustomExtendedEditor : Editor
    {
        public EditorCustomStyles styles;
    }
}