using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Movement")]
    public float maxSpeed = 4f;
    private float baseMaxSpeed = 4f;
    public float acceleration = 1.5f;
    private float baseAcceleration = 1.5f;
    public Rigidbody2D rb;

    [Header("PlayerStates")]
    [Tooltip("Estado actual del jugador")]
    public ENUM_PlayerStates state;

    [Tooltip("Arma equipada por el jugador")]
    public ENUM_Weapons weapon;

    [Header("Attack Settings")]
    [Tooltip("Daño de ataque")]
    public int attackDamage = 10;
    [Tooltip("Rango de detección")]
    public float attackRange = 1.5f;
    [Tooltip("Capa de los enemigos")]
    public LayerMask enemyLayer;
    [Tooltip("Intervalo entre ataques")]
    public float attackCooldown = 1f;
    //Tiempo del último ataque
    private float lastAttackTime;
    //Enemigos en el rango de ataque que detectaremos con una overlapsphere
    private Collider2D[] enemiesInRange;
    [Tooltip("DelayAnimacionAtaque")]
    public float delayAnimAttack = 0.2f;


    [Header("Jump")]
    //public float jumpSpeed = 5f;
    public float jumpmStrengthToGoTOTheFockingMoonAndPlay = 10f;
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
    [Tooltip("Tiempo dle que disponemos para saltar")]
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 sizeGroundCheck = new Vector2(0.16f, 0.03f);
    [SerializeField] bool grounded;
    [HideInInspector] public Color groundColor = Color.white;

    [Header("WallCheck")]
    public float currentFrameCheck;
    public float previousFrameCheck1;
    public float previousFrameCheck2;

    public bool wallStamp;
    public float wallFallGravity = 5f;
    private float waitCheckWall = 0.5f;

    [Header("Skills")]
    public float qJumpForce = 2.5f;
    public bool enterULTI;
    public bool endULTI;
    public float eMaxSpeed = 10f;
    public float eAcceleration = 4f;

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
    private bool _hasJump;
    private string _lastAnimationName;
    [SerializeField] private float _storedCombo;
    [SerializeField] private float _storedScore;

    private float _currentSpeed;

    private Animator _animator;

    public float GravityScale { get => _defaultGravityMultiplier * _currentIncreasedGravityValue / _currentDecreasedGravityValue; }
    public float TopLimit => CameraController.instance.transform.position.y + ModuleContainer.instance.mainCamera.orthographicSize;
    public bool IsInIdleSide => transform.position.y < -1.5f;

    public static PlayerController instance;
    private void Awake() {
        if (!instance) { instance = this; }

        _defaultGravityMultiplier = rb.gravityScale;
        _decreasedGravityValue = rb.gravityScale / peakGlideGravityDecrease;
        _increasedGravityValue = rb.gravityScale * fallingGravity;
        _currentIncreasedGravityValue = 1;
        _currentDecreasedGravityValue = 1;
        baseMaxSpeed = maxSpeed;
        baseAcceleration = acceleration;
        state = ENUM_PlayerStates.Running;

        _animator = GetComponentInChildren<Animator>();
    }

    private void Update() {

        EvaluateGrounded();
        CalculateCoyoteTime();
        Accelerate();
        Movement();
        //Acciones de salto y planeo
        JumpGlideActions();
        // Comprobar ataques automáticos
        AutoAttack();

        if (!grounded && state == ENUM_PlayerStates.Running) {
            PlayAnimation("JumpTransition");
        }

    }
    private void FixedUpdate() {
        TimerWallCheckInit();
    }

    private void LateUpdate() {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y, -5.42f, 2.8f);
        transform.position = newPosition;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, sizeGroundCheck);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #region Movement
    private void Accelerate() {
        if (state != ENUM_PlayerStates.Attacking)
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    }

    private void Movement() {
        if (wallStamp && !grounded) return;
        if (enterULTI) return;

        Vector3 velocity = rb.velocity;

        // Aumentamos la velocidad hacia adelante si estamos planeando
        if (isGliding) {
            velocity.x = _currentSpeed * glideSpeedMultiplier; // Aumentar la velocidad durante el planeo
        } else {
            velocity.x = _currentSpeed; // Lógica normal
        }

        rb.velocity = velocity;

        // Limitar la velocidad vertical (esto ya lo teníamos antes)
        ClampVerticalSpeed();
    }

    private void ClampVerticalSpeed() {

        if (enterULTI) return;

        _velocity = rb.velocity;

        if (isGliding) {
            _velocity.y = Mathf.Clamp(_velocity.y, glideFallSpeed, 100); // Limitar velocidad hacia abajo durante el planeo.
        } else {
            _velocity.y = Mathf.Clamp(_velocity.y, -maxVerticalSpeed, 100); // Velocidad normal en otras situaciones.
        }

        rb.velocity = _velocity;
    }
    #endregion

    #region Jump Glide Logic
    public void Jump() {

        //coyoteTimeCounter=0f;

        state = ENUM_PlayerStates.Jumping;

        Vector3 velocity = rb.velocity;
        float jumpVelocity = CalculateJumpVelocityWithTotalTime();
        velocity.y = jumpVelocity;
        rb.velocity = velocity;

        rb.gravityScale = CalculateGravity();

        IncreaseGravityAtPeak();
        _hasJump = true;
    }

    public void CalculateCoyoteTime() {
        if (grounded && state != ENUM_PlayerStates.Jumping) { coyoteTimeCounter = coyoteTime; } else coyoteTimeCounter -= Time.deltaTime;
    }

    private float CalculateJumpVelocityWithTotalTime() {
        return (2 * height) / (timeAtheightPeak);
    }

    private float CalculateGravity() {

        if(endULTI) return 0;

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

        PlayAnimation("JumpUp");
        // Esperar hasta llegar al pico del salto
        while (rb.velocity.y > 0.17f) {
            yield return null; // Continuar esperando mientras sube.
        }
        PlayAnimation("JumpTransition");

        // Cuando se alcance el pico o comience a caer, verificamos si se ha solicitado planeo
        if (glideRequested && !isGliding) {
            StartGlide();
            glideRequested = false; // Reiniciar la solicitud después de activar el planeo.
        }

        // Mientras esté planeando, esperar hasta que comience a caer
        while (rb.velocity.y > -0.1f && isGliding) {
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

    #region Glide
    private void StartGlide() {
        if (isGliding) return; // Evitar múltiples activaciones.
        PlayAnimation("Glide");

        isGliding = true;
        state = ENUM_PlayerStates.Gliding;
        rb.gravityScale = glideGravity; // Reducir la gravedad para el planeo.
        Debug.Log("Planeo activado.");
    }

    private void StopGlide() {
        if (!isGliding) return; // Evitar múltiples llamadas.
        PlayAnimation("JumpLanding");

        isGliding = false;
        state = ENUM_PlayerStates.Jumping; // Volver al estado de salto.
        RecoverDefaultGravity(); // Restaurar la gravedad normal.
        Debug.Log("Planeo desactivado.");
    }
    #endregion

    private void JumpGlideActions() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if ((grounded || coyoteTimeCounter > 0) && state != ENUM_PlayerStates.Jumping) {
                Jump(); // Primer salto.
            } else if (!isGliding && state == ENUM_PlayerStates.Jumping) {
                glideRequested = true; // Solicitar planeo.
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGliding) {
            StopGlide(); // Detener el planeo al soltar Espacio.
        }

        // Comprobamos si estamos saltando o cayendo y activamos el planeo si es necesario.
        if (glideRequested && state == ENUM_PlayerStates.Jumping && !isGliding) {
            if (rb.velocity.y <= 0) // Solo activar cuando estamos cayendo o al alcanzar el pico.
            {
                StartGlide();
                glideRequested = false; // Reiniciar la solicitud después de activar el planeo.
            }
        }
    }
    #endregion

    #region GroundCheck
    /// <summary>
    /// Checks if the player is grounded and update the boolean
    /// </summary>
    private void EvaluateGrounded() {

        if (state == ENUM_PlayerStates.Ability_3) return;

        _groundCollision = Physics2D.OverlapBox(groundCheck.position, sizeGroundCheck, 0f, groundLayer);
        grounded = _groundCollision ? true : false;

        _animator.SetBool("isGrounded", grounded);

        if (grounded) {
            if (isGliding) {
                StopGlide(); // Terminar el planeo al aterrizar.
            }
        }

        if (rb.velocity.y < 0 && (!_lastGrounded && grounded)) {
            RecoverDefaultGravity();
            MarkBoolsWhenLanding();
            if (state != ENUM_PlayerStates.Ability_2 && grounded) {

                 PlayAnimation("JumpLanding");
                 state = ENUM_PlayerStates.Running;
            } 

        }

        _lastGrounded = grounded;
    }

    private void MarkBoolsWhenLanding() {
        _afectedByIntenseFalling = false;
    }
    #endregion

    private void AutoAttack() {
        enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0 && Time.time > lastAttackTime + attackCooldown) {
            foreach (Collider2D colision in enemiesInRange) {
                if (colision == null) {
                    continue;
                }
                // Aquí puedes llamar al método de daño del enemigo
                Debug.Log($"Atacando a {colision.name}");
                // enemy.GetComponent<EnemyController>()?.TakeDamage(attackDamage);
                //Testeo


                if (IsInIdleSide) {

                    Enemy enemy = colision.GetComponent<Enemy>();
                    if (enemy == null) { continue; }
                    int enemyHealth = enemy._currentHP;

                    if (enemyHealth >= enemy._maxHP && enemy.type == Enemy.EnemyType.StaticBig) {
                        _animator.Play("Skuld_IdleToAttack");
                        state = ENUM_PlayerStates.Attacking;
                        _currentSpeed = 0;
                        enemy.Set(attackDamage);

                    } else if (enemyHealth > 0) {

                        enemy.Set(attackDamage);

                    } else {
                        Debug.Log("Dejo de atacar");
                        _animator.SetTrigger("exitAttack");
                        state = ENUM_PlayerStates.Running;
                        colision.gameObject.SetActive(false);
                    }

                } else {
                    StartCoroutine(DestroyTimer(colision.gameObject));
                    _animator.Play("Attack Spear");
                }

            }

            // Actualizar el tiempo del último ataque
            lastAttackTime = Time.time;
        }
    }

    public IEnumerator DestroyTimer(GameObject target) {

        yield return new WaitForSeconds(delayAnimAttack);

        target.SetActive(false);
    }

    private void WallFrameCheck() {

        if(enterULTI) return;

        // Simular un valor que cambia (puedes reemplazarlo con el valor real que quieres comprobar)
        currentFrameCheck = transform.position.x;

        // Comprobar si el valor se ha mantenido igual en los últimos dos frames
        if (currentFrameCheck == previousFrameCheck1 && previousFrameCheck1 == previousFrameCheck2 && state != ENUM_PlayerStates.Attacking) {
            Debug.Log("El valor no ha cambiado en los últimos dos frames.");

            rb.gravityScale = wallFallGravity;

            wallStamp = true;
        } else {
            rb.gravityScale = CalculateGravity();
            wallStamp = false;
            //Debug.Log("El valor ha cambiado.");
        }

        // Actualizar los valores para el siguiente frame
        previousFrameCheck2 = previousFrameCheck1;
        previousFrameCheck1 = currentFrameCheck;
    }

    private void TimerWallCheckInit() {
        if (waitCheckWall > 0) waitCheckWall -= Time.deltaTime; else WallFrameCheck();

    }

    public void StartCoroutineSkill1() {
        StartCoroutine(UpSkillImpulse());
    }

    public void StartCoroutineSkill2()
    {
        StartCoroutine(DownSkillImpulse());
    }

    public void StartCoroutineSkill3()
    {
        StartCoroutine(FrontSkillImpulse());
    }

    public IEnumerator UpSkillImpulse() {
        if (IsInIdleSide) {

            if (grounded)
            {
                state = ENUM_PlayerStates.Ability_1;

                PlayAnimation("Skuld_Helicopter");
                Vector3 newVelocity = rb.velocity;
                newVelocity.y = jumpmStrengthToGoTOTheFockingMoonAndPlay;
                rb.velocity = newVelocity;

                ModuleContainer.instance.GetInitialModule();

                yield return new WaitForSeconds(1f);

                EvaluateState();
            }

        } else {

            PlayAnimation("Skuld_TopHelicopter");

            state = ENUM_PlayerStates.Ability_1;

            wallStamp = false;

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + qJumpForce * rb.gravityScale, 0);

            yield return new WaitForSeconds(1f);

            EvaluateState();

        }

    }

    public IEnumerator DownSkillImpulse()
    {
        if (!IsInIdleSide)
        {

            PlayAnimation("Skuld_CutAbility");

            state = ENUM_PlayerStates.Ability_2;

            wallStamp = false;

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - qJumpForce * rb.gravityScale *2, 0);

            yield return new WaitForSeconds(2f);

            EvaluateState();

        }

    }

    public IEnumerator FrontSkillImpulse()
    {
        if (!IsInIdleSide)
        {

            PlayAnimation("Skuld_ULTI");

            state = ENUM_PlayerStates.Ability_3;

            enterULTI=true; 
            endULTI=true; 

            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            rb.simulated = false;

            yield return new WaitForSeconds(0.6f);

            enterULTI = false;

            rb.simulated = true;

            rb.gravityScale = 0;

            maxSpeed = eMaxSpeed;

            acceleration = eAcceleration;

            //rb.velocity = new Vector3(rb.velocity.x + 30f, rb.velocity.y, 0);

            yield return new WaitForSeconds(1.2f);

            rb.constraints = RigidbodyConstraints2D.None;

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            maxSpeed = baseMaxSpeed;

            acceleration = baseAcceleration;

            EvaluateState();

            endULTI = false;

        }
    }
    private void EvaluateState() {
        if (grounded) {
            state = ENUM_PlayerStates.Running;
        } else {
            state = ENUM_PlayerStates.Jumping;

        }
    }


    //public void FootSteps() {
    //    SoundFxController.instance.FootSteps();
    //}

    public void PlayAnimation(string animationName) {
        if (_lastAnimationName != animationName) {
            _animator.Play(animationName);
        }

        _lastAnimationName = animationName;
    }
}
