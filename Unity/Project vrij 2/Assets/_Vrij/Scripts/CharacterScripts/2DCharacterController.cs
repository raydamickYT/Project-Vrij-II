using System.Collections;
using Unity.VisualScripting;
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
    [HideInInspector]
    public bool IsJumping;
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
        //Debug.Log(IsJumping);
    }

    private void ExecuteJump()
    {
        var rb = GetComponent<Rigidbody2D>();
        int InputSize = inputLib.InputAmount;
        if (IsDebugging)
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX; //unfreeze
            IsJumping = true;

            rb.AddForce(new Vector2(jumpForce, jumpForce * 1.6f));
        }
        else
        {
            if (InputSize > (inputLib.ConnectedClients * SuccessGrens))
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX; //unfreeze
                IsJumping = true;

                rb.AddForce(new Vector2(jumpForce, jumpForce * 1.6f));
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            IsJumping = false;
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
        int Time = 10;
        switch (other.tag)
        {
            case "EventTriggerText":
                if (other.GetComponent<Text>() != null)
                {
                    DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "InformationText", 0);
                }
                break;
            case "EventTriggerOther":
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                if (other.GetComponent<InputTrigger>() != null)
                {
                    Time = other.GetComponent<InputTrigger>().CalculateTimeToEvent(speed);
                    DelegateManager.Instance.StartTimerDelegate?.Invoke(Time);
                }
                progressBarManager.EnableSlider?.Invoke(); //voor visual feedback laten we ook een progress bar zien met de hoeveelheid mensen die in de lobby zitten
                Debug.Log("Time: " + Time + "");
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton", Time); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                Time = 10;
                break;

            case "EventTriggerButterfly":
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                if (other.GetComponent<InputTrigger>() != null)
                {
                    Time = other.GetComponent<InputTrigger>().CalculateTimeToEvent(speed);
                    DelegateManager.Instance.StartTimerDelegate?.Invoke(Time);
                }
                progressBarManager.EnableSlider?.Invoke(); //voor visual feedback laten we ook een progress bar zien met de hoeveelheid mensen die in de lobby zitten
                Debug.Log("Time: " + Time + "");
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButterFly", Time); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                Time = 10; // reset time, incase of failure
                break;
            case "EventTriggerFire":
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                if (other.GetComponent<InputTrigger>() != null)
                {
                    Time = other.GetComponent<InputTrigger>().CalculateTimeToEvent(speed);
                    DelegateManager.Instance.StartTimerDelegate?.Invoke(Time);
                }
                progressBarManager.EnableSlider?.Invoke(); //voor visual feedback laten we ook een progress bar zien met de hoeveelheid mensen die in de lobby zitten
                Debug.Log("Time: " + Time + "");
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowFire", Time); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                Time = 10; // reset time in case of failure
                break;
            case "EventTriggerPerformAction":
                // DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton");
                ExecuteJump();
                // Debug.Log("werkt");
                // DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>(), "ShowButton", 0); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.DisableSlider?.Invoke();
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
