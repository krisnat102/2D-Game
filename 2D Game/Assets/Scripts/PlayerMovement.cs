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

	[SerializeField]
	private float dodgePower = 200f;
	[SerializeField]
	private float dodgeCost = 20f;
	private bool dodgeDirection = true;


	void Update()
	{

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

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
		if (PlayerStats.stam > 20)
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
		}
    }
}
