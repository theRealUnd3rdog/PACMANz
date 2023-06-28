using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<Enemy> enemies; // enemies in the list

    [Header("Enemy initials")]
    [Range(1f, 6f)]
    public float gateDuration;

    // Stages for timer

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ScoreManager.OnPelletCountCompleted += StopAllEnemies;
    }

    private void Destroy()
    {
        ScoreManager.OnPelletCountCompleted -= StopAllEnemies;
    }

    public void StopAllEnemies()
    {
        foreach (Enemy enemy in enemies)
            enemy.StopMovement();
    }
}
