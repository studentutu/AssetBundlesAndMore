using UnityEngine;
namespace Scripts.Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteStretch : MonoBehaviour
    {
        [SerializeField] private RectTransform currentRect = null;
        [SerializeField] private bool keepAspectRatio = false;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private static Vector3? topRightCorner = null;
        private static Vector3 TopRightCorner
        {
            get
            {
                if (topRightCorner == null)
                {
                    var mainCam = Camera.main;
                    topRightCorner = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCam.transform.position.z));
                }
                return topRightCorner.Value;
            }
        }

        private void OnValidate()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentRect = GetComponent<RectTransform>();
            Init();
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            var worldSpaceWidth = TopRightCorner.x * 2;
            var worldSpaceHeight = TopRightCorner.y * 2;

            var spriteSize = spriteRenderer.bounds.size;

            var scaleFactorX = worldSpaceWidth / spriteSize.x;
            var scaleFactorY = worldSpaceHeight / spriteSize.y;

            if (keepAspectRatio)
            {
                if (scaleFactorX > scaleFactorY)
                {
                    scaleFactorY = scaleFactorX;
                }
                else
                {
                    scaleFactorX = scaleFactorY;
                }
            }

            gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
        }
    }
}