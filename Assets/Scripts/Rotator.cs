using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum RotateType
    {
        local,
        global,
    }

    public RotateType Type = RotateType.global;

    [Range(-10f, 360f)]
    [SerializeField] private float _rotationSpeed = 15f;

    private void LateUpdate()
    {
        RotateInDeg(_rotationSpeed * Time.deltaTime);
    }

    public void RotateInDeg(float deg)
    {
        if (Type == RotateType.global)
            transform.Rotate(Vector3.up, deg, Space.World);
        else if (Type == RotateType.local)
            transform.Rotate(transform.up, -deg, Space.Self);
    }
}
