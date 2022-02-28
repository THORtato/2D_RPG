using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider slider;
    public Color low;
    public Color High;
    public Vector3 offset;

    public void setMana(float mana, float maxMana)
    {
        slider.gameObject.SetActive(mana < maxMana);
        slider.value = mana;
        slider.maxValue = maxMana;

        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, High, slider.normalizedValue);

    }

    // Update is called once per frame
    void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
}
