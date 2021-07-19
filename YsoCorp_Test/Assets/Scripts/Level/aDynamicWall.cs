using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aDynamicWall : MonoBehaviour
{
    [Header("Références Externes")]
    DynamicWallManager DWM;
    PlayerControls PC;

    [Header("Références Internes")]
    BoxCollider myBoxCollider;
    Animator myAnimator;

    public bool HasPlayerHooked = false;

    void Awake()
    {
        DWM = DynamicWallManager.Instance;
        PC = PlayerControls.Instance;

        myBoxCollider = GetComponent<BoxCollider>();
        myAnimator = GetComponentInChildren<Animator>();

        DWM.DynamicWalls.Add(this);
    }

    public void ChangeActivation(bool Activate)
    {
        myBoxCollider.enabled = Activate;
        myAnimator.SetTrigger("Change");

        if (!Activate && HasPlayerHooked)
        {
            HasPlayerHooked = false;
            PC.Launch(Vector3.zero);
        }
    }
}
