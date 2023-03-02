using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum States { MainMenu, InfiniteGame, InLevel, SubMenu, levelMenu }
    public States currentState { get; private set; }
    public static GameManager instance; int lastLevelEntered;
    [SerializeField] MenuButton[] levelSelectors, finishedLevel;
    [SerializeField] GameObject resetLevel;
    [SerializeField] GameObject[] MENU, submenu;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] BGM;

    GameObject[] objects; //for nonAlloc purposes

    [SerializeField] GameObject[] titleObjects, gameDisplayObjects, comboObjects;
    [SerializeField] GameObject the_launcher; Image launcher_image;
    Image[] titleImages, displayImages, comboImages;
    [SerializeField] SpriteRenderer reticleRender;

    public Image[] scoreDisplay()
    {
        return new Image[] { displayImages[5], displayImages[4], displayImages[3], displayImages[2], displayImages[1], displayImages[0] };
    }
    public Image[] shotsDisplay()
    {
        return new Image[] { displayImages[10], displayImages[9], displayImages[8], displayImages[7], displayImages[6] };
    }
    public Image[] comboDisplay()
    {
        return new Image[] { comboImages[3], comboImages[2], comboImages[1], comboImages[0] };
    }

    public void AdjustLauncherImage(float theFloat)
    {
        the_launcher.transform.position = new Vector3(the_launcher.transform.position.x, Mathf.Lerp(UI_Lerping.instance.Lerp_Points()[6].position.y, UI_Lerping.instance.Lerp_Points()[7].position.y, theFloat),the_launcher.transform.position.z);
    }

    public Vector3 GetLauncherPosition()
    {
        return the_launcher.transform.position;
    }

    void Start()
    {
        titleImages = new Image[titleObjects.Length];
        for (int i = 0; i < titleObjects.Length; i++)
        {
            titleImages[i] = titleObjects[i].GetComponent<Image>();
        }

        displayImages = new Image[gameDisplayObjects.Length];
        for (int i = 0; i < gameDisplayObjects.Length; i++)
        {
            displayImages[i] = gameDisplayObjects[i].GetComponent<Image>();
        }

        comboImages = new Image[comboObjects.Length];
        for (int i = 0; i < comboObjects.Length; i++)
        {
            comboImages[i] = comboObjects[i].GetComponent<Image>();
        }

        launcher_image = the_launcher.GetComponent<Image>();

        instance = this;
        currentState = States.MainMenu;
    }

    public void GoToMenu()
    {
        currentState = States.MainMenu;
        for (int i = 0; i < displayImages.Length; i++)
        {
            displayImages[i].enabled = false;
        }
        reticleRender.enabled = false;
        for (int i = 0; i < MENU.Length; i++)
        {
            MENU[i].SetActive(true);
        }
        for (int i = 0; i < submenu.Length; i++)
        {
            submenu[i].SetActive(false);
        }
        for (int i = 0; i < levelSelectors.Length; i++)
        {
            levelSelectors[i].Deactivate();
        }
        for (int i = 0; i < finishedLevel.Length; i++)
        {
            finishedLevel[i].Deactivate();
        }
        titleImages[0].enabled = true;
        resetLevel.SetActive(false);

        objects = GameObject.FindGameObjectsWithTag("debugvertex");
        if (objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Destroy(objects[i]);
            }
        }

        launcher_image.enabled = false;
        aud.Stop();
        aud.clip = BGM[1];
        aud.Play();
        Cursor.visible = true;
        CleanUp();
        scoring.ResetScore();
    }

    public void PlayLevel()
    {
        if (currentState == States.SubMenu)
        {
            currentState = States.levelMenu;
            for (int i = 0; i < submenu.Length; i++)
            {
                submenu[i].SetActive(false);
            }
            for (int i = 0; i < levelSelectors.Length; i++)
            {
                levelSelectors[i].Activate();
            }
            submenu[1].SetActive(true);
        }
    }

    public void PlayInfinite()
    {
        if (currentState == States.SubMenu)
        {
            currentState = States.InfiniteGame;
            for (int i = 0; i < displayImages.Length; i++)
            {
                displayImages[i].enabled = true;
            }
            reticleRender.enabled = true;
            for (int i = 0; i < MENU.Length; i++)
            {
                MENU[i].SetActive(false);
            }
            for (int i = 0; i < submenu.Length; i++)
            {
                submenu[i].SetActive(false);
            }
            scoring.RESET_MULT();
            aud.Stop();
            aud.clip = BGM[0];
            aud.Play();
            Cursor.visible = false;
            TargetsManager.instance.UpdateDisplay();
            TargetsManager.instance.GoAlready();
            submenu[1].SetActive(true);
            launcher.instance.SetCanLaunch(true);
            launcher_image.enabled = true;

            float t = UI_Lerping.instance.LerpTheX(UI_Lerping.instance.Lerp_Points()[0], UI_Lerping.instance.Lerp_Points()[1], the_launcher.transform);
            launcher.instance.gameObject.transform.position = new Vector3(Mathf.Lerp(UI_Lerping.instance.Lerp_Points()[3].position.x, UI_Lerping.instance.Lerp_Points()[4].position.x, t), launcher.instance.transform.position.y, launcher.instance.transform.position.z);
        }
    }

    public void LevelSelect(int index)
    {
        if (currentState == States.levelMenu)
        {
            launcher.instance.SetCanLaunch(true);
            allTheLevels.instance.LoadALevel(index);
            currentState = States.InLevel;
            for (int i = 0; i < displayImages.Length; i++)
            {
                displayImages[i].enabled = true;
            }
            reticleRender.enabled = true;
            lastLevelEntered = index;
            for (int i = 0; i < levelSelectors.Length; i++)
            {
                levelSelectors[i].Deactivate();
            }
            TargetsManager.instance.UpdateDisplay();
            resetLevel.SetActive(true);
            scoring.RESET_MULT();
            launcher_image.enabled = true;

            float t = UI_Lerping.instance.LerpTheX(UI_Lerping.instance.Lerp_Points()[0], UI_Lerping.instance.Lerp_Points()[1], the_launcher.transform);
            launcher.instance.gameObject.transform.position = new Vector3(Mathf.Lerp(UI_Lerping.instance.Lerp_Points()[3].position.x, UI_Lerping.instance.Lerp_Points()[4].position.x, t), launcher.instance.transform.position.y, launcher.instance.transform.position.z);
        }
    }

    public void LevelFinished()
    {
        for (int i = 0; i < finishedLevel.Length; i++)
        {
            finishedLevel[i].Activate();
        }
        if (!(allTheLevels.instance.LEVELS.Length > lastLevelEntered + 1))
        {
            finishedLevel[2].Deactivate();
        }
        CleanUp();
        for (int i = 0; i < displayImages.Length; i++)
        {
            displayImages[i].enabled = false;
        }
        reticleRender.enabled = false;
        submenu[1].SetActive(false);
        resetLevel.SetActive(false);
        launcher_image.enabled = false;
    }

    public void NextLevel()
    {
        if (allTheLevels.instance.LEVELS.Length > lastLevelEntered + 1)
        {
            GoToMenu(); PlayPressed(); PlayLevel();
            LevelSelect(lastLevelEntered + 1);
        }
    }

    public void PlayPressed()
    {
        if (currentState == States.MainMenu)
        {
            titleImages[0].enabled = false;
            currentState = States.SubMenu;
            for (int i = 0; i < MENU.Length; i++)
            {
                MENU[i].SetActive(false);
            }
            for (int i = 0; i < submenu.Length; i++)
            {
                submenu[i].SetActive(true);
            }
        }
    }

    public void CleanUp()
    {
        CleanThisUP("arrow");
        CleanThisUP("target");
        CleanThisUP("wall");
        CleanThisUP("projection");
    }

    public void CleanThisUP(string tag)
    {
        objects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }
    }
}
