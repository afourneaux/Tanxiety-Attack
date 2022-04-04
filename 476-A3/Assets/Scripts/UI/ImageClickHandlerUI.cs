using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ImageClickHandlerUI : MonoBehaviour, IPointerDownHandler
{
    BoxCollider2D col;
    public ColourPickerUI colourPicker;

    void Start() {
        col = GetComponent<BoxCollider2D>();
    }

    // Determines where on the colour picker the user selected in order to pick an s and v colour value
    public void OnPointerDown (PointerEventData eventData) {
        float y = eventData.position.y - transform.position.y + (col.size.y / 2);
        float x = eventData.position.x - transform.position.x + (col.size.x / 2);

        float s = x / 200;
        float v = y / 200;

        colourPicker.SetSV(s, v);
    }
}
