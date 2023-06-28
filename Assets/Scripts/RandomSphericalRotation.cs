using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSphericalRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1f;
    private Quaternion _targetRotation;

    private void Start()
    {
        GenerateRandomRotation();
    }

    private void LateUpdate()
    {
        RotateTowardsTarget();
    }

    private void GenerateRandomRotation()
    {
        Vector3 randomAxis = Random.onUnitSphere;
        _targetRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), randomAxis);
    }

    private void RotateTowardsTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f)
        {
            GenerateRandomRotation();
        }
    }
}
