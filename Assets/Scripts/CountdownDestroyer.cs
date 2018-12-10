using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownDestroyer : MonoBehaviour {

    public float destroyTime = 8f;
    float timeAlive = 0f;
	
	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
        if (timeAlive > destroyTime)
        {
            Destroy(this.gameObject);
        }
	}
}
