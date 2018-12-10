using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

    //Pixels per second dropping speed
    public float velocity = 2f;
    float pos;

    public LayerMask isObject;

    [Tooltip("Possible types are: 'Light', 'Normal', 'Heavy', 'Scaffold'")]
    public string type;

    [HideInInspector]
    public bool grounded = false;
    [HideInInspector]
    public bool moving = false;

    // Use this for initialization
    void Start () {
        pos = transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - 0.499f, transform.position.y - 0.525f), Vector2.right, 0.998f, isObject);

        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player")
            {
                hit.collider.GetComponent<PlayerController>().isDead = true;
            }
            else
            {
                grounded = true;
                velocity = 2;
            }
        }
        else
        {
            grounded = false;

            float newPos = pos - (velocity * Time.fixedDeltaTime);
            
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(new Vector2(transform.position.x, pos -0.5f), Vector2.down, pos - newPos, isObject);

            foreach (RaycastHit2D downHit in hitArray)
            {
                if (downHit.collider != null && downHit.collider.gameObject != this.gameObject)
                {
                    newPos = pos - downHit.distance + 0.001f;
                    break;
                }
            }
            
            pos = newPos;

            transform.position = new Vector3(transform.position.x, PixelRounder.RoundByPixels(pos, 32), 0);
        }
    }

    public IEnumerator MoveToPosition(bool moveRight, float timeToMove)
    {
        moving = true;
        Vector3 position = transform.position;
        var currentPos = transform.position;
        if (moveRight)
        {
            position.x += 1;
        }
        else
        {
            position.x -= 1;
        }
        var t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            float roundedTime = PixelRounder.RoundByPixels(t, 32);
            transform.position = Vector3.Lerp(currentPos, position, roundedTime);
            yield return null;
        }

        transform.position = position;
        moving = false;
    }
}
