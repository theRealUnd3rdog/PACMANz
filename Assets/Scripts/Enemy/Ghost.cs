using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    private Animator _animator;

    public override void Awake()
    {
        base.Awake();

        _animator = GetComponentInChildren<Animator>();
    }

    public override void Start()
    {
        base.Start();

        if (Trapped)
        {
            StartCoroutine(StartTrapAnimationAtRandom(Trapped));
        }
    }

    private IEnumerator StartTrapAnimationAtRandom(bool trap)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));

        _animator.SetBool("trapped", trap);
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        base.OnTriggerEnter(collider);
    }

    public override void OnTriggerExit(Collider collider)
    {
        base.OnTriggerExit(collider);
    }

    public override void OnTrappedStateChanged(bool newTrappedState)
    {
        base.OnTrappedStateChanged(newTrappedState);

        _animator.SetBool("trapped", newTrappedState);
    }
}
