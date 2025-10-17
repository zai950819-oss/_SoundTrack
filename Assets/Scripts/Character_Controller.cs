using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.Collections;


public class Character_Controller : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite _moving_sprite;
    private float distance = 1f;    
    private UnityEngine.Vector2 direction;    
    [SerializeField] private Rigidbody2D rb;

    //Todo : make the Move() function work -> move for block distance and use the trigger event to move
    //lanenum {0 : W, 1 : S ....}
    public void Move(int lanenum)
    {
        switch (lanenum)
        {
            case 0:
                direction = new UnityEngine.Vector2(0, distance);
                break;
            case 1:
                direction = new UnityEngine.Vector2(0, -1 * distance);
                break;
            case 2:
                direction = new UnityEngine.Vector2(-1 * distance, 0);
                break;
            case 3:
                direction = new UnityEngine.Vector2(distance, 0);
                break;
        }
        UnityEngine.Vector2 target = rb.position + direction;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 1f, LayerMask.GetMask("Walls"));
        if (hit.collider == null)
            rb.MovePosition(target);

    }
    
    IEnumerator Wait_08f()
    {
        yield return new WaitForSeconds(0.8f); // wait 2 seconds
    }
    public void moving_animation()
    {
        Sprite originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = _moving_sprite;
        StartCoroutine(Wait_08f());
        spriteRenderer.sprite = originalSprite;
    }

    
    
}
