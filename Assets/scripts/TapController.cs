using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;    // Bird rotates as it falls
    public Vector3 startPosition;

    public AudioSource jumpAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigidBody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rigidBody.simulated = false;
	}
	
	// Update is every frame update (once per frame)
	void Update () {
        if (game.GameOver) { return; }
        if (Input.GetMouseButtonDown(0)) // Left-Mouse button, auto-converts to a tap on mobile
        {
            jumpAudio.Play();
            transform.rotation = forwardRotation;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
	}

    // Subscribe to GameManager's GameStart and GameOver events
    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    // Unubscribe to GameManager's GameStart and GameOver events
    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    // Activates game physics on game start
    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    // Stops game physics on game end and resets bird position
    void OnGameOverConfirmed()
    {
        transform.localPosition = startPosition;
        transform.rotation = Quaternion.identity;
    }

    // Looking for DeadZones and ScoreZones
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ScoreZone")
        {
            // Register a score event
            OnPlayerScored(); // Event sent to GameManager
            // Play a sound
            scoreAudio.Play();
        }
        if (collision.gameObject.tag == "DeadZone")
        {
            rigidBody.simulated = false; // Freezes Bird
            // Register a dead event
            OnPlayerDied(); // Event sent to GameManager
            // Play a sound
            dieAudio.Play();
        }
    }
}
