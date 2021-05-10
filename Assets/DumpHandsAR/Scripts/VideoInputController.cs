using System;
using UnityEngine;

namespace DumpHandsAR
{
    public class VideoInputController : MonoBehaviour, IVideoInput
    {
        [SerializeField] WebCamTextureInput _editorVideoInput;
        [SerializeField] ARFoundationVideoInput _phoneVideoInput;
        [SerializeField] private Material _blitMaterial;

        private IRawVideoInput _rawVideoInput;
        private RenderTexture _buffer;
        
        public Texture Texture => _buffer;
        private bool VideoIsRotated => _rawVideoInput.VideoRotationAngle == 90 || _rawVideoInput.VideoRotationAngle == -90;
        
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _rawVideoInput = _phoneVideoInput;
                Destroy(_editorVideoInput.gameObject);
            }
            else
            {
                _rawVideoInput = _editorVideoInput;
                Destroy(_phoneVideoInput.gameObject);
            }
        }

        private void Update()
        {
            if(_rawVideoInput.Texture == null)
                return;
            if (_buffer == null)
            {
                var tWidth = !VideoIsRotated ? _rawVideoInput.Texture.width : _rawVideoInput.Texture.height;
                var tHeight = !VideoIsRotated ? _rawVideoInput.Texture.height : _rawVideoInput.Texture.width;
                Debug.Log($"buffer width: {tWidth}, height:{tHeight}");
                _buffer = new RenderTexture(tWidth, tHeight, 0);
            }
            
            var vflip = _rawVideoInput.VideoVerticallyMirrored;
            var scale = new Vector2(1, vflip ? -1 : 1);
            _blitMaterial.SetFloat("_RotationAngle", -_rawVideoInput.VideoRotationAngle);
            _blitMaterial.SetVector("_Scale", new Vector4(scale.x, scale.y, 0,0));

            Graphics.Blit(_rawVideoInput.Texture, _buffer, _blitMaterial);
        }

    }
}