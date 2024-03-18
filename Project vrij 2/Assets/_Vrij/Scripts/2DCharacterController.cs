using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Simple2DCharacterController : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private RandomJump randomJump;
    public float movementSpeed = 5f;
    public float jumpForce = 700f;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    void Start()
    {
        randomJump = new RandomJump(_slider);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal * movementSpeed, rb.velocity.y);
        rb.velocity = movement;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            bool canJump = randomJump.RandJumpVoid(); // Check of de random jump ons toestaat om te springen.
            if (canJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
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
            StartCoroutine(PullPlayerUp());
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

    }

    private IEnumerator PullPlayerUp()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        GameObject closestRespawnPoint = FindClosestRespawnPoint();
        if (closestRespawnPoint != null)
        {
            while (Vector3.Distance(rb.position, closestRespawnPoint.transform.position) > 0.1f)
            {
                rb.position = Vector3.MoveTowards(rb.position, closestRespawnPoint.transform.position, movementSpeed * Time.deltaTime);
                yield return null; // Wacht tot de volgende frame
            }
        }
        this.gameObject.GetComponent<Collider2D>().enabled = true;
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    GameObject FindClosestRespawnPoint()
    {
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
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
