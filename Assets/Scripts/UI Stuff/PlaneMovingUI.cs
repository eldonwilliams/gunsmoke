using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    TODO: Update Comments b/c foreground is now whatever the Transform of PlaneMovingUI is
*/

/// <summary>
///  A UI object that consists of a foreground and a background.
///  The foreground can be dragged and the background will remain in the same place.
///  For normal use, the background should be the parent of the foreground.
///  That check isn't enforced
/// 
///  Background movements are captured using mouse events
/// </summary>
public class PlaneMovingUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    ///  A reference to the background object, this is the stationary object
    /// </summary>
    [Tooltip("A reference to the background object, this is the stationary object")]
    public RectTransform Background;

    private Image _image;

    void Start() {
        _image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
        transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
    }
}
