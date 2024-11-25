using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private Rigidbody rig;
    private float startTime;
    private float timeTaken;
    private int collectablesPicked;
    public int maxCollectables = 10;
    private bool isPlaying;
    public GameObject playButton;
    public TextMeshProUGUI curTimeText;
    public LayerMask waterLayer;
    public float distanceToGround = 0.1f;
    private CapsuleCollider _col;
    public Camera playerCamera;

    void OnTriggerEnter (Collider other)
    {
        if(other.gameObject.CompareTag("Collectable"))
        {
            collectablesPicked++;
            Destroy(other.gameObject);
            if(collectablesPicked == maxCollectables)
                End();
        }
    }

    public void Begin ()
    {
        startTime = Time.time;
        isPlaying = true;
        playButton.SetActive(false);
    }

    void End ()
    {
        timeTaken = Time.time - startTime;
        isPlaying = false;
        playButton.SetActive(true);
        Leaderboard.instance.SetLeaderboardEntry(-Mathf.RoundToInt(timeTaken * 1000.0f));
    }

    void Move ()
    {
        // get the input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // calculate a direction relative to where we're facing
        Vector3 dir = (transform.forward * z + transform.right * x) * speed;
        dir.y = rig.velocity.y;
        // set that as our velocity
        rig.velocity = dir;
    }
    
    void TryJump ()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);
        // shoot the raycast
        if(Physics.Raycast(ray, 1.5f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool onWater()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x,
            _col.bounds.min.y, _col.bounds.center.z);
        bool water = Physics.CheckCapsule(_col.bounds.center,
            capsuleBottom, distanceToGround, waterLayer,
                QueryTriggerInteraction.Ignore);
        return water;
    }

    // Start is called before the first frame update
    void Start()
    {
        _col = GetComponent<CapsuleCollider>();
    }

    void Awake ()
    {
        rig = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Demon")
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPlaying)
            return;
        if(onWater()) {
            speed = 20;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 100, 0.05f);
        }
        else {
            speed = 10;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 80, 0.05f);
        }
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
        TryJump();
        curTimeText.text = (Time.time - startTime).ToString("F2");
    }
}
