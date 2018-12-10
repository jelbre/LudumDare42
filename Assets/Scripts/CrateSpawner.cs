using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour {

    public int gridWidth = 9;
    public List<GameObject> crates;
    public Transform crateParent;

    int level = 1;
    public List<DropPattern> patterns;
    DropPattern currentPattern = new DropPattern();

    [Tooltip("Adding a negative value here will add extra time before the very first crate drop")]
    public float currentTime = 0f;

    private void Start()
    {
        SelectPattern();
    }

    // Update is called once per frame
    void Update () {
        currentTime += Time.deltaTime;
        if (currentTime > currentPattern.waitTime)
        {
            SpawnCrates();
            currentTime -= currentPattern.waitTime;
            currentPattern.patternLenght--;
            if (currentPattern.patternLenght <= 0)
            {
                level++;
                SelectPattern();
            }
        }
	}

    public void SelectPattern()
    {
        List<DropPattern> availablePatterns = new List<DropPattern>();
        foreach (DropPattern pattern in patterns)
        {
            if (pattern.minLevel <= level && pattern.maxLevel >= level)
            {
                availablePatterns.Add(pattern);
            }
        }

        DropPattern clonePattern;
        if (availablePatterns.Count > 0)
        {
            clonePattern = availablePatterns[Random.Range(0, availablePatterns.Count)];
        }
        else
        {
            clonePattern = availablePatterns[0];
        }
        
        currentPattern.maxCrateAmount = clonePattern.maxCrateAmount;
        currentPattern.maxLevel = clonePattern.maxLevel;
        currentPattern.minCrateAmount = clonePattern.minCrateAmount;
        currentPattern.minLevel = clonePattern.minLevel;
        currentPattern.patternLenght = clonePattern.patternLenght;
        currentPattern.velocity = clonePattern.velocity;
        currentPattern.waitTime = clonePattern.waitTime;
        currentPattern.CheckCrateAmountVariation();
    }

    public void SpawnCrates()
    {
        int crateAmount;
        if (currentPattern.crateAmountVatiation)
        {
            crateAmount = Random.Range(currentPattern.minCrateAmount, currentPattern.maxCrateAmount);
        }
        else
        {
            crateAmount = currentPattern.minCrateAmount;
        }

        int availableRows = gridWidth;
        List<int> rows = new List<int>();
        for (int i = 0; i < crateAmount; i++)
        {
            int randomInt = Random.Range(1, availableRows);
            foreach (int r in rows)
            {
                if (randomInt >= r)
                {
                    randomInt++;
                }
            }
            rows.Add(randomInt);
            rows.Sort();
            availableRows--;
        }

        foreach (int row in rows)
        {
            GameObject cratePrefab = crates[Random.Range(0, crates.Count)];
            Vector3 position = new Vector3(transform.position.x + row, transform.position.y, transform.position.z);

            GameObject spawnedCrate = Instantiate(cratePrefab, position, Quaternion.identity, crateParent);
            Crate crate = spawnedCrate.GetComponent<Crate>();
            crate.velocity = currentPattern.velocity;
        }
    }
}
