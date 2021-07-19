using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Références Externes")]
    PlayerControls PC;
    CameraManager CM;
    public TextMeshProUGUI LevelPresentation;

    [Header("Groupe de Verdict")]
    public GameObject VerdictGroup;
    TextMeshProUGUI VerdictText;
    GameObject NextStage_Button;
    GameObject Retry_Button;
    GameObject Menu_Button;

    [Header("Transitions")]
    public Animator Transitions;
    int NewSceneIndexToLoad;

    [Header("Place du niveau dans le jeu")]
    public int NextLevelIndex;
    public string LevelName;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        PC = PlayerControls.Instance;
        CM = CameraManager.Instance;
    }

    private void Start()
    {
        VerdictText = VerdictGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        NextStage_Button = VerdictGroup.transform.GetChild(1).GetChild(0).gameObject;
        Retry_Button = VerdictGroup.transform.GetChild(1).GetChild(1).gameObject;
        Menu_Button = VerdictGroup.transform.GetChild(1).GetChild(2).gameObject;

        VerdictGroup.SetActive(false);

        LevelPresentation.text = "<i>" + LevelName + "</i>\n" + "Level " + SceneManager.GetActiveScene().buildIndex;
    }

    public void Verdict(bool Victory)
    {
        VerdictGroup.SetActive(true);

        if (Victory)
        {
            VerdictText.color = Color.green;
            VerdictText.text = "You have WON";

            Retry_Button.gameObject.SetActive(false);
            if(NextLevelIndex > 0)
            {
                PlayerPrefs.SetInt("Level_" + NextLevelIndex, 1);
                NextStage_Button.gameObject.SetActive(true);
            } 
            else
                NextStage_Button.gameObject.SetActive(false);
        }
        else
        {
            VerdictText.color = Color.red;
            VerdictText.text = "You are DEAD";

            Retry_Button.gameObject.SetActive(true);
            NextStage_Button.gameObject.SetActive(false);
        }
    }

    public void StartRetry()
    {
        Transitions.SetTrigger("Retry");
    }

    public void Retry()
    {
        PC.Respawn();
        CM.Respawn();
        VerdictGroup.SetActive(false);
    }

    ///

    public void StartLoad(string NextScene)
    {
        Transitions.SetTrigger("Load");
        if (NextScene == "NextStage")
            NewSceneIndexToLoad = NextLevelIndex;
        else if (NextScene == "Menu")
            NewSceneIndexToLoad = 0;
    }

    public void LoadNewScene()
    {
        SceneManager.LoadScene(NewSceneIndexToLoad);
    }
}
