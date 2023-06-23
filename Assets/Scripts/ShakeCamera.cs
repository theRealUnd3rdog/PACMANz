using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ShakeCamera : MonoBehaviour
{
    [Range(0f, 12f)]
    [SerializeField] private float _magnitude;

    [Range(0f, 12f)]
    [SerializeField] private float _roughness;

    [Range(0f, 12f)]
    [SerializeField] private float _fadeInTime;

    private void Start()
    {
        CameraShaker.Instance.StartShake(_magnitude, _roughness, _fadeInTime);
    }
}
