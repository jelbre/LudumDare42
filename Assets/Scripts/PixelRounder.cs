using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PixelRounder {
    
    /// <summary>
    /// Rounds floats down to the amount of pixels per unit
    /// </summary>
    /// <param name="numberToRound">the number that will be rounded down and returned</param>
    /// <param name="pixelAmount">amount of pixels per unit</param>
    /// <returns></returns>
    public static float RoundByPixels(float numberToRound, float pixelAmount)
    {
        return Mathf.Round(numberToRound * pixelAmount) / pixelAmount;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
