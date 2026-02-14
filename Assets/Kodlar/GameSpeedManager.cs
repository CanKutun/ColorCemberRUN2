using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    public static float SpeedMultiplier = 1f;

    public float startSpeed = 1f;
    public float maxSpeed = 2.5f;
    public float increaseRate = 0.02f;

    void Start()
    {
        SpeedMultiplier = startSpeed;
    }

    void Update()
    {
        SpeedMultiplier += increaseRate * Time.deltaTime;
        SpeedMultiplier = Mathf.Clamp(SpeedMultiplier, startSpeed, maxSpeed);
    }
}