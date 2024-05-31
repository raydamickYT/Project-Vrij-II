using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Simple2DCharacterController : MonoBehaviour
{
    public InputLib inputLib;
    public ProgressBarManager progressBarManager;
    public float jumpForce = 700f;
    private bool isGrounded = true;
    [Range(0, 1)]
    public float SuccessGrens = 0.6f;
    [SerializeField]
    private bool IsDebugging = false;
    GameObject[] respawnPoints;
    private Vector3 previousPosition;
    private float speed;

    void Start()
    {
        respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
        previousPosition = transform.position;
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
        }

        CalculateSpeed();
        previousPosition = transform.position; // Update previous position at the end of Update
    }

    private void FixedUpdate()
    {
        progressBarManager.UpdateSliderProgress(inputLib.InputAmount);
    }

    private void ExecuteJump()
    {
            int InputSize = inputLib.InputAmount;
            if (IsDebugging)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
                isGrounded = false;
            }
            else
            {
                if (InputSize > (inputLib.ConnectedClients * SuccessGrens))
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
                    isGrounded = false;
                }
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
                    DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "InformationText");
                }
                break;
            case "EventTriggerOther":
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton"); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                if (other.GetComponent<InputTrigger>() != null)
                {
                    var Time = other.GetComponent<InputTrigger>().CalculateTimeToEvent(speed);
                    DelegateManager.Instance.StartTimerDelegate?.Invoke(Time);
                }
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
            transform.position = closestRespawnPoint.transform.position;
        }
        this.gameObject.GetComponent<Collider2D>().enabled = true;
    }

    GameObject FindClosestRespawnPoint()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
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

    private void CalculateSpeed()
    {
        float distanceTraveled = Vector3.Distance(transform.position, previousPosition);
        float deltaTime = Time.deltaTime;
        speed = distanceTraveled / deltaTime;
    }
}
