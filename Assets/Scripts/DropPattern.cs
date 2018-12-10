using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropPattern {

    //This drop pattern will be used between these levels. Harder levels mean harder drop patterns!
    public int minLevel;
    public int maxLevel;

    public float waitTime;
    public float velocity;

    [HideInInspector]
    public bool crateAmountVatiation = false;
    public int minCrateAmount;
    public int maxCrateAmount;

    public int patternLenght;

	// Use this for initialization
	void Start () {
        CheckCrateAmountVariation();
	}

    public void CheckCrateAmountVariation()
    {
        if (maxCrateAmount > minCrateAmount)
        {
            crateAmountVatiation = true;
        }
        else
        {
            crateAmountVatiation = false;
        }
    }
}
