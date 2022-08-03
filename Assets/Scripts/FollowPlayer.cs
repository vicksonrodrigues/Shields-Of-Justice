using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowPlayer : MonoBehaviour
{   
    // Work Pending
    // Might use A* Algorithm for following the player
    private Transform player;
    Rigidbody2D rb;
    public float speed;
    
    public static FollowPlayer instance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
       
    }



    // Update is called once per frame
    void Update()
    {
       
        Movement();
        
    }
    void Movement()
    {
        
        Animator followerAnim = GetComponent<Animator>();
        Vector2 movement = PlayerController.instance.actions.FindAction("Movement").ReadValue<Vector2>();
        float x = movement.x;
        float y = movement.y;

        if (PlayerController.instance.canMove)
        {

            if (Vector2.Distance(rb.position, player.position) > 2)
            {

                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * Mathf.Abs(x), direction.y * Mathf.Abs(y)) * speed;
                Debug.Log(rb.velocity);

                followerAnim.SetFloat("moveX", rb.velocity.x);
                followerAnim.SetFloat("moveY", rb.velocity.y);

            }
            else
            {
                rb.velocity = Vector2.zero;
            }
            
        }

        else
        {
            rb.velocity = Vector2.zero;
        }


        

        if (x == 1 || x == -1 || y == 1 || y == -1)
        {
            if (PlayerController.instance.canMove)
            {
                followerAnim.SetFloat("IdleX", x);
                followerAnim.SetFloat("IdleY", y);
            }

        }


    }
}
