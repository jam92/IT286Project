﻿using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public int health = 6;
    public float recoveryTime = 0.8f;
    public float lastDmg;
    bool recovering = false;


    public float moveSpeed = 2.0f;
    public bool hasControl = true;
    public bool invincible = false;
    public bool spinning = false;
    public LayerMask RayCastIgnore;
    public GameObject WeaponController;

    public GameObject CurrentRoom;
    public Sprite n;
    public Sprite e;
    public Sprite s;
    public string orientation;

    // Use this for initialization
    void Start()
    {
        lastDmg = Time.time - recoveryTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.singleton.CurrentGameState != GameManager.GameState.Play) return;
        if (hasControl)
        {
            //Rotate Weapon Hitbox
            if (WeaponController.GetComponent<weaponHandler>().weaponState == "idle")
            {
                if ((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0))
                    WeaponController.transform.rotation = Quaternion.LookRotation(transform.forward, (Camera.main.transform.right * Input.GetAxisRaw("Horizontal") + Camera.main.transform.up * Input.GetAxisRaw("Vertical")));
            }
            if ((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0))
            {
                move(moveSpeed, Camera.main.transform.right * Input.GetAxisRaw("Horizontal") + Camera.main.transform.up * Input.GetAxisRaw("Vertical"));
            }


            //Change Sprites
            if (Input.GetKey(KeyCode.UpArrow))
            {
                GetComponent<SpriteRenderer>().sprite = n;
                GetComponent<SpriteRenderer>().flipX = false;
                orientation = "n";
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                GetComponent<SpriteRenderer>().sprite = s;
                GetComponent<SpriteRenderer>().flipX = false;
                orientation = "s";
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                GetComponent<SpriteRenderer>().sprite = e;
                GetComponent<SpriteRenderer>().flipX = false;
                orientation = "w";
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                GetComponent<SpriteRenderer>().sprite = e;
                GetComponent<SpriteRenderer>().flipX = true;
                orientation = "e";
            }

        }
        if ((spinning) || (!hasControl)) {
            //If we can't go off of player input, make some assumptions
            if (Mathf.Abs(WeaponController.transform.up.y) <= Mathf.Abs(WeaponController.transform.up.x))
            {
                if (WeaponController.transform.up.x > 0)
                {
                    //right 
                    GetComponent<SpriteRenderer>().sprite = e;
                    GetComponent<SpriteRenderer>().flipX = true;
                    orientation = "e";
                }
                else {
                    //left
                    GetComponent<SpriteRenderer>().sprite = e;
                    GetComponent<SpriteRenderer>().flipX = false;
                    orientation = "w";
                }
            }
            else {
                if (WeaponController.transform.up.y > 0)
                {
                    //up
                    GetComponent<SpriteRenderer>().sprite = n;
                    GetComponent<SpriteRenderer>().flipX = false;
                    orientation = "n";
                }
                else {
                    //down
                    GetComponent<SpriteRenderer>().sprite = s;
                    GetComponent<SpriteRenderer>().flipX = false;
                    orientation = "s";
                }
            }
        }


        if (Time.time - lastDmg > recoveryTime)
        {
            recovering = false;
        }
        //Else.... recovery behaviour?
    }

    public void move(float speed, Vector3 direction)
    {
        Vector3 input = direction * Time.deltaTime * speed;
        //The "easy" way
        //transform.Translate(input);
        
        //The "I forgot that function's a thing" way
        RaycastHit2D move;
        RaycastHit2D move2;
        
        move2 = Physics2D.Raycast(transform.position, new Vector2(input.x, 0), GetComponent<BoxCollider2D>().size.x * transform.lossyScale.x / 2, ~RayCastIgnore);
        if (input.x < 0)
        {
            move = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 360, new Vector2(input.x, 0), input.x, ~RayCastIgnore);
        }
        else
        {
            move = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 360, new Vector2(input.x, 0), input.x, ~RayCastIgnore);
        }
        if (!move2)
        {
            if (!move) transform.position = transform.position + new Vector3(input.x, 0, 0);
            else
            {
                Debug.Log("Weird");
                transform.Translate(input);
            }
        }
        else
        {
            transform.position = transform.position + new Vector3(move.centroid.x, move.centroid.y, 0);
        }


        move2 = Physics2D.Raycast(transform.position, new Vector2(0, input.y), GetComponent<BoxCollider2D>().size.y * transform.lossyScale.y / 2, ~RayCastIgnore);
        if (input.y < 0)
        {
            move = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 360, new Vector2(0, input.y), input.y, ~RayCastIgnore);
        }
        else
        {
            move = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 360, new Vector2(0, input.y), input.y, ~RayCastIgnore);
        }
        if (!move2)
        {
            if (!move) transform.position = transform.position + new Vector3(0, input.y, 0);
            else
            {
                Debug.Log("Weird");
                transform.Translate(input);
            }
        }
        else {
            transform.position = transform.position + new Vector3(move.centroid.x, move.centroid.y, 0);
        }
        
    }

    public void DamagePlayer() {
        if ((!invincible) && (!recovering)) {
            health--;
            if (health <= 0)
            {
                //Death state stuff goes here
            }
            else {
                lastDmg = Time.time;
                recovering = true;
                //Update health in the UI
            }
        }
    }

}
