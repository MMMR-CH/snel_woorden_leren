using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class UnityEditorExtensionMethods
{
    #region IMAGE   
    [MenuItem("CONTEXT/Image/Rename")]
    public static void NameImageObj(MenuCommand command)
    {
        Image _image = (Image)command.context;
        _image.gameObject.name = _image.sprite.name;
    }

    [MenuItem("CONTEXT/Image/Resize")]
    public static void ResizeImageObj(MenuCommand command)
    {
        Image _image = (Image)command.context;
        if (_image.TryGetComponent(out RectTransform rectTr))
        {
            rectTr.sizeDelta = new Vector2(_image.sprite.rect.width, _image.sprite.rect.height);
        }
    }
    #endregion


    #region AUDIOSOURCE
    [MenuItem("CONTEXT/AudioSource/Rename")]
    public static void NameASObj(MenuCommand command)
    {
        AudioSource asource = (AudioSource)command.context;
        asource.gameObject.name = asource.clip.name;
    }
    #endregion

    #region SpriteRenderer
    [MenuItem("CONTEXT/SpriteRenderer/Rename")]
    public static void NameSpriteRendererObj(MenuCommand command)
    {
        SpriteRenderer _image = (SpriteRenderer)command.context;
        _image.gameObject.name = _image.sprite.name;
    }
    #endregion
}
