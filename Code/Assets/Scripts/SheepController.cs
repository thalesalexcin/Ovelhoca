using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour 
{
    public bool IsDead { get; set; }

    public Vector3 Velocity;
    public Vector3 JumpForce;
    public Vector3 DoubleJumpForce;
    public Projectile ProjectilePrefab;

    public float ProjectileSpeed = 1;
    public float TurningSpeed;
    public int InputFramesDelay = 0;
    public float ShootingDelay = 0;
    public SheepType[] SheepTypes;

    private Rigidbody _Rigidbody;
    private SheepState _CurrentState;
    private Vector3 _Velocity;
    private float _TurningAngle;
    private Queue<EAction> _Inputs;
    private float _InputTimer;
    private EAction _CurrentAction;
    private SheepState _PreviousState;
    private float _ShootingTimer;

    enum SheepState
    {
        Jumping,
        DoubleJumping,
        Running,
        Bounce,
        Damaged
    }

    void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _Inputs = new Queue<EAction>();
    }

	// Use this for initialization
	void Start () 
    {
        _CurrentState = SheepState.Running;
        for (int i = 0; i < InputFramesDelay; i++)
            _Inputs.Enqueue(0);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!IsDead)
        {
            _Velocity = Velocity;
            _TurningAngle = 0;

            var input = _GetAction();
            _Inputs.Enqueue(input);

            _CurrentAction = _Inputs.Dequeue();

            if (_HasFlag(_CurrentAction, EAction.Left))
                _TurningAngle = -TurningSpeed * Time.deltaTime;

            if (_HasFlag(_CurrentAction, EAction.Right))
                _TurningAngle = TurningSpeed * Time.deltaTime;

            switch (_CurrentState)
            {
                case SheepState.Running:
                    _RunningState();
                    break;
                case SheepState.Jumping:
                    _JumpingState();
                    break;
                case SheepState.DoubleJumping:
                    _DoubleJumpingState();
                    break;
                case SheepState.Damaged:
                    _DamagedState();
                    break;
                case SheepState.Bounce:
                    _BounceState();
                    break;
            }

            transform.Translate(_Velocity * Time.deltaTime);
            transform.Rotate(new Vector3(0, 1, 0), _TurningAngle);

            _HandleShooting();
        }
	}

    private void _HandleShooting()
    {
        _ShootingTimer += Time.deltaTime;
        if (_HasFlag(_CurrentAction, EAction.Shoot) && _ShootingTimer >= ShootingDelay)
        {
            _ShootingTimer = 0;
            var projectile = Instantiate(ProjectilePrefab);
            projectile.tag = gameObject.tag;
            projectile.Velocity = transform.forward * ProjectileSpeed;
            projectile.transform.position = transform.position + (transform.forward);
        }
    }

    private static bool _HasFlag(EAction input, EAction flag)
    {
        return (input & flag) == flag;
    }

    private EAction _GetAction()
    {
        EAction action = 0;

        bool inverted = SheepTypes.Contains(SheepType.Inverted);
        
        bool autoLeft = SheepTypes.Contains(SheepType.AutoLeft);
        bool autoRight = SheepTypes.Contains(SheepType.AutoRight);
        bool autoShooter = SheepTypes.Contains(SheepType.AutoShooter);
        bool autoJumper = SheepTypes.Contains(SheepType.AutoJumper);

        bool rotateLeft = SheepTypes.Contains(SheepType.RotatingLeft);
        bool rotateRight = SheepTypes.Contains(SheepType.RotatingRight);
        bool shooter = SheepTypes.Contains(SheepType.Shooter);
        bool jumper = SheepTypes.Contains(SheepType.Jumper);

        if (autoLeft || (rotateLeft && Input.GetKey(KeyCode.LeftArrow)))
            action = action | ( inverted ? EAction.Right : EAction.Left );

        if (autoRight || (rotateRight && Input.GetKey(KeyCode.RightArrow)))
            action = action | (inverted ? EAction.Left : EAction.Right);

        if (autoJumper || (jumper && Input.GetKeyDown(KeyCode.Space)))
            action = action | (inverted ? EAction.Shoot : EAction.Jump);

        if (autoShooter || (shooter && Input.GetKeyDown(KeyCode.LeftShift)))
            action = action | (inverted ? EAction.Jump : EAction.Shoot);

        return action;
    }

    private void _DamagedState()
    {
        _Velocity = Vector3.zero;
        _TurningAngle = 0;
        _CurrentState = SheepState.Running;
    }

    private void _DoubleJumpingState()
    {

    }

    private void _BounceState()
    {
        _TurningAngle = 45;
        _CurrentState = _PreviousState;
    }

    private void _JumpingState()
    {
        if (_HasFlag(_CurrentAction, EAction.Jump))
        {
            _Jump(DoubleJumpForce);
            _CurrentState = SheepState.DoubleJumping;
        }
    }

    private void _RunningState()
    {
        if (_HasFlag(_CurrentAction, EAction.Jump))
        {
            _Jump(JumpForce);
            _CurrentState = SheepState.Jumping;
        }
    }

    private void _Jump(Vector3 force)
    {
        _Rigidbody.velocity = Vector3.zero;
        _Rigidbody.AddForce(force, ForceMode.VelocityChange);
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain")
            _CurrentState = SheepState.Running;

        if (collision.collider.tag == "Fence")
        {
            _PreviousState = _CurrentState;
            _CurrentState = SheepState.Bounce;
        }

        if (collision.collider.tag == "Hazard")
            _CurrentState = SheepState.Damaged;
    }

    public void Kill()
    {
        IsDead = true;
        GetComponentInChildren<Animator>().SetTrigger("Die");
        Destroy(this);
        Destroy(_Rigidbody);
        Destroy(GetComponent<BoxCollider>());
    }
}

[Flags]
public enum EAction
{
    Jump = 0x01,
    Left = 0x02,
    Right = 0x04,
    Shoot = 0x08
}

[Serializable]
public enum SheepType
{
    Inverted,
    AutoLeft,
    RotatingLeft,
    AutoRight,
    RotatingRight,
    AutoJumper,
    Jumper,
    Shooter,
    AutoShooter,
}