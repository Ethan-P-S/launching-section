using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class scoring
{
    public static int score { get; private set; } = 0;
    public static int shots { get; private set; } = 0;

    public static int shotMultiplier { get; private set; } = 1;
    public static float consecutiveMultiplier { get; private set; } = 1;

    public static bool somethingWasHit = false;

    public static void ResetScore()
    {
        shots = 0;
        score = 0;
        shotMultiplier = 1;
    }

    public static void ShotTaken()
    {
        shots++;
        TargetsManager.instance.UpdateDisplay();
    }

    public static void Score(int amount)
    {
        score += Mathf.RoundToInt((float)amount * ComboMultiplier());
        TargetsManager.instance.UpdateDisplay();
    }

    public static void AddShotMultiplier()
    {
        shotMultiplier++;
    }
    public static void ResetShotMultiplier()
    {
        shotMultiplier = 1;
    }

    public static void AddConMultiplier()
    {
        consecutiveMultiplier+= 0.2f;
        TargetsManager.instance.UpdateCombos();
    }
    public static void ResetConMultiplier()
    {
        consecutiveMultiplier = 1;
    }

    public static void RESET_MULT()
    {
        ResetShotMultiplier();
        ResetConMultiplier();
        TargetsManager.instance.UpdateCombos();
    }

    public static float ComboMultiplier()
    {
        float foo = (float)shotMultiplier * consecutiveMultiplier;

        if(foo >= 100f)
        {
            foo = 99.9f;
        }
        return foo;
    }
}
