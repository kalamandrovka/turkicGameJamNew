using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private bool isShaking = false;
    private float shakeTime = 0f;
    private float shakeDuration = 0.0f;
    private float shakePower = 0.0f;


    private Vector2 originalCameraPosition;


    // Start is called before the first frame update
    private void Start()
    {
        originalCameraPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isShaking)
        {
            if (shakeTime > 0)
            {
                transform.localPosition = originalCameraPosition + Random.insideUnitCircle * shakePower;
                shakeTime -= Time.deltaTime;
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalCameraPosition;

            }
        }
    }

    public void StartCameraShake(float duration, float power)
    {
        shakeTime = duration;
        shakePower = power;
        isShaking = true;

    }
}
