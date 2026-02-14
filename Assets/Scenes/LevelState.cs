using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static int level = 1;

    public static void LevelAtla()
    {
        level++;
        Debug.Log("Yeni level: " + level);
    }
}