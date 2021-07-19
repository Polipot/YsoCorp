using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aLevelCube : MonoBehaviour
{
    [Header("Références Internes")]
    Animator myAnimator;
    MeshRenderer myMeshRenderer;

    [Header("Références Externes")]
    MenuManager MM;
    public List<MeshRenderer> DependentLines;

    [Header("Immobile")]
    public string LevelName;
    public int IndexInBuild;
    [Header("Mobile")]
    public bool UnlockedByDefault;
    [HideInInspector]public bool Unlocked = false;

    private void Awake()
    {
        MM = MenuManager.Instance;
        myMeshRenderer = GetComponent<MeshRenderer>();
        MM.Levels.Add(this);
        Actualize();
    }

    void Actualize()
    {
        myAnimator = GetComponent<Animator>();

        if (!UnlockedByDefault)
        {
            int IsUnlocked = PlayerPrefs.GetInt("Level_" + IndexInBuild);
            if (IsUnlocked == 1)
                Unlock();
        }

        else
            Unlock();
    }

    void Unlock()
    {
        myMeshRenderer.material = MM.UnlockedMaterial;
        for(int i = 0; i < DependentLines.Count; i++)
        {
            DependentLines[i].material = MM.LineUnlockedMaterial;
        }
        Unlocked = true;
    }

    public void Selected()
    {
        if (Unlocked)
        {
            myAnimator.SetTrigger("Interaction");
            Vector3 PositionToScreen = Camera.main.WorldToScreenPoint(transform.position);
            MM.OpenValidator(true, PositionToScreen, LevelName, IndexInBuild);
        }
    }

    //Only for editing
    public void Lock()
    {
        myMeshRenderer.material = MM.LockedMaterial;
        for (int i = 0; i < DependentLines.Count; i++)
        {
            DependentLines[i].material = MM.LineLockedMaterial;
        }
        Unlocked = false;
    }
}
