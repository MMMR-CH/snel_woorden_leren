using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PalaGames.CameraManagement
{   
    public class ContentBoundsUIContainer : MonoBehaviour
    {
        [Header("UI References")]
        [field: SerializeField] public Canvas Canvas { get; private set; }
        [field: SerializeField] public RectTransform TopReferenceObjectTopLeft { get; private set; }
        [field: SerializeField] public RectTransform TopReferenceObjectBottomRight { get; private set; }
        [field: SerializeField] public RectTransform BottomReferenceObjectTopLeft { get; private set; }
        [field: SerializeField] public RectTransform BottomReferenceObjectBottomRight { get; private set; }

        [InspectorButton("Generate Reference Objects")]
        public void GenerateReferenceObjects()
        {
            if (TopReferenceObjectTopLeft == null)
            {
                GameObject topLeftGO = new GameObject("[TopLeftCornerReferenceObject]");
                topLeftGO.transform.SetParent(transform);
                TopReferenceObjectTopLeft = topLeftGO.AddComponent<RectTransform>();
                TopReferenceObjectTopLeft.localPosition = Vector3.zero;
                TopReferenceObjectTopLeft.localScale = Vector3.one;
            }

            if (TopReferenceObjectBottomRight == null)
            {
                GameObject topRightGO = new GameObject("[TopRightCornerReferenceObject]");
                topRightGO.transform.SetParent(transform);
                TopReferenceObjectBottomRight = topRightGO.AddComponent<RectTransform>();
                TopReferenceObjectBottomRight.localPosition = Vector3.zero;
                TopReferenceObjectBottomRight.localScale = Vector3.one;
            }

            if (BottomReferenceObjectTopLeft == null)
            {
                GameObject bottomLeftGO = new GameObject("[BottomLeftCornerReferenceObject]");
                bottomLeftGO.transform.SetParent(transform);
                BottomReferenceObjectTopLeft = bottomLeftGO.AddComponent<RectTransform>();
                BottomReferenceObjectTopLeft.localPosition = Vector3.zero;
                BottomReferenceObjectTopLeft.localScale = Vector3.one;
            }

            if (BottomReferenceObjectBottomRight == null)
            {
                GameObject bottomRightGO = new GameObject("[BottomRightCornerReferenceObject]");
                bottomRightGO.transform.SetParent(transform);
                BottomReferenceObjectBottomRight = bottomRightGO.AddComponent<RectTransform>();
                BottomReferenceObjectBottomRight.localPosition = Vector3.zero;
                BottomReferenceObjectBottomRight.localScale = Vector3.one;
            }
        }
    }
}
