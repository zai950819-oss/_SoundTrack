using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class Character_Controller : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    public UnityEngine.Vector3 rotationSpeed = new UnityEngine.Vector3(0, 0, 90); // Rotate 50 degrees per second around Y-axis


    // Update is called once per frame

    public void Move(float horizontal, float vertical, float speed)
    {
        rb.linearVelocity = new UnityEngine.Vector2(horizontal * speed, vertical * speed);
    }

    public void rotate()
    {
        transform.Rotate(rotationSpeed);
    }
}
