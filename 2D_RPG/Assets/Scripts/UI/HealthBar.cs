using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Color low;
    public Color High;
    public Vector3 offset;

    public void setHealth(float health, float maxHealth)
    {
        healthSlider.gameObject.SetActive(health < maxHealth);
        healthSlider.value = health;
        healthSlider.maxValue = maxHealth;

        healthSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, High, healthSlider.normalizedValue);

    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
}
