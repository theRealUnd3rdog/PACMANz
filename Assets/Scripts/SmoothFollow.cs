using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public enum FollowType
    {
        ParentFollow,
        ParentRotation,
        ParentRotAndLoc,
    }
    public FollowType Type;

    [Header("Variables")]
    public float SmoothSpeed = 15f;
    private Quaternion LocalRot;
    private Quaternion CurRot;
    private Vector3 CurLoc;

    void Start()
    {
        LocalRot = transform.localRotation;
        CurRot = transform.rotation;
        CurLoc = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Type == FollowType.ParentFollow) ParentLocation();
        if (Type == FollowType.ParentRotation) ParentRotation();
        if (Type == FollowType.ParentRotAndLoc)
        {
            ParentLocation();
            ParentRotation();
        }
    }

    private void ParentLocation()
    {
        Vector3 targetLoc = transform.parent.position;

        CurLoc = Vector3.Lerp(CurLoc, targetLoc, SmoothSpeed * Time.deltaTime);
        transform.position = CurLoc;
    }

    private void ParentRotation()
    {
        Quaternion targetRot = transform.parent.rotation;

        CurRot = Quaternion.Slerp(CurRot, targetRot, SmoothSpeed * Time.deltaTime);
        transform.rotation = CurRot;
    }
}
