using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreablePellet : Pellet , IScoreablePellet
{
    public void Collect()
    {
        ScoreManager.TriggerPelletCountUpdated();
        ScoreManager.PelletCountCompleted();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            Collect();

        base.OnTriggerEnter(collider);
    }
}
