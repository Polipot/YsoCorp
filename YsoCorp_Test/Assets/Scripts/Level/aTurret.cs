using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretSituation { Reloading, Locking, Seeking };

public class aTurret : MonoBehaviour
{
    [Header("Références Externes")]
    PlayerControls PC;
    CameraManager CM;
    Transform PlayerTransform;

    [Header("Références Internes")]
    public Transform Canon;
    [HideInInspector]public aProjectile myProjectile;
    Animator myWarning;

    [Header("Station de tir")]
    TurretSituation mySituation;
    [Space]
    public float FireLatency;
    float FireTime;
    bool ReadyToFire = false;
    [Space]
    public float LockLatency;
    float LockTime;

    // Start is called before the first frame update
    void Awake()
    {
        PC = PlayerControls.Instance;
        CM = CameraManager.Instance;
        PlayerTransform = PC.transform;

        myWarning = transform.GetChild(2).GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(PC.PS == PlayerSituation.Hooked || PC.PS == PlayerSituation.Launched)
        {
            if (Visible())
            {
                LookPlayer();
                if (ReadyToFire)
                    Locking();
            }

            else if (mySituation == TurretSituation.Locking)
                ResetLocking();
        }

        if (!ReadyToFire)
        {
            Reload();
        }
    }

    bool Visible()
    {
        RaycastHit Hit;
        Vector3 Diretion = (PC.transform.position - transform.position).normalized;
        float Distance = Vector3.Distance(PC.transform.position, transform.position);
        if (Physics.Raycast(transform.position, Diretion, out Hit, Distance) && Hit.collider.gameObject.tag == "Player")
            return true;
        else
            return false;
    }

    void LookPlayer()
    {
        Vector3 targetPos = new Vector3(PlayerTransform.position.x, transform.position.y, transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(PlayerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
    }

    void Reload()
    {
        FireTime += Time.deltaTime;
        if(FireTime >= FireLatency)
        {
            ReadyToFire = true;
            mySituation = TurretSituation.Seeking;
            FireTime = 0;
        }
    }

    void Locking()
    {
        if(mySituation != TurretSituation.Locking)
        {
            LockTime = 0;
            mySituation = TurretSituation.Locking;
            myWarning.SetTrigger("Warning");
        }

        LockTime += Time.deltaTime;
        if (LockTime >= LockLatency)
        {
            mySituation = TurretSituation.Reloading;
            LockTime = 0;
            Fire();
        }
    }

    void ResetLocking()
    {
        mySituation = TurretSituation.Seeking;
        LockTime = 0;
        myWarning.SetTrigger("StopWarning");      
    }

    void Fire()
    {
        ReadyToFire = false;
        mySituation = TurretSituation.Reloading;
        myProjectile.Fire(Canon);

        CM.ExteriorShake();
    }

    void Reactivate()
    {
        FireTime = 0;
        ReadyToFire = false;
        mySituation = TurretSituation.Reloading;
    }
}
