using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aProjectile : MonoBehaviour
{
    [Header("Références externes")]
    PlayerControls PC;

    [Header("Références internes")]
    Rigidbody myRigidbody;
    BoxCollider myBoxCollider;
    MeshRenderer myMeshRenderer;
    ParticleSystem myShootParticle;

    [Header("Global")]
    [HideInInspector] public bool Launched = false;
    public float Speed;

    [Header("Tourelle mère")]
    aTurret myMotherTurret;


    // Start is called before the first frame update
    void Awake()
    {
        PC = PlayerControls.Instance;

        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        myMeshRenderer = GetComponent<MeshRenderer>();

        myMotherTurret = GetComponentInParent<aTurret>();
        myMotherTurret.myProjectile = this;

        myShootParticle = GetComponentInChildren<ParticleSystem>();

        myBoxCollider.enabled = false;
        myMeshRenderer.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Launched)
            myRigidbody.velocity = transform.forward * Speed;
        else
            myRigidbody.velocity = Vector3.zero;
    }

    public void Fire(Transform Canon)
    {
        transform.parent = null;
        transform.rotation = myMotherTurret.transform.rotation;
        transform.position = Canon.transform.position;

        myBoxCollider.enabled = true;
        myMeshRenderer.enabled = true;

        Launched = true;
    }

    void Destruction()
    {
        Launched = false;

        myBoxCollider.enabled = false;
        myMeshRenderer.enabled = false;

        myShootParticle.gameObject.transform.parent = null;
        myShootParticle.gameObject.transform.eulerAngles = Vector3.zero;
        myShootParticle.gameObject.transform.position = transform.position;
        myShootParticle.Play();

        transform.parent = myMotherTurret.transform;
        transform.position = myMotherTurret.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            PC.Death();
            Destruction();
        }

        if(collision.gameObject != myMotherTurret.gameObject)
            Destruction();
    }
}
