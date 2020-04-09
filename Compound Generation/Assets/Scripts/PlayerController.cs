using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    public float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    }
    public void Move(float x, float y){
		rb.velocity = new Vector2(x, y) * speed;
	}
}
