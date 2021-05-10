using UnityEngine;

namespace DumpHandsAR
{
    public interface IRawVideoInput
    {
        Texture Texture { get; }
        int VideoRotationAngle { get; }
        bool VideoVerticallyMirrored { get; }
    }
    
}