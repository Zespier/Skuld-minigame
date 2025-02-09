using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform playerCenter;
    public SpriteRenderer spriteRenderer;
    public Vector2 getHitStregth = new Vector2(-3.5f, 3f);

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
    private float lastAttackTime;
    private Collider2D[] enemiesInRange;
    [Tooltip("DelayAnimacionAtaque")]
    public float delayAnimAttack = 0.2f;


    [Header("Jump")]
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
    private bool isGliding = false;
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

    private float _currentSpeed;
    private bool _canChangeState = true;

    private Animator _animator;

    public float GravityScale { get => _defaultGravityMultiplier * _currentIncreasedGravityValue / _currentDecreasedGravityValue; }
    public float TopLimit => CameraController.instance.transform.position.y + ModuleContainer.instance.mainCamera.orthographicSize;
    public bool IsInIdleSide => transform.position.y < -1.5f;
    public ENUM_PlayerStates State { get => state; set => ChangeState(value); }
    public bool IsUsingAbility => State == ENUM_PlayerStates.Ability_1 || State == ENUM_PlayerStates.Ability_2 || state == ENUM_PlayerStates.Ability_3;

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
        State = ENUM_PlayerStates.Running;

        _animator = GetComponentInChildren<Animator>();
    }

    private void Update() {

        EvaluateGrounded();
        CalculateCoyoteTime();
        Accelerate();
        Movement();
        JumpGlideActions();
        AutoAttack();

        if (!grounded && State == ENUM_PlayerStates.Running) {
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
        if (State != ENUM_PlayerStates.Attacking)
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, State == ENUM_PlayerStates.Attacking ? 0 : maxSpeed, acceleration * Time.deltaTime);
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

        if (IsInIdleSide && State == ENUM_PlayerStates.Attacking) return;
        if (!_canChangeState) { return; }

        State = ENUM_PlayerStates.Jumping;

        Vector3 velocity = rb.velocity;
        float jumpVelocity = CalculateJumpVelocityWithTotalTime();
        velocity.y = jumpVelocity;
        rb.velocity = velocity;

        rb.gravityScale = CalculateGravity();

        IncreaseGravityAtPeak();
        _hasJump = true;
    }

    public void CalculateCoyoteTime() {
        if (grounded && State != ENUM_PlayerStates.Jumping) { coyoteTimeCounter = coyoteTime; } else coyoteTimeCounter -= Time.deltaTime;
    }

    private float CalculateJumpVelocityWithTotalTime() {
        return (2 * height) / (timeAtheightPeak);
    }

    private float CalculateGravity() {

        if (endULTI) return 0;

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
        if (isGliding || !_canChangeState) return; // Evitar múltiples activaciones.
        PlayAnimation("Glide");

        isGliding = true;
        State = ENUM_PlayerStates.Gliding;
        rb.gravityScale = glideGravity; // Reducir la gravedad para el planeo.
    }

    private void StopGlide() {
        if (!isGliding) return; // Evitar múltiples llamadas.
        PlayAnimation("JumpLanding");

        isGliding = false;
        State = ENUM_PlayerStates.Jumping; // Volver al estado de salto.
        RecoverDefaultGravity(); // Restaurar la gravedad normal.
    }
    #endregion

    private void JumpGlideActions() {
        if (!_canChangeState) { return; }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if ((grounded || coyoteTimeCounter > 0) && State != ENUM_PlayerStates.Jumping) {
                Jump(); // Primer salto.
            } else if (!isGliding && State == ENUM_PlayerStates.Jumping) {
                glideRequested = true; // Solicitar planeo.
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGliding) {
            StopGlide(); // Detener el planeo al soltar Espacio.
        }

        // Comprobamos si estamos saltando o cayendo y activamos el planeo si es necesario.
        if (glideRequested && State == ENUM_PlayerStates.Jumping && !isGliding) {
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

        if (State == ENUM_PlayerStates.Ability_3) return;

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
            if (State != ENUM_PlayerStates.Ability_2 && grounded) {

                PlayAnimation("JumpLanding");
                State = ENUM_PlayerStates.Running;
            }

        }

        _lastGrounded = grounded;
    }

    private void MarkBoolsWhenLanding() {
        _afectedByIntenseFalling = false;
        if (_canChangeState) {
            rb.excludeLayers = 0;
        }
    }
    #endregion

    private void AutoAttack() {
        enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0 && Time.time > lastAttackTime + attackCooldown) {
            foreach (Collider2D collision in enemiesInRange) {
                if (collision == null) { continue; }

                if (collision.TryGetComponent(out Enemy enemy)) {

                    if (IsInIdleSide) {

                        if (enemy._currentHP >= enemy._maxHP && enemy.type == Enemy.EnemyType.Static) {
                            _animator.Play("Skuld_IdleToAttack");
                            State = ENUM_PlayerStates.Attacking;
                            //_currentSpeed = 0;
                            StartCoroutine(C_SlowlyStop());
                            enemy.ReduceHp(attackDamage);

                        } else if (enemy._currentHP > 1) {

                            enemy.ReduceHp(attackDamage);

                        } else if (enemy._currentHP > 0) {
                            enemy.ReduceHp(attackDamage);
                            _animator.SetTrigger("exitAttack");
                            State = ENUM_PlayerStates.Running;
                        } else {
                            _animator.ResetTrigger("exitAttack");

                        }

                    } else {
                        if (enemy.strength == Enemy.EnemyStrength.Weak || IsUsingAbility) {
                            enemy.Death();
                            _animator.Play("Attack Spear");
                        }
                    }
                }
            }

            lastAttackTime = Time.time;
        }
    }

    private IEnumerator C_SlowlyStop() {
        _currentSpeed = 1;
        while (_currentSpeed > 0) {
            _currentSpeed -= Time.deltaTime * 1.8f;
            yield return null;
        }
        _currentSpeed = 0;
    }

    private void WallFrameCheck() {

        if (enterULTI) return;

        currentFrameCheck = transform.position.x;

        if (currentFrameCheck == previousFrameCheck1 && previousFrameCheck1 == previousFrameCheck2 && State != ENUM_PlayerStates.Attacking) {
            Debug.Log("El valor no ha cambiado en los últimos dos frames.");

            rb.gravityScale = wallFallGravity;

            wallStamp = true;
        } else {
            rb.gravityScale = CalculateGravity();
            wallStamp = false;
        }

        previousFrameCheck2 = previousFrameCheck1;
        previousFrameCheck1 = currentFrameCheck;
    }

    private void TimerWallCheckInit() {
        if (waitCheckWall > 0) waitCheckWall -= Time.deltaTime; else WallFrameCheck();

    }

    public void StartCoroutineSkill1() {
        if (_canChangeState) {
            StartCoroutine(UpSkillImpulse());
        }
    }

    public void StartCoroutineSkill2() {
        if (_canChangeState) {
            StartCoroutine(DownSkillImpulse());
        }
    }

    public void StartCoroutineSkill3() {
        if (_canChangeState) {
            StartCoroutine(FrontSkillImpulse());
        }
    }

    public IEnumerator UpSkillImpulse() {
        if (IsInIdleSide) {

            if (grounded) {
                State = ENUM_PlayerStates.Ability_1;

                PlayAnimation("Skuld_Helicopter");
                Vector3 newVelocity = rb.velocity;
                newVelocity.y = jumpmStrengthToGoTOTheFockingMoonAndPlay;
                rb.velocity = newVelocity;
                _currentSpeed = maxSpeed;

                ModuleContainer.instance.GetInitialModule();

                yield return new WaitForSeconds(1f);

                EvaluateState();
            }

        } else {

            PlayAnimation("Skuld_TopHelicopter");

            State = ENUM_PlayerStates.Ability_1;

            wallStamp = false;

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + qJumpForce * rb.gravityScale, 0);

            yield return new WaitForSeconds(1f);

            EvaluateState();

        }

    }

    public IEnumerator DownSkillImpulse() {
        if (!IsInIdleSide) {

            PlayAnimation("Skuld_CutAbility");

            State = ENUM_PlayerStates.Ability_2;

            wallStamp = false;

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - qJumpForce * rb.gravityScale * 2, 0);

            yield return new WaitForSeconds(2f);

            EvaluateState();

        }

    }

    public IEnumerator FrontSkillImpulse() {
        if (!IsInIdleSide) {

            PlayAnimation("Skuld_ULTI");

            State = ENUM_PlayerStates.Ability_3;

            enterULTI = true;
            endULTI = true;

            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            rb.simulated = false;

            yield return new WaitForSeconds(0.6f);

            enterULTI = false;

            rb.simulated = true;

            rb.gravityScale = 0;

            maxSpeed = eMaxSpeed;

            acceleration = eAcceleration;

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
            State = ENUM_PlayerStates.Running;
        } else {
            State = ENUM_PlayerStates.Jumping;

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

    public void GetHit(GameObject attacker) {
        if (_canChangeState && attacker.activeSelf && !IsInIdleSide) {

            rb.excludeLayers = ~0;
            State = ENUM_PlayerStates.Falling;


            Vector3 newVelocity = new Vector3(getHitStregth.x, getHitStregth.y, 0);
            rb.velocity = newVelocity;
            _currentSpeed = getHitStregth.x;

            StartCoroutine(C_DisableChangeState());
            StartCoroutine(C_IntermitentSpriteAfterHit());
        }
    }

    private IEnumerator C_DisableChangeState() {
        _canChangeState = false;

        float timer = 2;
        while (timer >= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        _canChangeState = true;
    }

    public void ChangeState(ENUM_PlayerStates state) {
        if (_canChangeState) {
            this.state = state;
        }
    }

    private IEnumerator C_IntermitentSpriteAfterHit() {

        spriteRenderer.color = Color.white;

        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.white));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.white));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.white));
    }

    private IEnumerator C_IntermitentSpriteAfterHit(Color color) {

        float timer = 0.15f;

        while (timer >= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = color;
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}
