using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Color healthLow;
    public Color healthHigh;
    public Color manaLow;
    public Color manaHigh;
    public Vector3 offset;

    public void setHealth(float health, float maxHealth)
    {
        healthSlider.value = health;
        healthSlider.maxValue = maxHealth;

        healthSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(healthLow, healthHigh, healthSlider.normalizedValue);

    }

    public void setMana(float mana, float maxMana)
    {
        manaSlider.value = mana;
        manaSlider.maxValue = maxMana;

        manaSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(manaLow, manaHigh, healthSlider.normalizedValue);

    }

    // Update is called once per frame
    /*
    void Update()
    {
        healthSlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
    */
}
