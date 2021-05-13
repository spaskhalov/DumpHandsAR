using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace DumpHandsAR
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GestureRecognizer _gestureRecognizer;
        [SerializeField] private ARRaycastManager _arRaycastManager;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _fireBallPrefab;
        [SerializeField] private float _fireballPower = 1000f;

        private Transform _cameraTransform;
        private Vector2 _centerOfScreen;

        private void Start()
        {
            _gestureRecognizer.GestureChanged += GestureRecognizerOnGestureChanged;
            _cameraTransform = Camera.main.transform;
            _centerOfScreen = new Vector2((float)Screen.width / 2, (float)Screen.height / 2);
        }

        private void GestureRecognizerOnGestureChanged(Gesture obj)
        {
            switch (obj)
            {
                case Gesture.Rock:
                    SpawnWall();
                    break;
                case Gesture.HighFive:
                    SpawnFireball();
                    break;
            }
        }

        private void SpawnFireball()
        {
            var fireball = Instantiate(_fireBallPrefab, _cameraTransform.position, Quaternion.identity);
            var body = fireball.GetComponent<Rigidbody>();
            body.AddForce(_cameraTransform.forward * _fireballPower);
        }

        private void SpawnWall()
        {
            var ray = new Ray(_cameraTransform.position, _cameraTransform.forward * 20);
            var rez = new List<ARRaycastHit>();
            
            if (_arRaycastManager.Raycast(_centerOfScreen, rez, TrackableType.PlaneWithinBounds))
            {
                var targetPos = rez.First().sessionRelativePose.position;
                Instantiate(_wallPrefab, targetPos, Quaternion.identity);
            }
            
            #if UNITY_EDITOR
            Instantiate(_wallPrefab, _cameraTransform.position, Quaternion.identity);
            #endif
        }
    }
}