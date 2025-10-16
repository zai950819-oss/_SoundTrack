using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private Character_Controller controller;
    private float horizontal;
    private float vertical;
    public float speed = 8f;
    private static Player instance;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        controller.Move(horizontal, vertical, speed);

    }

    public void freezecharacter()
    {
        controller.enabled = false;
        Debug.Log("Freezed character");
    }
    
    public void unfreezecharacter()
    {
        controller.enabled = true;
        Debug.Log("Unfreezed character");

    }
}