using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float maxSpeed = 4f;
    public float acceleration = 1.5f;
    public Rigidbody2D rb;

    [Header("PlayerStates")]
    [Tooltip("Estado actual del jugador")]
    public ENUM_PlayerStates state;

    [Tooltip("Arma equipada por el jugador")]
    public ENUM_Weapons weapon;

    [Header("Jump")]
    //public float jumpSpeed = 5f;
    public float height = 1;
    public float timeAtheightPeak = 5;
    public float fallingGravity = 3f;
    public float peakGlideGravityDecrease = 1.75f;
    public float maxVerticalSpeed = 3f;

    [Header("Coyote Time")]
    [Tooltip("Estado actual del jugador")]
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 sizeGroundCheck = new Vector2(0.16f, 0.03f);
    [SerializeField] bool grounded;
    [HideInInspector] public Color groundColor = Color.white;

    private Vector3 _velocity;
    private float _targetGravity;
    private Coroutine _increaseGravity;
    private Collider2D _groundCollision;
    private bool _lastGrounded;
    private float _defaultGravityMultiplier;
    private float _increasedGravityValue;
    private float _currentIncreasedGravityValue;
    private float _decreasedGravityValue;
    private float _currentDecreasedGravityValue;
    private bool _afectedByIntenseFalling;
    private bool _isDead;
    [SerializeField] private float _storedCombo;
    [SerializeField] private float _storedScore;

    private float _currentSpeed;

    public float GravityScale { get => _defaultGravityMultiplier * _currentIncreasedGravityValue / _currentDecreasedGravityValue; }

    public static PlayerController instance;
    private void Awake() {
        if (!instance) { instance = this; }

        _defaultGravityMultiplier = rb.gravityScale;
        _decreasedGravityValue = rb.gravityScale / peakGlideGravityDecrease;
        _increasedGravityValue = rb.gravityScale * fallingGravity;
        _currentIncreasedGravityValue = 1;
        _currentDecreasedGravityValue = 1;
        state = ENUM_PlayerStates.Running;

    }

    private void Update() {
        EvaluateGrounded();
        CalculateCoyoteTime();
        Accelerate();
        Movement();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, sizeGroundCheck);
    }

    private void Accelerate() {
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    }

    private void Movement() {
        Vector3 velocity = rb.velocity;
        velocity.x = _currentSpeed;
        rb.velocity = velocity;

        ClampVerticalSpeed();
    }

    private void ClampVerticalSpeed() {
        _velocity = rb.velocity;
        _velocity.y = Mathf.Clamp(_velocity.y, -maxVerticalSpeed, 100);
        rb.velocity = _velocity;
    }

    /// <summary>
    /// Checks if the player is grounded and update the boolean
    /// </summary>
    private void EvaluateGrounded() {
        _groundCollision = Physics2D.OverlapBox(groundCheck.position, sizeGroundCheck, 0f, groundLayer);
        grounded = _groundCollision ? true : false;

        //#region GroundColor
        //if (grounded && _groundCollision.TryGetComponent(out GroundColor groundColor)) {
        //    this.groundColor = groundColor.color;
        //} else {
        //    this.groundColor = Color.white;
        //}
        //#endregion

        if (rb.velocity.y < 0 && (!_lastGrounded && grounded)) {
            RecoverDefaultGravity();
            MarkBoolsWhenLanding();
        }

        if (rb.velocity.y < 0 && !grounded && !_afectedByIntenseFalling) {
            _currentIncreasedGravityValue = _increasedGravityValue;
            rb.gravityScale = GravityScale;
            _afectedByIntenseFalling = true;
        }

        _lastGrounded = grounded;
    }

    public void Jump() {
        if (coyoteTimeCounter<=0) return;

        coyoteTimeCounter=0f;

        state = ENUM_PlayerStates.Jumping;

        Vector3 velocity = rb.velocity;
        float jumpVelocity = CalculateJumpVelocityWithTotalTime();
        velocity.y = jumpVelocity;
        rb.velocity = velocity;

        rb.gravityScale = CalculateGravity();

        IncreaseGravityAtPeak();
    }

    public void CalculateCoyoteTime()
    {
        if (grounded) {coyoteTimeCounter = coyoteTime;} else coyoteTimeCounter-=Time.deltaTime;
    }

    private float CalculateJumpVelocityWithTotalTime() {
        return (2 * height) / (timeAtheightPeak);
    }

    private float CalculateGravity() {
        _targetGravity = (-2 * height) / (timeAtheightPeak * timeAtheightPeak);
        return _targetGravity / Physics2D.gravity.y;
    }

    private void IncreaseGravityAtPeak() {

        if (_increaseGravity != null) {
            StopCoroutine(_increaseGravity);
        }
        _increaseGravity = StartCoroutine(IncreaseGravityAtPeakCoroutine());
    }

    private IEnumerator IncreaseGravityAtPeakCoroutine() {

        while (rb.velocity.y > 0.17f) {

            yield return null;
        }

        _currentIncreasedGravityValue = 1;
        _currentDecreasedGravityValue = _decreasedGravityValue;
        rb.gravityScale = GravityScale;

        while (rb.velocity.y > -0.1f) {
            yield return null;
        }

        _currentDecreasedGravityValue = 1;
        _currentIncreasedGravityValue = _increasedGravityValue;
        rb.gravityScale = GravityScale;
    }

    private void RecoverDefaultGravity() {
        _currentIncreasedGravityValue = 1;
        _currentDecreasedGravityValue = 1;

        if (rb != null) {
            rb.gravityScale = GravityScale;
        }
    }

    private void MarkBoolsWhenLanding() {
        _afectedByIntenseFalling = false;
    }

    //public void FootSteps() {
    //    SoundFxController.instance.FootSteps();
    //}
}
