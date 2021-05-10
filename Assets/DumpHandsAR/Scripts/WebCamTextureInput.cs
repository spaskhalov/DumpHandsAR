using UnityEngine;
using UnityEngine.UI;

namespace DumpHandsAR
{
    public sealed class WebCamTextureInput : MonoBehaviour
    {
        [SerializeField] RawImage _mainUI = null;
        [SerializeField] Vector2Int _resolution = new Vector2Int(1920, 1080);
        [SerializeField] float _frameRate = 60;
        [SerializeField] private Material _blitMaterial;

        WebCamTexture _webcam;
        RenderTexture _buffer;

        public Texture Texture => _buffer;
        
    
        void Start()
        {
            _webcam = new WebCamTexture(_resolution.x, _resolution.y, (int)_frameRate);
            
            _webcam.Play();
        }
    
        void OnDestroy()
        {
            if (_webcam != null) Destroy(_webcam);
            if (_buffer != null) Destroy(_buffer);
        }
    
        void Update()
        {
            if (!_webcam.didUpdateThisFrame) return;
            // if(_webcam.width < 100 && _webcam.height < 100)
            //     return;

            if (_buffer == null)
            {
                var isRotated = _webcam.videoRotationAngle == 90 || _webcam.videoRotationAngle == -90;
                var width = !isRotated ? _webcam.width : _webcam.height;
                var height = !isRotated ? _webcam.height : _webcam.width;
                _buffer = new RenderTexture(width, height, 0);
                
                _mainUI.GetComponent<AspectRatioFitter>().aspectRatio = (float)width / height;
                
                var vflip = _webcam.videoVerticallyMirrored;
                var scale = new Vector2(1, vflip ? -1 : 1);

                _blitMaterial.SetFloat("_RotationAngle", - _webcam.videoRotationAngle);
                _blitMaterial.SetVector("_Scale", new Vector4(scale.x, scale.y, 0,0));
            }
            
            Graphics.Blit(_webcam, _buffer, _blitMaterial);
        }

        private void LateUpdate()
        {
            // UI update
            _mainUI.texture = Texture;
        }
    }
}