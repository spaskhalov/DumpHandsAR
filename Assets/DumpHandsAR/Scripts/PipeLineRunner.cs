using System;
using MediaPipe.HandPose;
using UnityEngine;

namespace DumpHandsAR
{
    public class PipeLineRunner : MonoBehaviour
    {
        [SerializeField] WebCamTextureInput _webcam = null;
        [SerializeField] ResourceSet _resources = null;

        public HandPipeline Pipeline { get; private set; }
        
        void Awake()
        {
            Pipeline = new HandPipeline(_resources);
        }
    
        private void OnGUI()
        {
            if(_webcam.Texture != null)
                GUI.DrawTexture(new Rect(0, 0, 512, 512), _webcam.Texture, ScaleMode.ScaleToFit);
        }
        
        void OnDestroy()
        {
            Pipeline.Dispose();
        }
    
        void LateUpdate()
        {
            // Feed the input image to the Hand pose pipeline.
            Pipeline.ProcessImage(_webcam.Texture);
        }
    }
}