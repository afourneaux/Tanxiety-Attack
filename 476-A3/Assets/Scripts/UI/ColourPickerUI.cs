using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourPickerUI : MonoBehaviour
{
    public Material baseHsvMaterial;
    private Material hsvMaterial;
    private Image SliderHandleImage;
    public LobbyUI lobby;

    float h;
    float s;
    float v;

    void Start()
    {
        hsvMaterial = new Material(baseHsvMaterial.shader);
        hsvMaterial.CopyPropertiesFromMaterial(baseHsvMaterial);
        transform.Find("Image").GetComponent<Image>().material = hsvMaterial;
        SliderHandleImage = transform.Find("Hue/Handle Slide Area/Handle").GetComponent<Image>();
        
        float defaultHue = hsvMaterial.GetFloat("_Hue");
        transform.Find("Hue").GetComponent<Slider>().value = defaultHue;

        h = defaultHue;
        s = 1f;
        v = 1f;
        
        SliderHandleImage.color = Color.HSVToRGB(defaultHue, 1, 1);
    }

    public void OnSliderUpdate(float value) {
        h = value;
        hsvMaterial.SetFloat("_Hue", value);
        SliderHandleImage.color = Color.HSVToRGB(value, 1, 1);
        UpdateColour();
    }

    public void SetSV(float newS, float newV) {
        s = newS;
        v = newV;
        UpdateColour();
    }

    void UpdateColour() {
        lobby.UpdateColour(Color.HSVToRGB(h, s, v));
    }
}
