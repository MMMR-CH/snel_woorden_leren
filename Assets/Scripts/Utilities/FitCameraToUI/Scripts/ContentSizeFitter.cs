using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PalaGames.CameraManagement
{
    /// <summary>
    /// Adjusts the content size and position to fit the content defined by ContentBoundsSceneContainer within the UI area defined by ContentBoundsUIContainer.
    /// Works only if the UI Canvas is in Screen Space - Camera mode.
    /// </summary>
    public class ContentSizeFitter
    {
        #region FIELDS AND PROPERTIES
        ContentBoundsSceneContainer contentBoundsController;
        ContentBoundsUIContainer contentBoundsUIController;
        CameraHelper cameraHelper;
        #endregion

        #region CONSTRUCTOR
        public ContentSizeFitter(ContentBoundsSceneContainer contentBoundsController, ContentBoundsUIContainer contentBoundsUIController, CameraHelper cameraHelper, bool adjustOnStart = true)
        {
            this.contentBoundsController = contentBoundsController;
            this.contentBoundsUIController = contentBoundsUIController;
            this.cameraHelper = cameraHelper;

            if (adjustOnStart) Adjust();
        }
        #endregion

        #region METHODS
        [InspectorButton("Adjust Content Size and Position")]
        public void Adjust()
        {
            if (!cameraHelper) return;

            float contentWidth = Mathf.Abs(contentBoundsController.bottomRightCornerReferenceObject.position.x - contentBoundsController.topLeftCornerReferenceObject.position.x);
            float contentHeight = Mathf.Abs(contentBoundsController.topLeftCornerReferenceObject.position.y - contentBoundsController.bottomRightCornerReferenceObject.position.y);
            float contentAspectRatio = contentWidth / contentHeight;
        }
        #endregion
    }
}