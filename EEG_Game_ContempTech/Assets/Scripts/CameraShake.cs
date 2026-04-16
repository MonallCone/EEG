using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private float intensity;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void SetIntensity(float value)
    {
        intensity = Mathf.Clamp01(value);
    }

    void Update()
    {
        if (intensity > 0f)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * intensity;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }
}
