using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [HideInInspector]
    public int maxHeight = 0;

    public LayerMask groundSurfaces;
    public LayerMask hittableObjects;
    //The way the player is facing
    bool facingRight = true;
    //The max distance the player can push a crate from
    public float pushDistance = 1f;
    //The amount of light crates the player is able to push (not including first crate)
    public int pushAmount;
    //Amount of time player has to wait before he can kick/push again
    public float pushRecovery;
    [HideInInspector]
    public float currentPushRecovery;

    [HideInInspector]
    public bool isDead = false;

    [Header("Vertical movement variables")]

    //Is the object on the ground
    bool grounded = false;
    //Correct vertical position, gets rounded to move pixel by pixel and then inserted into the transform
    public float verticalPos;
    //Current vertical movement speed
    public float verticalVelocity;
    //Initial upward velocity of the player
    public float jumpForce;
    //Strenght of the gravity acting upon the player
    public float mass;

    [Header("Horizontal movement variables")]

    //Correct horizontal position, gets rounded to move pixel by pixel and then inserted into the transform
    float horizontalPos;
    //Current horizontal movement speed
    float horizontalVelocity;
    //Max horizontal movement speed
    public float horizontalMaxVelocity;
    //Horizontal acceleration speed
    public float horizontalAcceleration;
    //Horizontal breaking speed (the same in air as on ground for now)
    public float horizontalBreakSpeed;
    //Multiplier for faster breaking when walking in the opposite direction of the velocity
    public float turnMultiplier;
    //Horizontal speed at which the player will completely come to a stop
    public float horizontalStopSpeed;

    [Header("Animation clips")]
    public Sprite standingSprite;
    public Sprite walkingSprite;
    public Sprite jumpingSprite;
    public SpriteRenderer spriteRenderer;

    string currentSprite = "standing";

    private List<Crate> currentPushingCrates;

	// Use this for initialization
	void Start () {
        currentPushRecovery = 0;
        horizontalPos = transform.position.x;
        verticalPos = transform.position.y;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        currentPushRecovery += Time.fixedDeltaTime;
        if (currentPushRecovery > pushRecovery)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                HitCrate(false);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                HitCrate(true);
            }
        }

        #region Check feet collision and jumping
        //Check if the player is standing on the ground
        RaycastHit2D feetHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.374f, transform.position.y + -0.751f), Vector2.right, 0.748f, groundSurfaces);
        if (feetHit.collider != null && feetHit.collider.tag == "Ground")
        {
            grounded = true;
            verticalVelocity = 0;

            int height = (int)Mathf.Round(transform.position.y);
            if (height > maxHeight)
            {
                maxHeight = height;
            }
        }
        else
        {
            grounded = false;
        }

        //Check if user pressed the space bar and is on the ground
        if (grounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= 9.80665f * mass * Time.fixedDeltaTime;
        }
        #endregion

        #region horizontal movement
        //Check if user is pressing A or D
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (horizontalMovement != 0)
        {
            if (horizontalMovement == 1)
            {
                if (!facingRight)
                {
                    facingRight = true;
                    Flip();
                }
            }
            else
            {
                if (facingRight)
                {
                    facingRight = false;
                    Flip();
                }
            }

            float newVelocity = horizontalMovement * horizontalAcceleration * Time.fixedDeltaTime;
            if (horizontalVelocity < 0 && horizontalMovement == 1 || horizontalVelocity > 0 && horizontalMovement == -1)
            {
                newVelocity *= turnMultiplier;
            }
            //Add velocity
            horizontalVelocity += newVelocity;

            //If velocity is bigger than max velocity, limit it and set it to the max
            if (horizontalVelocity > horizontalMaxVelocity)
            {
                horizontalVelocity = horizontalMaxVelocity;
            }
            if (horizontalVelocity < -horizontalMaxVelocity)
            {
                horizontalVelocity = -horizontalMaxVelocity;
            }
        }
        else
        {
            //Stop moving if velocity is within stop speed
            if (horizontalVelocity < horizontalStopSpeed && horizontalVelocity > -horizontalStopSpeed)
            {
                horizontalVelocity = 0;
            }
            //Slow down player in positive or negative direction
            else if (horizontalVelocity < 0)
            {
                horizontalVelocity += horizontalBreakSpeed * Time.fixedDeltaTime;
            }
            else
            {
                horizontalVelocity -= horizontalBreakSpeed * Time.fixedDeltaTime;
            }
        }

        //Check if the sides of the player are clear
        bool sideClear = true;
        if (horizontalVelocity < 0)
        {
            sideClear = CheckSide(true);
        }
        else if (horizontalVelocity > 0)
        {
            sideClear = CheckSide(false);
        }

        if (!sideClear)
        {
            horizontalVelocity = 0;
        }
        #endregion

        //Update real vertical and horizontal position (raycasts check if the ground has been clipped and make sure the player doesn't fall/walk through
        #region verticalClippingCheck
        float newVerticalPos = verticalPos + (verticalVelocity * Time.fixedDeltaTime);

        if (verticalVelocity < 0)
        {
            RaycastHit2D leftDownHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.374f, verticalPos + -0.75f), Vector2.down, verticalPos - newVerticalPos, groundSurfaces);
            RaycastHit2D rightDownHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.374f, verticalPos + -0.75f), Vector2.down, verticalPos - newVerticalPos, groundSurfaces);

            if (leftDownHit.collider != null)
            {
                newVerticalPos = verticalPos - leftDownHit.distance + 0.001f;
            }
            else if (rightDownHit.collider != null)
            {
                newVerticalPos = verticalPos - rightDownHit.distance + 0.001f;
            }
        }
        #endregion

        #region horizontalClippingCheck
        float newHorizontalPos = horizontalPos + (horizontalVelocity * Time.fixedDeltaTime);

        float xOffset = 0.375f;
        Vector2 direction = Vector2.right;
        float distance = newHorizontalPos - horizontalPos;

        if (horizontalVelocity < 0)
        {
            xOffset = -xOffset;
            direction = Vector2.left;
            distance = horizontalPos - newHorizontalPos;
        }

        RaycastHit2D UpHit = Physics2D.Raycast(new Vector2(transform.position.x + xOffset, verticalPos + 0.749f), direction, distance, groundSurfaces);
        RaycastHit2D MiddleHit = Physics2D.Raycast(new Vector2(transform.position.x + xOffset, verticalPos), direction, distance, groundSurfaces);
        RaycastHit2D DownHit = Physics2D.Raycast(new Vector2(transform.position.x + xOffset, verticalPos - 0.749f), direction, distance, groundSurfaces);

        if (UpHit.collider != null)
        {
            newHorizontalPos = horizontalPos - UpHit.distance;
            horizontalVelocity = 0;
        }
        else if (MiddleHit.collider != null)
        {
            newHorizontalPos = horizontalPos - MiddleHit.distance;
            horizontalVelocity = 0;
        }
        else if (DownHit.collider != null)
        {
            newHorizontalPos = horizontalPos - DownHit.distance;
            horizontalVelocity = 0;
        }
        #endregion

        //Update real positions
        horizontalPos = newHorizontalPos;
        verticalPos = newVerticalPos;

        if (!grounded)
        {
            if (currentSprite != "jumping")
            {
                spriteRenderer.sprite = jumpingSprite;
                currentSprite = "jumping";
            }
        }
        else if (horizontalVelocity > horizontalStopSpeed || horizontalVelocity < -horizontalStopSpeed)
        {
            if (currentSprite != "walking")
            {
                spriteRenderer.sprite = walkingSprite;
                currentSprite = "walking";
            }
        }
        else if (currentSprite != "standing")
        {
            spriteRenderer.sprite = standingSprite;
            currentSprite = "standing";
        }

        //Round down position and apply to the transform
        transform.position = new Vector3(PixelRounder.RoundByPixels(horizontalPos, 32), PixelRounder.RoundByPixels(verticalPos, 32));

    }

    bool CheckSide(bool leftSide)
    {
        float sideOffset = 0.376f;

        if (leftSide)
        {
            sideOffset = -sideOffset;
        }

        RaycastHit2D sideHit = Physics2D.Raycast(new Vector2(transform.position.x + sideOffset, transform.position.y + -0.749f), Vector2.up, 1.498f, groundSurfaces);

        if (sideHit.collider != null && sideHit.collider.tag == "Ground")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void HitCrate(bool isKick)
    {
        currentPushRecovery = 0;

        float height = 0.4f;
        if (isKick)
        {
            height = -0.5f;
        }

        Vector2 direction = Vector2.left;
        if (facingRight)
        {
            direction = Vector2.right;
        }

        RaycastHit2D firstHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + height), direction, pushDistance, hittableObjects);

        if (firstHit.collider != null)
        {
            Transform hitTransform = firstHit.transform;
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(hitTransform.position.x, hitTransform.position.y + 0.499f), direction, 20f, groundSurfaces);

            Array.Sort(hits, delegate (RaycastHit2D x, RaycastHit2D y) { return x.distance.CompareTo(y.distance); });

            //Crates that are being affected by the push
            List<GameObject> pushingCrates = new List<GameObject>();

            for (int i = 0; i < hits.Length; i++)
            {
                if (i == 0)
                {
                    pushingCrates.Add(hits[i].collider.gameObject);
                }
                else if (hits[i].distance - hits[i - 1].distance <= 1)
                {
                    pushingCrates.Add(hits[i].collider.gameObject);
                }
                else
                { 
                    break;
                }
            }

            currentPushingCrates = new List<Crate>();

            if (CheckIfPushable(pushingCrates))
            {
                foreach (Crate crate in currentPushingCrates)
                {
                    StartCoroutine(crate.MoveToPosition(facingRight, 0.5f));
                }
            }
        }
    }

    bool CheckIfPushable(List<GameObject> crates)
    {
        bool pushable = true;
        int pushableCrates = pushAmount;
        for (int i = 0; i < crates.Count; i++)
        {
            if (crates[i].layer == 10)
            {
                Crate c = crates[i].GetComponent<Crate>();
                currentPushingCrates.Add(c);
                if (c.type == "Heavy" || i > 0 && c.type == "Normal" || !c.grounded)
                {
                    pushable = false;
                    break;
                }
                else if (i > 0)
                {
                    if (pushableCrates > 0)
                    {
                        pushableCrates--;
                    }
                    else
                    {
                        pushable = false;
                        break;
                    }
                }
            }
            else
            {
                pushable = false;
                break;
            }
        }
        return pushable;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }
}
