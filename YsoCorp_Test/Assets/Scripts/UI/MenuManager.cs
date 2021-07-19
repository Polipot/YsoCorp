using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Références Externes")]
    public Animator Transitions;
    public GameObject Validator;
    public TextMeshProUGUI LevelName;

    [Header("Couleurs des cubes de niveau")]
    public Material LockedMaterial;
    public Material UnlockedMaterial;
    [Space]
    [Header("Couleurs des Liasons")]
    public Material LineLockedMaterial;
    public Material LineUnlockedMaterial;

    [Header("Scene Loader")]
    [HideInInspector] public List<aLevelCube> Levels;
    bool ValidatorOpened = false;
    int NewSceneToLoadIndex;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        Validator.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for(int i = 0; i < Levels.Count; i++)
            {
                if (!Levels[i].UnlockedByDefault)
                {
                    PlayerPrefs.SetInt("Level_" + Levels[i].IndexInBuild, 0);
                    Levels[i].Lock();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit Hit;
            if (Physics.Raycast(MousePosition, transform.forward, out Hit, 10) && Hit.transform.gameObject.name.Contains("aCubeLevel"))
            {
                if (!ValidatorOpened)
                {
                    Hit.transform.gameObject.GetComponent<aLevelCube>().Selected();
                }
                else
                    OpenValidator(false, Vector3.zero);
            }
            else
                OpenValidator(false, Vector3.zero);
        }
    }

    public void OpenValidator(bool Opened, Vector3 OpenPosition, string myLevelName = "", int BuildInIndex = 0)
    {
        if (Opened)
        {
            Validator.SetActive(true);
            Validator.transform.position = OpenPosition;
            LevelName.text = myLevelName;
            NewSceneToLoadIndex = BuildInIndex;

            ValidatorOpened = true;
        }
        else
        {
            Validator.SetActive(false);
            ValidatorOpened = false;
        }
    }

    public void StartOpen() => Transitions.SetTrigger("Load");

    public void LoadNewScene() => SceneManager.LoadScene(NewSceneToLoadIndex);
}
