using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

namespace DumpHandsAR
{
    public class ARFoundationVideoInput : MonoBehaviour, IRawVideoInput
    {
        [SerializeField]
        private ARCameraManager _cameraManager;
        
        private Texture2D _texture;

        private void Start()
        {
            _cameraManager.frameReceived += OnFrameReceived;
        }
        
        public void Dispose()
        {
            if (_cameraManager != null)
            {
                _cameraManager.frameReceived -= OnFrameReceived;
            }

            Object.Destroy(_texture);
        }

        private void OnFrameReceived(ARCameraFrameEventArgs frameArgs)
        {
            UpdateTexture();
        }

        private void UpdateTexture()
        {
            if (!_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                return;
            }

            var conversionParams = new XRCpuImage.ConversionParams
            {
                // Get the entire image.
                inputRect = new RectInt(0, 0, image.width, image.height),
                // Downsample.
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
                // Choose RGBA format.
                outputFormat = TextureFormat.RGBA32,
            };

            // See how many bytes you need to store the final image.
            int size = image.GetConvertedDataSize(conversionParams);

            // Allocate a buffer to store the image.
            var buffer = new NativeArray<byte>(size, Allocator.Temp);

            // Extract the image data
            unsafe
            {
                image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
            }

            // The image was converted to RGBA32 format and written into the provided buffer
            // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
            image.Dispose();

            // At this point, you can process the image, pass it to a computer vision algorithm, etc.
            // In this example, you apply it to a texture to visualize it.

            // You've got the data; let's put it into a texture so you can visualize it.
            if (_texture == null ||
                _texture.width != conversionParams.outputDimensions.x ||
                _texture.height != conversionParams.outputDimensions.y)
            {
                if (_texture) Object.Destroy(_texture);
                _texture = new Texture2D(
                    conversionParams.outputDimensions.x,
                    conversionParams.outputDimensions.y,
                    conversionParams.outputFormat,
                    false);
            }

            _texture.LoadRawTextureData(buffer);
            _texture.Apply();

            buffer.Dispose();
        }

        public Texture Texture => _texture;
        public int VideoRotationAngle => 90;
        public bool VideoVerticallyMirrored => true;
    }
}