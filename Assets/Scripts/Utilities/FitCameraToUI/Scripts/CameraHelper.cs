using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PalaGames.CameraManagement
{
    [RequireComponent(typeof(Camera))]
    public class CameraHelper : MonoBehaviour
    {
        #region FIELDS AND PROPERTIES
        public const int DEFAULT_PIXELS_PER_UNIT = 100;

        [field: SerializeField] public Camera Cam { get; private set; }

        float CameraWidth => Cam.aspect * Cam.orthographicSize * 2f;
        float CameraHeight => Cam.orthographicSize * 2f;
        float AspectRatio => Cam.aspect;
        #endregion


        public void SetCameraWidth(float requestedWidth)
        {
            float requiredHeight = requestedWidth / Cam.aspect;
            Cam.orthographicSize = requiredHeight / 2f;
        }

        public void SetCameraHeight(float requestedHeight)
        {
            Cam.orthographicSize = requestedHeight / 2f;
        }

        void Reset()
        {
            Cam = GetComponent<Camera>();
        }
    }
}
