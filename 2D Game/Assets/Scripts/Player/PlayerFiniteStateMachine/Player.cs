using UnityEngine;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Components

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Rigidbody2D rb;

    public Animator Animator { get => animator; private set => animator = value; }
    public PlayerInputHandler InputHandler { get => inputHandler; private set => inputHandler = value; }
    public Rigidbody2D RB { get => rb; private set => rb = value; }
    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 workspace;
    #endregion

    #region Check Variables
    public int FacingDirection { get; private set; }

    [SerializeField] private Transform groundCheck;
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);

        FacingDirection = 1;

        //InputHandler = gameObject.GetComponent<PlayerInputHandler>();
        //Animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        CurrentVelocity = RB.velocity;

        if(InputHandler.JumpInputStop == true)
        {
            RB.velocity = new Vector2();
        }
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Set Functions
    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        SetFinalVelocity();
    }
    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        SetFinalVelocity();
    }
    private void SetFinalVelocity()
    {
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    #endregion

    #region Check Functions
    public void CheckIfShouldFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
    #endregion
}
