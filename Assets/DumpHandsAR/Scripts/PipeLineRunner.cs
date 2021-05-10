using System;
using MediaPipe.HandPose;
using UnityEngine;

namespace DumpHandsAR
{
    public class PipeLineRunner : MonoBehaviour
    {
        [SerializeField] VideoInputController videoInputController = null;
        [SerializeField] ResourceSet _resources = null;

        public HandPipeline Pipeline { get; private set; }
        
        void Awake()
        {
            Pipeline = new HandPipeline(_resources);
        }
    
        private void OnGUI()
        {
            if(videoInputController.Texture != null)
                GUI.DrawTexture(new Rect(0, 0, 512, 512), videoInputController.Texture, ScaleMode.ScaleToFit);
        }
        
        void OnDestroy()
        {
            Pipeline.Dispose();
        }
    
        void LateUpdate()
        {
            if(videoInputController.Texture != null)
            // Feed the input image to the Hand pose pipeline.
                Pipeline.ProcessImage(videoInputController.Texture);
        }
    }
}