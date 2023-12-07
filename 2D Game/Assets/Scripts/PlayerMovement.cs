using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private AudioSource walking;

	[Header("Stats")]
	[SerializeField] private float runSpeed = 40f;
	[SerializeField] private float dodgePower = 200f;
	[SerializeField] private float dodgeCost = 20f;
	[SerializeField] private float dodgeCooldown = 1f;
	[SerializeField] private float climbSpeed = 200f;

	private CharacterController2D controller;
	private Animator animator;
	private Rigidbody2D rb;
	private float horizontalMove = 0f;
	private float verticalMove = 0f;
	private float speed;
	private bool jump = false;
	private bool crouch = false;
	private bool grassSound = true;
	private bool ableClimb = false;
	private bool dodgeCool = false;
	private bool dodgeDirection = true;
	private Vector3 oldPosition;

    private void Start()
    {
		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
    }

	private void Update()
	{
		if (GameManager.gamePaused == false)
		{
			horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
			verticalMove = Input.GetAxisRaw("Vertical") * climbSpeed;

			animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
			animator.SetFloat("ClimbSpeed", Mathf.Abs(verticalMove));

			if (Input.GetButtonDown("Jump"))
			{
				jump = true;

				animator.SetBool("IsJumping", true);
			}

			if (Input.GetButtonDown("Crouch"))
			{
				crouch = true;
			}
			else if (Input.GetButtonUp("Crouch"))
			{
				crouch = false;
			}
			if (Input.GetButtonDown("Dodge"))
			{
				Dodge();
			}

			if (horizontalMove > 0)
			{
				dodgeDirection = true;
			}
			else if (horizontalMove < 0)
			{
				dodgeDirection = false;
			}
			if (ableClimb == true && Input.GetButton("Vertical"))
			{

				rb.velocity = new Vector2(horizontalMove, verticalMove) * Time.fixedDeltaTime;
			}

			if (horizontalMove != 0 && grassSound == true && CharacterController2D.m_Grounded == true && verticalMove == 0)
			{
				grassSound = false;
				Invoke("GrassRunningSound", 0.2f);
			}
		}
		speed = Vector3.Distance(oldPosition, transform.position) * 100f;
		oldPosition = transform.position;

		if (speed > 30) PlayerStats.Instance.Immune = true;

		else PlayerStats.Instance.Immune = false;
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	public void OnCrouching(bool isCrouching)
	{
		animator.SetBool("IsCrouching", isCrouching);
	}

	void FixedUpdate()
	{
		// move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);

		jump = false;
	}

	private void Dodge()
    {

		Vector2 dodge = new Vector2(dodgePower, 0);

		if (PlayerStats.stam > 20 && dodgeCool == false)
		{
			if (dodgeDirection == true)
			{
				rb.AddForce(dodge);

				PlayerStats.stam -= dodgeCost;
			}
			else
			{
				rb.AddForce(-dodge);

				PlayerStats.stam -= dodgeCost;
			}
			dodgeCool = true;
			Invoke("DodgeCooldown", dodgeCooldown);
		}
	}

	private void DodgeCooldown()
    {
		dodgeCool = false;
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Climbable")
		{
			ableClimb = true;
		}
	}
    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.tag == "Climbable")
		{
			ableClimb = false;
		}
	}

	private void GrassRunningSound()
    {
		grassSound = true;

		walking.Play();

		//FindObjectOfType<AudioManager>().Play("GrassRunning");
	}
}
