using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    public int playerSpeed;
    
    public string playerCurrentArea ;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    public bool canMove = true;

    public InputActionAsset actions;




    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if( instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        
    }

    void Awake()
    {
        // Retrieve the action asset
        actions = GetComponent<PlayerInput>().actions;
    }

    // Update is called once per frame
    void Update()
    {
        OnInteractButtonUp();
        OnInteractButtonDown();
        OnOpenMenu();
        Movement();
        KeepPlayerInsideBounds();
    }

    void Movement()
    {
        Rigidbody2D player = this.gameObject.GetComponent<Rigidbody2D>();
        Animator playerAnim = this.gameObject.GetComponent<Animator>();
        Vector2 movement = actions.FindAction("Movement").ReadValue<Vector2>();
        float x = movement.x;
        float y = movement.y;
        if (canMove)
        {

            player.velocity = new Vector2(x, y) * playerSpeed;
        }
        else
        {
            player.velocity = Vector2.zero;
        }


        playerAnim.SetFloat("moveX", player.velocity.x);
        playerAnim.SetFloat("moveY", player.velocity.y);

        if(x == 1 || x == -1 || y == 1 || y == -1)
        {
            if(canMove)
            {
                playerAnim.SetFloat("lastMoveX", x);
                playerAnim.SetFloat("lastMoveY", y);
            }
            
        }
      
        
    }
    public bool OnInteractButtonUp ()
    {
        bool interact = actions.FindAction("Interact").WasReleasedThisFrame();
            return interact;
  
    }
    public bool OnInteractButtonDown()
    {
        bool interact = actions.FindAction("Interact").WasPressedThisFrame();
        return interact;

    }

    public bool OnOpenMenu()
    {
        bool menuOpen = actions.FindAction("Menu").WasPerformedThisFrame();
        return menuOpen;
    }

    public void SetBound(Vector3 bottomleft ,Vector3 topRight)
    {
        bottomLeftLimit = bottomleft + new Vector3(0.5f, 1f, 0f);
        topRightLimit = topRight + new Vector3(-0.5f, -1f, 0f);
    }

    private void KeepPlayerInsideBounds()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
                                            Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y),
                                             transform.position.z);
    }
}
