using UnityEditor;
using UnityEngine;

public class movementIgor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 5f;
    public VectorValue startingPosition; //AL
    private Animator animator; //AL
    public Rigidbody2D rb;
    Vector2 velocity;
    public AudioSource audioSource;
    public AudioClip walkingSound;
    private bool isPlayingFootsteps = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        transform.position = startingPosition.initialValue;
        
        // Corrected Rigidbody2D settings
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;  // This replaces the constraints setting
        // Get or add AudioSource component
        //audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource
        audioSource.clip = walkingSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector2.zero;
        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove(); //AL
        UpdateFootstepsSound();

    }
        

    void UpdateAnimationAndMove() //AL
    { //AL
        if(velocity !=Vector2.zero) //AL
        {   
            animator.SetFloat("moveX", velocity.x); //AL
            animator.SetFloat("moveY", velocity.y); //AL
            animator.SetBool("moving", true); //AL
        } 
        else{animator.SetBool("moving",false);}
        //AL 
    } //AL
    void UpdateFootstepsSound()
    {
        // Play sound when moving
        if (velocity != Vector2.zero && !isPlayingFootsteps)
        {
            audioSource.Play();
            isPlayingFootsteps = true;
        }
        // Stop sound when not moving
        else if (velocity == Vector2.zero && isPlayingFootsteps)
        {
            audioSource.Stop();
            isPlayingFootsteps = false;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * moveSpeed * Time.fixedDeltaTime);
    }
}