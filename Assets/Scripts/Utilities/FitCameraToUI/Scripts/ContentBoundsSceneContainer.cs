using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PalaGames.CameraManagement
{   
    public class ContentBoundsSceneContainer : MonoBehaviour
    {
        [field: SerializeField] public Transform topLeftCornerReferenceObject { get; private set; }
        [field: SerializeField] public Transform bottomRightCornerReferenceObject { get; private set; }

        void OnDrawGizmos()
        {
            if (topLeftCornerReferenceObject && bottomRightCornerReferenceObject)
            {
                Gizmos.color = Color.red;
                DrawRectangleGizmo(topLeftCornerReferenceObject.position, bottomRightCornerReferenceObject.position);
            }
        }

        void DrawRectangleGizmo(Vector3 topLeftCornerPos, Vector3 bottomRightCornerPos)
        {
            topLeftCornerPos.z = 0f;
            bottomRightCornerPos.z = 0f;
            Vector3 topRightCornerPos = new Vector3(bottomRightCornerPos.x, topLeftCornerPos.y, 0f);
            Vector3 bottomLeftCornerPos = new Vector3(topLeftCornerPos.x, bottomRightCornerPos.y, 0f);

            Gizmos.DrawLine(topLeftCornerPos, topRightCornerPos);
            Gizmos.DrawLine(topRightCornerPos, bottomRightCornerPos);
            Gizmos.DrawLine(bottomRightCornerPos, bottomLeftCornerPos);
            Gizmos.DrawLine(bottomLeftCornerPos, topLeftCornerPos);
        }

        [InspectorButton("Generate Reference Objects")]
        public void GenerateReferenceObjects()
        {
            if (topLeftCornerReferenceObject == null)
            {
                GameObject topLeftGO = new GameObject("TopLeftCornerReferenceObject");
                topLeftGO.transform.SetParent(transform);
                topLeftCornerReferenceObject = topLeftGO.transform;
                topLeftCornerReferenceObject.localPosition = Vector3.zero;
            }

            if (bottomRightCornerReferenceObject == null)
            {
                GameObject bottomRightGO = new GameObject("BottomRightCornerReferenceObject");
                bottomRightGO.transform.SetParent(transform);
                bottomRightCornerReferenceObject = bottomRightGO.transform;
                bottomRightCornerReferenceObject.localPosition = Vector3.zero;
            }
        }
    }
}
