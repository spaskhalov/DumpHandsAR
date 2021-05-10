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