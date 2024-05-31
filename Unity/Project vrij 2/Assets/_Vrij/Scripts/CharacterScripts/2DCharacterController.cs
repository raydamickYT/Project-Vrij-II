using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Simple2DCharacterController : MonoBehaviour
{
    public InputLib inputLib;
    public ProgressBarManager progressBarManager;
    public float movementSpeed = 5f;
    public float jumpForce = 700f;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    [Range(0, 1)]
    public float SuccessGrens = 0.6f;
    [SerializeField]
    private bool IsDebugging = false;
    GameObject[] respawnPoints;


    void Start()
    {
        respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        DelegateManager.Instance.ExecuteJumpDelegate += ExecuteJump;
    }

    void Update()
    {
        if (isGrounded)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            // Vector2 movement = new Vector2(1 * movementSpeed, rb.velocity.y);
            // rb.velocity = movement;
        }

        // if (Input.GetButtonDown("Jump"))
        // {
        //     ExecuteJump();
        // }
    }
    private void FixedUpdate()
    {
        progressBarManager.UpdateSliderProgress(inputLib.InputAmount);
    }

    private void ExecuteJump()
    {
        if (isGrounded)
        {
            int InputSize = inputLib.InputAmount;
            if (IsDebugging)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                isGrounded = false;
            }
            else
            {
                if (InputSize > (inputLib.ConnectedClients * SuccessGrens))
                {
                    rb.AddForce(new Vector2(0f, jumpForce));
                    isGrounded = false;
                }
            }
            //else doe niks
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.collider.CompareTag("Fall"))
        {
            // StartCoroutine(PullPlayerUp());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "EventTriggerText":
                if (other.GetComponent<Text>() != null)
                {
                    Debug.Log("trigger");
                    DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "InformationText");
                }
                break;
            case "EventTriggerOther":
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton"); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                progressBarManager.ToggleSlider?.Invoke(); //voor visual feedback laten we ook een progress bar zien met de hoeveelheid mensen die in de lobby zitten
                break;
            case "EventTriggerPerformAction":
                // DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton");
                ExecuteJump();
                Debug.Log("werkt");
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton"); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.ToggleSlider?.Invoke();
                DelegateManager.Instance.WipeInputListDelegate?.Invoke(); //ff resetten
                break;
            case "Fall":
                TeleportPlayerToRespawn();
                break;
            default:
                break;
        }
    }


    private void TeleportPlayerToRespawn()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject closestRespawnPoint = FindClosestRespawnPoint();
        if (closestRespawnPoint != null)
        {
            // Teleporteer de speler direct naar het dichtstbijzijnde respawnpunt
            rb.position = closestRespawnPoint.transform.position;
        }
        this.gameObject.GetComponent<Collider2D>().enabled = true;
    }

    GameObject FindClosestRespawnPoint()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = rb.position;
        foreach (GameObject respawnPoint in respawnPoints)
        {
            float distance = Vector3.Distance(respawnPoint.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closest = respawnPoint;
                closestDistance = distance;
            }
        }
        return closest;
    }
}
