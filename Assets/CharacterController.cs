using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;


    // Update is called once per frame

    public void Move(float horizontal,float vertical, float speed)
    {
        rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);
    }
}
