using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PalaGames.CameraManagement
{
    /// <summary>
    /// Adjusts the camera size and position to fit the content defined by ContentBoundsSceneContainer within the UI area defined by ContentBoundsUIContainer.
    /// Works only if the UI Canvas is in Screen Space - Overlay mode.
    /// </summary>
    public class CameraSizeFitter
    {
        #region FIELDS AND PROPERTIES
        ContentBoundsSceneContainer contentBoundsController;
        ContentBoundsUIContainer contentBoundsUIController;
        CameraHelper cameraHelper; 
        #endregion

        #region CONSTRUCTOR
        public CameraSizeFitter(ContentBoundsSceneContainer contentBoundsController, ContentBoundsUIContainer contentBoundsUIController, CameraHelper cameraHelper, bool adjustOnStart = true)
        {
            this.contentBoundsController = contentBoundsController;
            this.contentBoundsUIController = contentBoundsUIController;
            this.cameraHelper = cameraHelper;

            if (adjustOnStart) AdjustCamera();
        }
        #endregion

        #region METHODS
        [InspectorButton("Adjust Camera Size and Position")]
        // This is just for demonstration. Do this only when it is needed (if you happen to call it in Start method, wait for single frame to allow UI to get refreshed first).
        public async void AdjustCamera()
        {
            if (!cameraHelper) return;

            float contentWidth = Mathf.Abs(contentBoundsController.bottomRightCornerReferenceObject.position.x - contentBoundsController.topLeftCornerReferenceObject.position.x);
            float contentHeight = Mathf.Abs(contentBoundsController.topLeftCornerReferenceObject.position.y - contentBoundsController.bottomRightCornerReferenceObject.position.y);
            float contentAspectRatio = contentWidth / contentHeight;

            var uiTopDeltaY = contentBoundsUIController.TopReferenceObjectTopLeft.position.y - contentBoundsUIController.TopReferenceObjectBottomRight.position.y;
            var uiBottomDeltaY = contentBoundsUIController.BottomReferenceObjectTopLeft.position.y - contentBoundsUIController.BottomReferenceObjectBottomRight.position.y;
            var centerAreaWidth = Mathf.Abs(contentBoundsUIController.TopReferenceObjectBottomRight.position.x - contentBoundsUIController.BottomReferenceObjectTopLeft.position.x);
            var centerAreaHeight = Mathf.Abs(contentBoundsUIController.TopReferenceObjectBottomRight.position.y - contentBoundsUIController.BottomReferenceObjectTopLeft.position.y);
            var centerAreaAspectRatio = centerAreaWidth / centerAreaHeight;

            float topRatio = Mathf.Abs(uiTopDeltaY / centerAreaHeight);
            float bottomRatio = Mathf.Abs(uiBottomDeltaY / centerAreaHeight);

            float extraHeightTop;
            float extraHeightBottom;

            if (contentAspectRatio >= centerAreaAspectRatio)
            {
                cameraHelper.SetCameraWidth(contentWidth);

                contentHeight = contentWidth / centerAreaAspectRatio;
                extraHeightTop = contentHeight * topRatio;
                extraHeightBottom = contentHeight * bottomRatio;
            }
            else
            {
                extraHeightTop = contentHeight * topRatio;
                extraHeightBottom = contentHeight * bottomRatio;

                float finalHeight = extraHeightTop + contentHeight + extraHeightBottom;

                cameraHelper.SetCameraHeight(finalHeight);
            }

            // yield one frame. so that UI layout can update before calculating camera position
            if (Application.isPlaying) await UniTask.Yield();
            else await UniTask.WaitForSeconds(0.1f); // for editor();

            Vector3 cameraPos = new Vector3();
            Vector3 cameraBottomReferenceObjectTopLeftDifference = contentBoundsUIController.BottomReferenceObjectTopLeft.position - cameraHelper.Cam.transform.position;
            cameraPos.x = (contentBoundsController.topLeftCornerReferenceObject.position.x + contentBoundsController.bottomRightCornerReferenceObject.position.x) / 2f;
            cameraPos.y = (contentBoundsController.topLeftCornerReferenceObject.position.y + contentBoundsController.bottomRightCornerReferenceObject.position.y + extraHeightTop - extraHeightBottom) / 2f;
            cameraPos.z = cameraHelper.Cam.transform.position.z;
            cameraHelper.Cam.transform.position = cameraPos;
            if (contentBoundsUIController.Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                cameraPos.y += contentBoundsController.bottomRightCornerReferenceObject.position.y - cameraHelper.Cam.ScreenToWorldPoint(contentBoundsUIController.BottomReferenceObjectTopLeft.position).y;
            }
            else if (contentBoundsUIController.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cameraPos.y += contentBoundsController.bottomRightCornerReferenceObject.position.y - (cameraBottomReferenceObjectTopLeftDifference + cameraHelper.Cam.transform.position).y;
            }
            cameraHelper.Cam.transform.position = cameraPos;
        }
        #endregion
    }
}
