using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public CharacterController2D controller;
	public Animator animator;
	public Rigidbody2D rb;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;

	[SerializeField] private float dodgePower = 200f;
	[SerializeField] private float dodgeCost = 20f;
	[SerializeField] private float dodgeCooldown = 1f;
	private bool dodgeCool = false;
	private bool dodgeDirection = true;

	[SerializeField] private float climbSpeed = 200f;
	private bool ableClimb = false;
	float verticalMove = 0f;

	bool grassSound = true;

	void Update()
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
		FindObjectOfType<AudioManager>().Play("GrassRunning");
	}
}
