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

    [Header("Glide")]
    [Tooltip("Gravedad reducida durante el planeo.")]
    public float glideGravity = 0.5f;
    [Tooltip("Velocidad máxima hacia abajo durante el planeo.")]
    public float glideFallSpeed = -2f;
    //Para controlar el estado de planeo.
    private bool isGliding = false;
    // Indica si se ha pedido el planeo.
    private bool glideRequested = false;
    [Tooltip("Aceleración extra cuando el jugador está planeando.")]
    public float glideSpeedMultiplier = 1.5f;



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

    private void Update()
    {
        EvaluateGrounded();
        CalculateCoyoteTime();
        Accelerate();
        Movement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || coyoteTimeCounter > 0)
            {
                Jump(); // Primer salto.
            }
            else if (!isGliding && state == ENUM_PlayerStates.Jumping)
            {
                glideRequested = true; // Solicitar planeo.
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGliding)
        {
            StopGlide(); // Detener el planeo al soltar Espacio.
        }

        // Comprobamos si estamos saltando o cayendo y activamos el planeo si es necesario.
        if (glideRequested && state == ENUM_PlayerStates.Jumping && !isGliding)
        {
            if (rb.velocity.y <= 0) // Solo activar cuando estamos cayendo o al alcanzar el pico.
            {
                StartGlide();
                glideRequested = false; // Reiniciar la solicitud después de activar el planeo.
            }
        }
    }




    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, sizeGroundCheck);
    }

    private void Accelerate() {
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    }

    private void Movement()
    {


        Vector3 velocity = rb.velocity;

        // Aumentamos la velocidad hacia adelante si estamos planeando
        if (isGliding)
        {
            velocity.x = _currentSpeed * glideSpeedMultiplier; // Aumentar la velocidad durante el planeo
        }
        else
        {
            velocity.x = _currentSpeed; // Lógica normal
        }

        rb.velocity = velocity;

        // Limitar la velocidad vertical (esto ya lo teníamos antes)
        ClampVerticalSpeed();
    }

    private void ClampVerticalSpeed()
    {
        _velocity = rb.velocity;

        if (isGliding)
        {
            _velocity.y = Mathf.Clamp(_velocity.y, glideFallSpeed, 100); // Limitar velocidad hacia abajo durante el planeo.
        }
        else
        {
            _velocity.y = Mathf.Clamp(_velocity.y, -maxVerticalSpeed, 100); // Velocidad normal en otras situaciones.
        }

        rb.velocity = _velocity;
    }


    /// <summary>
    /// Checks if the player is grounded and update the boolean
    /// </summary>
    private void EvaluateGrounded()
    {
        _groundCollision = Physics2D.OverlapBox(groundCheck.position, sizeGroundCheck, 0f, groundLayer);
        grounded = _groundCollision ? true : false;

        if (grounded)
        {
            if (isGliding)
            {
                StopGlide(); // Terminar el planeo al aterrizar.
            }
        }

        if (rb.velocity.y < 0 && (!_lastGrounded && grounded))
        {
            RecoverDefaultGravity();
            MarkBoolsWhenLanding();
            if (grounded) state = ENUM_PlayerStates.Running;
        }

        _lastGrounded = grounded;
    }


    public void Jump() {

        coyoteTimeCounter=0f;

        state = ENUM_PlayerStates.Jumping;

        Debug.Log(state);

        Vector3 velocity = rb.velocity;
        float jumpVelocity = CalculateJumpVelocityWithTotalTime();
        velocity.y = jumpVelocity;
        rb.velocity = velocity;

        rb.gravityScale = CalculateGravity();

        IncreaseGravityAtPeak();
    }

    private void StartGlide()
    {
        if (isGliding) return; // Evitar múltiples activaciones.

        isGliding = true;
        state = ENUM_PlayerStates.Gliding;
        rb.gravityScale = glideGravity; // Reducir la gravedad para el planeo.
        Debug.Log("Planeo activado.");
    }



    private void StopGlide()
    {
        if (!isGliding) return; // Evitar múltiples llamadas.

        isGliding = false;
        state = ENUM_PlayerStates.Jumping; // Volver al estado de salto.
        RecoverDefaultGravity(); // Restaurar la gravedad normal.
        Debug.Log("Planeo desactivado.");
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

    private IEnumerator IncreaseGravityAtPeakCoroutine()
    {
        // Esperar hasta llegar al pico del salto
        while (rb.velocity.y > 0.17f)
        {
            yield return null; // Continuar esperando mientras sube.
        }

        // Cuando se alcance el pico o comience a caer, verificamos si se ha solicitado planeo
        if (glideRequested && !isGliding)
        {
            StartGlide();
            glideRequested = false; // Reiniciar la solicitud después de activar el planeo.
        }

        // Mientras esté planeando, esperar hasta que comience a caer
        while (rb.velocity.y > -0.1f && isGliding)
        {
            yield return null;
        }
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
