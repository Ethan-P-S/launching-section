using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager instance;
    [SerializeField] float minX, maxX, minY, maxY;
    [SerializeField] int minTargets, maxTargets, minWalls, maxWalls;
    float count = 3f;

    [SerializeField] Sprite[] numerals;
    [SerializeField] SpriteRenderer[] score, shots, combo;
    Vector3 pos;
    Collider2D[] colid;
    GameObject[] wallObjects;
    LevelData currentLevel;
    int phaseNo;

    [SerializeField] GameObject[] targets;
    [SerializeField] int[] targetWeights;
    //these need to be the same length
    [SerializeField] GameObject[] walls;
    [SerializeField] int[] wallWeights;

    List<int> selection;

    bool updat = false;
    bool upd = false;

    private void Start()
    {
        instance = this;
        colid = new Collider2D[4];
        selection = new List<int>();
    }

    public void GoAlready()
    {
        count = 1.45f;
    }

    void Update()
    {
        count += Time.deltaTime;

        if (count > 1.5f)
        {
            count = 0;
            if (GameObject.FindGameObjectsWithTag("target").Length == 0 && GameObject.FindGameObjectsWithTag("arrow").Length == 0)
            {
                if (GameManager.instance.currentState == GameManager.States.InfiniteGame)
                {
                    SpawnTargets();
                }
                else if (GameManager.instance.currentState == GameManager.States.InLevel)
                {
                    if (currentLevel)
                    {
                        if (phaseNo < currentLevel.Phases.Length)
                        {
                            LoadTargets(currentLevel.Phases[phaseNo]);
                            phaseNo++;
                        }
                        else
                        {
                            GameManager.instance.LevelFinished();
                        }
                    }
                }
            }
        }

        //can't be bothered to implement another way of delaying a frame
        if (upd)
        {
            updat = true;
            upd = false;
        }
        else if (updat)
        {
            ComboUpdate();
            updat = false;
        }
    }

    public bool TargetsExist()
    {
        if (GameObject.FindGameObjectsWithTag("target").Length == 0 && launcher.instance.CanLaunch())
        {
            return false;
        }
        else return true;
    }

    public wall.RectWithReference[] MovingTargetBoxes()
    {
        wallObjects = GameObject.FindGameObjectsWithTag("wall");
        int movingCount = 0;
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (wallObjects[i].GetComponent<wall>().IsMobile())
            {
                movingCount++;
            }
        }
        wall.RectWithReference[] targetBoxes = new wall.RectWithReference[movingCount];
        movingCount = 0;
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (wallObjects[i].GetComponent<wall>().IsMobile())
            {
                targetBoxes[movingCount] = wallObjects[i].GetComponent<wall>().MovementBox();
                movingCount++;
            }
        }
        return targetBoxes;
    }
    public wall.RectWithReference[] OverlappingTargetZones(Vector2 point)
    {
        wall.RectWithReference[] targetBoxes = MovingTargetBoxes();
        int overlapCount = 0;
        for (int i = 0; i < targetBoxes.Length; i++)
        {
            if (targetBoxes[i].theRect.Contains(point))
            {
                overlapCount++;
            }
        }
        wall.RectWithReference[] overlaps = new wall.RectWithReference[overlapCount];
        overlapCount = 0;
        for (int i = 0; i < targetBoxes.Length; i++)
        {
            if (targetBoxes[i].theRect.Contains(point))
            {
                overlaps[overlapCount] = targetBoxes[i];
                overlapCount++;
            }
        }
        return overlaps;
    }

    void SpawnTargets()
    {
        int targetAmount = Random.Range(minTargets, maxTargets + 1);
        int wallAmount = Random.Range(minWalls, maxWalls + 1);
        for (int i = 0; i < targetAmount; i++)
        {
            selection.Clear();
            for (int j = 0; j < targets.Length; j++)
            {
                for (int k = 0; k < targetWeights[j]; k++)
                {
                    selection.Add(j);
                }
            }

            GameObject spawnObject = targets[selection[Random.Range(0, selection.Count)]];

            pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            GameObject theObject = Instantiate(spawnObject, pos, Quaternion.identity);
            if (Physics2D.OverlapBoxNonAlloc(pos, Vector2.one, 0, colid) > 1)
            {
                pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
                theObject.transform.position = pos;
            }
            theObject.name += " " + i.ToString();
        }

        wallObjects = GameObject.FindGameObjectsWithTag("wall");
        for (int i = 0; i < wallObjects.Length; i++)
        {
            Destroy(wallObjects[i]);
        }

        for (int i = 0; i < wallAmount; i++)
        {
            selection.Clear();
            for (int j = 0; j < walls.Length; j++)
            {
                for (int k = 0; k < wallWeights[j]; k++)
                {
                    selection.Add(j);
                }
            }

            GameObject spawnObject = walls[selection[Random.Range(0, selection.Count)]];

            pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            GameObject theObject = Instantiate(spawnObject, pos, Quaternion.identity);
            if (Physics2D.OverlapBoxNonAlloc(pos, Vector2.one, 0, colid) > 1)
            {
                pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
                theObject.transform.position = pos;
            }
        }
    }

    public void LoadTargets(PhaseData theLevel)
    {
        GameManager.instance.CleanUp();

        for (int i = 0; i < theLevel.TargetData.Length; i++)
        {
            Instantiate(targets[Mathf.RoundToInt(theLevel.TargetData[i].x)], new Vector2(theLevel.TargetData[i].y, theLevel.TargetData[i].z), Quaternion.identity);
        }

        for (int i = 0; i < theLevel.WallData.Length; i++)
        {
            Instantiate(walls[Mathf.RoundToInt(theLevel.WallData[i].x)], new Vector2(theLevel.WallData[i].y, theLevel.WallData[i].z), Quaternion.identity);
        }
    }

    public void LoadLevel(LevelData level)
    {
        count = 2f;
        currentLevel = level;
        phaseNo = 0;
        launcher.instance.ResetSpecial();
    }

    public void ResetLevel()
    {
        count = 2f;
        scoring.ResetScore();
        phaseNo = 0;
        launcher.instance.ResetSpecial();
        LoadTargets(currentLevel.Phases[0]);
        UpdateDisplay();
        launcher.instance.SetCanLaunch(true);
    }

    public void UpdateDisplay()
    {
        string scoreString = scoring.score.ToString();
        string shotString = scoring.shots.ToString();

        WriteDisplay(GameManager.instance.scoreDisplay(), scoreString);
        WriteDisplay(GameManager.instance.shotsDisplay(), shotString);

        UpdateCombos();
    }

    void WriteDisplay(SpriteRenderer[] renderers, string theString)
    {
        string s = "";
        int num;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sprite = null;
        }

        for (int i = 0; i < renderers.Length && i < theString.Length; i++)
        {
            s += theString[theString.Length - (i + 1)];
            num = int.Parse(s);
            renderers[i].sprite = numerals[num];
            s = "";
        }
    }

    void WriteDisplay(Image[] renderers, string theString)
    {
        string s = "";
        int num;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }

        for (int i = 0; i < renderers.Length && i < theString.Length; i++)
        {
            s += theString[theString.Length - (i + 1)];
            num = int.Parse(s);
            if (num > 999999)
            {
                num = 999999;
            }
            renderers[i].enabled = true;
            renderers[i].sprite = numerals[num];
            s = "";
        }
    }

    public void UpdateCombos()
    {
        upd = true;
    }

    void ComboUpdate()
    {
        int combo = Mathf.RoundToInt(scoring.ComboMultiplier() * 10f);
        Image[] writeToThis = new Image[] { GameManager.instance.comboDisplay()[0], GameManager.instance.comboDisplay()[1], GameManager.instance.comboDisplay()[2] };
        WriteDisplay(writeToThis, combo.ToString());
        RectTransform r = GameManager.instance.comboDisplay()[3].gameObject.GetComponent<RectTransform>();
        if(combo > 100)
        {
            r.localPosition = new Vector3(-375, r.localPosition.y, r.localPosition.z);
        }
        else
        {
            r.localPosition = new Vector3(-275, r.localPosition.y, r.localPosition.z);
        }
    }
}
