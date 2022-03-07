using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public GameObject subject;

    private void OnEnable()
    {
        subject.SetActive(true);
    }
}
