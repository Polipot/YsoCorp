using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Références externes")]
    PlayerControls PC;

    [Header("Références internes")]
    Animator myAnimatorPlayer;
    Animator myAnimatorOutside;

    [Header("Général")]
    public float smoothFollowSpeed;
    public float smoothZoomSpeed;
    [HideInInspector]
    public Transform Target;

    Vector3 StartPosition;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        PC = PlayerControls.Instance;

        myAnimatorOutside = transform.GetChild(0).GetComponent<Animator>();
        myAnimatorPlayer = myAnimatorOutside.transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Target)
        {
            Vector3 TargetPosition = new Vector3(Target.position.x, Target.position.y, transform.position.z);

            float DesiredZoom = -10f;
            DesiredZoom -= Vector3.Distance(transform.position, TargetPosition);
            DesiredZoom -= transform.position.z;

            if (PC.PS == PlayerSituation.Hooked && PC.JumpForce != 0)
            {
                DesiredZoom -= PC.JumpForce * 3;               
            } 

            if (Vector3.Distance(transform.position, TargetPosition) > 0.1f || DesiredZoom != 0)
            {
                transform.position += (TargetPosition - transform.position) * smoothFollowSpeed + new Vector3(0,0,DesiredZoom) * smoothFollowSpeed;
            }
        }
    }

    public void LaunchShake(float Magnitude)
    {
        myAnimatorPlayer.SetFloat("LaunchMagnitude", Magnitude);
        myAnimatorPlayer.SetTrigger("Launch");
    }

    public void PrepareShake(float Magnitude, bool Start = false)
    {
        myAnimatorPlayer.SetFloat("PrepareMagnitude", Magnitude);
        if (Start)
            myAnimatorPlayer.SetTrigger("Prepare");
    }

    public void ExteriorShake() => myAnimatorOutside.SetTrigger("Shake");

    public void Respawn()
    {
        transform.position = StartPosition;
    }
}
