using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [SerializeField] private float _rotationRate;
    private float _rotation;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", _rotation);

        if (_rotation >= 360f) _rotation = 0f;
        else _rotation += Time.deltaTime * _rotationRate;
    }
}
