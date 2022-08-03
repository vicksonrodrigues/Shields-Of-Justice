using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{


    public Transform target;
    
    public Tilemap theMap;

    public bool smallRoom;
    public GameObject Locktarget;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;


    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight*Camera.main.aspect;

        bottomLeftLimit = theMap.localBounds.min + new Vector3 (halfWidth,halfHeight, 0f);
        topRightLimit = theMap.localBounds.max + new Vector3 (-halfWidth,-halfHeight,0f);

        GetBoundsValueForPlayer();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(smallRoom)
        {
            transform.position = Locktarget.transform.position;
            return;
        }
        FollowPlayer();
        KeepCameraInsideBound();

        if (!musicStarted)
        {
            musicStarted = true;
            AudioManager.instance.PlayBGM(musicToPlay);
        }
    }

    private void FollowPlayer()
    {
        if(target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
        
    }

    private void KeepCameraInsideBound()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
                                            Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y),
                                             transform.position.z);
    }

    private void GetBoundsValueForPlayer()
    {
        PlayerController.instance.SetBound(theMap.localBounds.min, theMap.localBounds.max);
    }
}
