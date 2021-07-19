using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWallManager : Singleton<DynamicWallManager>
{
    public float ChangeLatency;
    float ChangeTime;

    [HideInInspector]
    public List<aDynamicWall> DynamicWalls;
    bool On = true;
    bool ManagerActive => DynamicWalls.Count > 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ManagerActive)
        {
            ChangeTime += Time.fixedDeltaTime;
            if(ChangeTime >= ChangeLatency)
            {
                ChangeTime = 0;
                ChangeWallMode();
            }
        }
    }

    void ChangeWallMode()
    {
        On = !On;

        for (int i = 0; i < DynamicWalls.Count; i++)
        {
            DynamicWalls[i].ChangeActivation(On);
        }
    }
}
