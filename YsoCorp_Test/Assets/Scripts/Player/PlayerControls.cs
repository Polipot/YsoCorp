using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PlayerSituation { Hooked, Launched, Dead, Win, Respawn };

public class PlayerControls : Singleton<PlayerControls>
{
    [Header("Références externes")]
    CameraManager CM;
    GameManager GM;

    [Header("Références internes")]
    Rigidbody myRb;
    MeshRenderer myMR;

    [Header("Global"), HideInInspector]
    public PlayerSituation PS = PlayerSituation.Launched;
    Vector3 StartPosition;

    [Header("Controle de la physique")]
    public AnimationCurve ForceEvolution;
    public AnimationCurve GravityEvolution;
    float PhysicTime;
    [HideInInspector] public List<aDynamicWall> HookedDynamicWalls;

    [Header("Dernier saut")]
    Vector3 JumpDirection;
    [HideInInspector]public float JumpForce;

    Vector2 TouchPointDown;

    [Header("Latence avant prochain accrochage")]
    bool isLaunching;
    float LatencyHook;

    [Header("FX")]
    public ParticleSystem JumpFX;
    public ParticleSystem DeathFX;
    public ParticleSystem VictoryFX;

    [Header("UI Feedbacks")]
    public GameObject DirectionArrow;

    [Header("Respawn")]
    float LatencyBeforePlay;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        CM = CameraManager.Instance;
        GM = GameManager.Instance;

        myRb = GetComponent<Rigidbody>();
        myMR = GetComponentInChildren<MeshRenderer>();
    }

    private void Start()
    {
        CM.Target = transform;
        StartPosition = transform.position;
        DirectionArrow.gameObject.SetActive(false);
    }

    void Update()
    {
        if(PS == PlayerSituation.Hooked && !EventSystem.current.IsPointerOverGameObject())
        {
            myRb.velocity = new Vector3(0, 0, 0);

            if (Input.GetMouseButtonDown(0))
            {
                TouchPointDown = Input.mousePosition;
                CM.PrepareShake(0, true);
                DirectionArrow.gameObject.SetActive(true);
                DirectionArrow.transform.localScale = new Vector3(0, 0, 0);

                Vector3 PointOnScreen = Camera.main.WorldToScreenPoint(transform.position);
                DirectionArrow.transform.position = PointOnScreen;
            }

            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 TouchPointUp = Input.mousePosition;
                Vector3 Dir = TouchPointDown - TouchPointUp;
                DirectionArrow.gameObject.SetActive(false);

                Launch(Dir);
            }

            else if (Input.GetMouseButton(0))
            {
                Vector2 TouchPointUp = Input.mousePosition;
                Vector3 Dir = TouchPointDown - TouchPointUp;

                JumpForce = Dir.magnitude / 500;
                JumpForce = Mathf.Clamp(JumpForce, 0, 1);


                Vector3 PointOnScreen = Camera.main.WorldToScreenPoint(transform.position);
                DirectionArrow.transform.position = PointOnScreen;
                DirectionArrow.transform.LookAt(DirectionArrow.transform.position + Dir.normalized);
                DirectionArrow.transform.localScale = new Vector3(JumpForce * 10, JumpForce * 10, JumpForce * 10);

                float angle = 0;

                Vector3 relative = DirectionArrow.transform.InverseTransformPoint(Dir);
                angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                DirectionArrow.transform.Rotate(0, 0, -angle);

                CM.PrepareShake(JumpForce);
            }
        }

        else if(PS == PlayerSituation.Launched)
        {
            if (isLaunching)
            {
                LatencyHook += Time.deltaTime;
                if (LatencyHook > 0.02f)
                {
                    LatencyHook = 0;
                    isLaunching = false;
                }
            }
        }

        else if (PS == PlayerSituation.Respawn)
        {
            LatencyBeforePlay += Time.deltaTime;
            if(LatencyBeforePlay > 0.5f)
            {
                LatencyBeforePlay = 0;
                Hooking();
            }
        }

    }

    private void FixedUpdate()
    {
        if (PS == PlayerSituation.Launched)
            Fly();
    }

    #region Physic

    void Fly()
    {
        Vector3 Force = JumpDirection * ForceEvolution.Evaluate(PhysicTime) * 40 * JumpForce;
        Vector3 Gravity = -transform.up * GravityEvolution.Evaluate(PhysicTime) * 20;

        Vector3 TotalForce = Force + Gravity;

        if (PhysicTime < 2)
        {
            PhysicTime += Time.deltaTime;
        }

        myRb.velocity = TotalForce;

        if (transform.position.y <= -20)
            Death();
    }

    public void Launch(Vector3 Force)
    {
        isLaunching = true;

        JumpDirection = Force.normalized;
        JumpForce = Force.magnitude / 500;
        JumpForce = Mathf.Clamp(JumpForce, 0, 1);

        PS = PlayerSituation.Launched;
        CM.LaunchShake(JumpForce);

        if(Force != Vector3.zero)
        {
            JumpFX.gameObject.transform.position = transform.position;
            JumpFX.Play();
        }

        if (HookedDynamicWalls.Count > 0)
        {
            DirectionArrow.gameObject.SetActive(false);

            for (int i = 0; i < HookedDynamicWalls.Count; i++)
                HookedDynamicWalls[i].HasPlayerHooked = false;

            HookedDynamicWalls.Clear();
        }
    }

    void Hooking()
    {
        PhysicTime = 0;
        JumpForce = 0;
        myRb.velocity = new Vector3(0, 0, 0);

        PS = PlayerSituation.Hooked;
    }

    #endregion

    #region Victory, Death and Respawn

    public void Death()
    {
        PS = PlayerSituation.Dead;
        myRb.velocity = new Vector3(0, 0, 0);

        CM.LaunchShake(1);

        myMR.enabled = false;
        DeathFX.gameObject.transform.position = transform.position;
        DeathFX.Play();

        GM.Verdict(false);

        DirectionArrow.gameObject.SetActive(false);
    }

    void Victory()
    {
        PS = PlayerSituation.Win;
        myRb.velocity = new Vector3(0, 0, 0);

        CM.LaunchShake(1);

        myMR.enabled = false;
        VictoryFX.gameObject.transform.position = transform.position;
        VictoryFX.Play();

        GM.Verdict(true);
    }

    public void Respawn()
    {
        transform.position = StartPosition;
        myMR.enabled = true;
        PS = PlayerSituation.Respawn;
    }

    #endregion

    private void OnCollisionStay(Collision collision)
    {
        if (PS == PlayerSituation.Launched && collision.gameObject.tag.Contains("Wall") && !isLaunching)
        {
            Hooking();
            if(collision.gameObject.tag == "DynamicWall")
            {
                aDynamicWall newDWHooked = collision.gameObject.GetComponent<aDynamicWall>();
                HookedDynamicWalls.Add(newDWHooked);
                newDWHooked.HasPlayerHooked = true;
            }
        }

        else if(collision.gameObject.tag == "Killer" && PS != PlayerSituation.Dead)
        {
            Death();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(PS == PlayerSituation.Launched && other.gameObject.tag == "Victory")
        {
            Victory();
        }
    }
}
