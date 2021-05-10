using UnityEngine;
using UnityEngine.UI;

namespace DumpHandsAR
{
    public sealed class WebCamTextureInput : MonoBehaviour
    {
        [SerializeField] RawImage _mainUI = null;
        [SerializeField] string _deviceName = "";
        [SerializeField] Vector2Int _resolution = new Vector2Int(1920, 1080);
        [SerializeField] float _frameRate = 60;
        [SerializeField] Texture2D _dummyImage = null;

        WebCamTexture _webcam;
        RenderTexture _buffer;

        public Texture Texture
            => _dummyImage != null ? (Texture)_dummyImage : (Texture)_buffer;
        
    
        void Start()
        {
            if (_dummyImage != null) return;
            _webcam = new WebCamTexture
                (_deviceName, _resolution.x, _resolution.y, (int)_frameRate);
            _buffer = new RenderTexture(_resolution.x, _resolution.y, 0);
            _webcam.Play();
        }
    
        void OnDestroy()
        {
            if (_webcam != null) Destroy(_webcam);
            if (_buffer != null) Destroy(_buffer);
        }
    
        void Update()
        {
            if (_dummyImage != null) return;
            if (!_webcam.didUpdateThisFrame) return;
    
            var aspect1 = (float)_webcam.width / _webcam.height;
            var aspect2 = (float)_resolution.x / _resolution.y;
            var gap = aspect2 / aspect1;
    
            var vflip = _webcam.videoVerticallyMirrored;
            var scale = new Vector2(gap, vflip ? -1 : 1);
            var offset = new Vector2((1 - gap) / 2, vflip ? 1 : 0);
    
            Graphics.Blit(_webcam, _buffer, scale, offset);
        }

        private void LateUpdate()
        {
            // UI update
            _mainUI.texture = Texture;
        }
    }
}