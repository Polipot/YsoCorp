using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    GameManager GM;
    MenuManager MM;

    // Start is called before the first frame update
    void Awake()
    {
        GM = GameManager.Instance;
        MM = MenuManager.Instance;
    }

    public void Respawn()
    {
        if (GM)
            GM.Retry();
    }

    public void LoadNewScene()
    {
        if (GM)
            GM.LoadNewScene();
        else if (MM)
            MM.LoadNewScene();
    }
}
