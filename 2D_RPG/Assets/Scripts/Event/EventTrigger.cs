using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public GameObject subject;
    public GameObject canvas;


    private void OnEnable()
    {
        subject.SetActive(true);
        Destroy(canvas, 13.5f);
    }
}
