using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateRectTransform : MonoBehaviour
{
    public float rotationSpeed = 15f;
    private Vector3 rotationVector;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        rotationVector = Vector3.forward * rotationSpeed * Time.deltaTime;
        rectTransform.Rotate(rotationVector);
    }
}
