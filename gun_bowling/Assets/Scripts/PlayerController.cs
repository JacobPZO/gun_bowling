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

    [SerializeField]
    private Gun Gun;

    public void PlayerShoot()
    {
        Gun.Shoot();
    }

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

    // Start is called before the first frame update
    void Start()
    {
        _col = GetComponent<CapsuleCollider>();
    }

    void Awake ()
    {
        rig = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(!isPlaying)
            return;
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
            TryJump();
        if (Input.GetMouseButtonDown(0))
            PlayerShoot();
        curTimeText.text = (Time.time - startTime).ToString("F2");
    }
}
