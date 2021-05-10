using UnityEngine;

namespace DumpHandsAR
{
    public class HandDrawer : MonoBehaviour
    {
        [SerializeField] private PipeLineRunner _pipeLineRunner;
        [SerializeField] Shader _keyPointShader = null;
        [SerializeField] Shader _handRegionShader = null;
        
        (Material keys, Material region) _material;

        void Start()
        {
            _material = (new Material(_keyPointShader),
                new Material(_handRegionShader));
    
            // Material initial setup
            _material.keys.SetBuffer("_KeyPoints", _pipeLineRunner.Pipeline.KeyPointBuffer);
            _material.region.SetBuffer("_Image", _pipeLineRunner.Pipeline.HandRegionCropBuffer);
        }
    
        void OnDestroy()
        {
            Destroy(_material.keys);
            Destroy(_material.region);
        }
    
        void OnRenderObject()
        {
            // Key point circles
            _material.keys.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 96, 21);
    
            // Skeleton lines
            _material.keys.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 4 * 5 + 1);
        }
    }
}