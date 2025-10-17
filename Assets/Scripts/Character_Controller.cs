using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class Character_Controller : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;



    // Update is called once per frame

    public void Move(float horizontal, float vertical, float speed)
    {
        rb.linearVelocity = new UnityEngine.Vector2(horizontal * speed, vertical * speed);
    }

    
    
}
