using UnityEngine;
using UnityEngine.UI;

namespace DumpHandsAR
{
    public sealed class WebCamTextureInput : MonoBehaviour, IRawVideoInput
    {
        [SerializeField] RawImage _mainUI = null;
        [SerializeField] Vector2Int _resolution = new Vector2Int(1920, 1080);
        [SerializeField] float _frameRate = 60;

        WebCamTexture _webcam;

        public Texture Texture => _webcam.width > 50 ? _webcam : null;
        public int VideoRotationAngle => _webcam.videoRotationAngle;
        public bool VideoVerticallyMirrored => _webcam.videoVerticallyMirrored;

        void Start()
        {
            _webcam = new WebCamTexture(_resolution.x, _resolution.y, (int)_frameRate);
            
            _webcam.Play();
        }
    
        void OnDestroy()
        {
            if (_webcam != null) Destroy(_webcam);
        }

        private void LateUpdate()
        {
            // UI update
            _mainUI.texture = Texture;
        }
    }
}