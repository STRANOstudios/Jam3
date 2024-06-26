using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class Enemy1Controller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0f)] float fireRatio = 0.5f;
    [SerializeField, Min(0f)] float viewDistance = 10f;
    [SerializeField, Range(0f, 80f)] float viewAngle = 30f;
    [SerializeField, Min(0f)] float safeDistance = 10f;

    [SerializeField] LayerMask targetMask;

    [Header("References")]
    [SerializeField, Tooltip("system of particles to be used as a projectile")] ParticleSystem fireParticles;
    [Space]
    [SerializeField] Transform body;
    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    [Header("VFX")]
    [SerializeField, Min(0.1f)] float rotationAmplitude = 10f;
    //[SerializeField, Min(0.1f)] float timeVFXCollapse = 0.5f;
    //[SerializeField, Min(0.1f)] float timeVFXSpawn = 0.5f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxMove;
    [SerializeField] AudioClip sfxFire;

    [Header("Debug")]
    [SerializeField] bool debug = true;

    private State currentState;
    private Transform target;
    private NavMeshAgent agent;

    private float timeSinceLastFire = 0f;
    private float previusVelocity = 0f;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (targetMask == 0) Debug.LogWarning("targetMask not setted");
        if (!body) Debug.LogWarning("body not assigned");
        if (!turret) Debug.LogWarning("turret not assigned");
        if (!barrel) Debug.LogWarning("barrel not assigned");

        // VFX
        if (!fireParticles) Debug.LogWarning("fireParticles not assigned");
    }

#endif

    private void Start()
    {
        //target = Move.Instance.transform;
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        currentState = new Idle(this);
    }

    private void Update()
    {
        //Debug.Log(currentState.name);
        currentState = currentState.Process();

        MotionVFX();
    }

    private void MotionVFX()
    {
        float velocity = agent.velocity.magnitude;

        float velocityRound = Mathf.Round(velocity * 100f) / 100f;

        float speed = (velocityRound - previusVelocity) / Time.deltaTime;

        float rotationAngle = 0f;

        if (speed > 0)
        {
            rotationAngle = -rotationAmplitude;
        }
        else if (speed < 0)
        {
            rotationAngle = rotationAmplitude;
        }

        previusVelocity = velocityRound;

        Quaternion targetRotation = Quaternion.Euler(rotationAngle, 0f, 0f);

        body.localRotation = Quaternion.Lerp(body.localRotation, targetRotation, 5f * Time.deltaTime);
    }


    // Refernces
    public Transform Target => target;
    public Transform Body => body;
    public Transform Turret => turret;
    public Transform Barrel => barrel;

    // Settings
    public float FireRatio => fireRatio;
    public float ViewDistance => viewDistance;
    public float SafeDistance => safeDistance;
    public float ViewAngle => viewAngle;
    public LayerMask TargetMask => targetMask;
    public float TimeSinceLastFire
    {
        get { return timeSinceLastFire; }
        set { timeSinceLastFire = value; }
    }
    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    // IA
    public NavMeshAgent NavMeshAgent => agent;

    // VFX
    public ParticleSystem FireParticles => fireParticles;

    // SFX
    public AudioClip SfxMove => sfxMove;
    public AudioClip SfxFire => sfxFire;

    private void OnDrawGizmos()
    {
        if (debug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(barrel.position, safeDistance);
    }
}