using System;
using MediaPipe.HandPose;
using UnityEngine;

namespace DumpHandsAR
{
    public class GestureRecognizer : MonoBehaviour
    {
        [Flags]
        private enum FingersState
        {
            Thumb = 1,
            Index = 2,
            Middle = 4,
            Ring = 8,
            Pinky = 16
        }
        [SerializeField] private PipeLineRunner pipeLineRunner;
        [SerializeField] private float gestureResetDelay = 0.5f;

        private Gesture _currentGesture;
        private FingersState _prevFingersState;
        private Gesture _nextGesture;
        private float _nextGestureTime = float.NaN;

        public event Action<Gesture> GestureChanged;
        
        public Gesture CurrentGesture
        {
            get => _currentGesture;
            private set
            {
                if (value != _currentGesture)
                {
                    _currentGesture = value;
                    GestureChanged?.Invoke(value);
                }
            }
        }

        private void Start()
        {
            GestureChanged += gesture => Debug.Log($"Gesture: {gesture.ToString()}");
        }

        private void Update()
        {
            var fingersState = GetFingersState();
            if(_prevFingersState != fingersState)
                Debug.Log($"Fingers: {fingersState:F}");
            _prevFingersState = fingersState;

            var targetGesture = GetGesture(fingersState);
            
            if (_nextGesture != targetGesture)
            {
                _nextGesture = targetGesture;
                _nextGestureTime = CurrentGesture == Gesture.None ? 0 : Time.time + gestureResetDelay;
            }
            
            if (!float.IsNaN(_nextGestureTime) && Time.time >= _nextGestureTime)
            {
                CurrentGesture = targetGesture;
                _nextGestureTime = float.NaN;
            }
            
        }

        private Gesture GetGesture(FingersState fingersState)
        {
            switch (fingersState)
            {
                case FingersState.Index | FingersState.Pinky:
                    return Gesture.Rock;
                case FingersState.Thumb | FingersState.Index | FingersState.Middle | FingersState.Ring | FingersState.Pinky:
                    return Gesture.HighFive;
            }

            return Gesture.None;
        }
        
        private FingersState GetFingersState()
        {
            FingersState newState = 0;
            if (!pipeLineRunner.Pipeline.HandIsVisible)
                return newState;

            if (FingerIsOpen(HandPipeline.KeyPoint.Thumb2, 
                HandPipeline.KeyPoint.Thumb3, HandPipeline.KeyPoint.Thumb4))
                newState = newState | FingersState.Thumb;
            if (FingerIsOpen(HandPipeline.KeyPoint.Index1, 
                HandPipeline.KeyPoint.Index3, HandPipeline.KeyPoint.Index4))
                newState = newState | FingersState.Index;
            if (FingerIsOpen(HandPipeline.KeyPoint.Middle1, 
                HandPipeline.KeyPoint.Middle3, HandPipeline.KeyPoint.Middle4))
                newState = newState | FingersState.Middle;
            if (FingerIsOpen(HandPipeline.KeyPoint.Ring1, 
                HandPipeline.KeyPoint.Ring3, HandPipeline.KeyPoint.Ring4))
                newState = newState | FingersState.Ring;
            if (FingerIsOpen(HandPipeline.KeyPoint.Pinky1, 
                HandPipeline.KeyPoint.Pinky3, HandPipeline.KeyPoint.Pinky4))
                newState = newState | FingersState.Pinky;
            return newState;
        }

        private bool FingerIsOpen(HandPipeline.KeyPoint refEndPoint, HandPipeline.KeyPoint targetStartPoint,
            HandPipeline.KeyPoint targetEndPoint)
        {
            var referenceVectorStart = pipeLineRunner.Pipeline.GetKeyPoint(HandPipeline.KeyPoint.Wrist);
            var referenceVectorEnd = pipeLineRunner.Pipeline.GetKeyPoint(refEndPoint);
            var targetVectorStart = pipeLineRunner.Pipeline.GetKeyPoint(targetStartPoint);
            var targetVectorEnd = pipeLineRunner.Pipeline.GetKeyPoint(targetEndPoint);
            
            var refDirection = (referenceVectorEnd - referenceVectorStart).normalized;
            var compareDirection = (targetVectorEnd - targetVectorStart).normalized;

            var dot = Vector3.Dot(refDirection, compareDirection);
            return dot > 0.7;
        }
    }

    public enum Gesture
    {
        None,
        Rock,
        HighFive
    }
}