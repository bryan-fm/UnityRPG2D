using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Rigidbody2D rb;
    public float moveSpeed;

    public Animator myAnim;

    public static PlayerController instance;

    public string areaTransitionName;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    // Start is called before the first frame update
    void Awake() {
        if(instance == null) {

            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")) * moveSpeed;

        myAnim.SetFloat("moveX", rb.velocity.x);
        myAnim.SetFloat("moveY", rb.velocity.y);


        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || 
            Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1) {
            myAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
            myAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
        }

        //keep the player inside the map
        transform.position = new Vector3(
            Mathf.Clamp(
                transform.position.x,
                bottomLeftLimit.x,
                topRightLimit.x
            ),
            Mathf.Clamp(
                transform.position.y,
                bottomLeftLimit.y,
                topRightLimit.y
            ),
            transform.position.z
        );
        
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight) {

        bottomLeftLimit = botLeft + new Vector3(0.5f, 0.5f, 0f);
        topRightLimit = topRight + new Vector3(-0.5f, -0.5f, 0f);
    }
}
