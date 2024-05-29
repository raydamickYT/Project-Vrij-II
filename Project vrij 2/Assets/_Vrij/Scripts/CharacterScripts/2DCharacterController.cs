using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Simple2DCharacterController : MonoBehaviour
{
    public InputLib inputLib;

    private struct PlayerState
    {
        public Vector2 position;
        public Vector2 velocity;
        public float timestamp;
    }

    private List<PlayerState> stateHistory = new List<PlayerState>();
    private Rigidbody2D rb;
    private bool isRewinding = false;
    public ProgressBarManager progressBarManager;
    public float movementSpeed = 5f;
    public float jumpForce = 700f;
    private bool isGrounded = true;
    [Range(0, 1)]
    public float SuccessGrens = 0.6f;
    [SerializeField]
    private bool IsDebugging = false;
    GameObject[] respawnPoints;
    private float rewindDuration = 5f; // Duur van de terugspoeling in seconden

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
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            RecordState();
        }

        if (isGrounded && !isRewinding)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            Vector2 movement = new Vector2(moveHorizontal * movementSpeed, rb.velocity.y);
            rb.velocity = movement;
        }

        if (Input.GetButtonDown("Jump"))
        {
            ExecuteJump();
        }
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
            StartRewind();
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
                    DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>().text, "InformationText");
                }
                break;
            case "EventTriggerOther":
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(null, "ShowButton"); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.SetSliderMax(inputLib.ConnectedClients);
                progressBarManager.ToggleSlider?.Invoke(); //voor visual feedback laten we ook een progress bar zien met de hoeveelheid mensen die in de lobby zitten
                break;
            case "EventTriggerPerformAction":
                ExecuteJump();
                DelegateManager.Instance.TextEventTriggerDetected?.Invoke(other.GetComponent<Text>().text, "ShowButton"); //we willen dat de players hun input kunnen geven dus laten we in de webclient de knop zien
                progressBarManager.ToggleSlider?.Invoke();
                DelegateManager.Instance.WipeInputListDelegate?.Invoke(); //ff resetten
                break;
            case "Fall":
                StartRewind();
                break;
            default:
                break;
        }
    }

    private void TeleportPlayerToRespawn()
    {
        StartRewind();
    }

    private void RecordState()
    {
        PlayerState state = new PlayerState
        {
            position = rb.position,
            velocity = rb.velocity,
            timestamp = Time.time
        };
        stateHistory.Insert(0, state);

        // Verwijder oudere staten die buiten de rewindDuration vallen
        stateHistory.RemoveAll(s => s.timestamp < Time.time - rewindDuration);
    }

    private void Rewind()
    {
        if (stateHistory.Count > 0)
        {
            PlayerState state = stateHistory[0];
            rb.position = state.position;
            rb.velocity = state.velocity;
            stateHistory.RemoveAt(0);

            if (stateHistory.Count == 0 || stateHistory[0].timestamp <= Time.time - rewindDuration)
            {
                StopRewind();
            }
        }
        else
        {
            StopRewind();
        }
    }

    private void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    private void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
        if (stateHistory.Count > 0)
        {
            PlayerState state = stateHistory[0];
            rb.position = state.position;
            rb.velocity = state.velocity;
        }
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
