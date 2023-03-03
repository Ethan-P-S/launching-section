using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class launcher : MonoBehaviour
{
    public static launcher instance;
    Camera c;
    Vector3 mousePos;
    projectile thePro;

    float[] equationVariables;
    float[] parabolaTerms;

    [SerializeField] Transform mouseAnchor;
    [SerializeField] GameObject LAUNCH_IMAGE, POWERUP_IMAGE, launched; Image theImage, power_up;
    [SerializeField] Sprite[] powerupSprites;
    [SerializeField] float elapseTime;
    float baseElapse;
    [SerializeField] Sprite readySprite, emptySprite;

    int specialLaunch;

    bool canlaunch = true;
    float set_count = -1f;
    bool set_set;

    void Start()
    {
        baseElapse = elapseTime;
        instance = this;
        equationVariables = new float[5];
        parabolaTerms = new float[5];
        c = Camera.main;
        theImage = LAUNCH_IMAGE.GetComponent<Image>();
        power_up = POWERUP_IMAGE.GetComponent<Image>();
    }

    public bool CanLaunch()
    {
        if (GameObject.FindGameObjectsWithTag("arrow").Length > 0)
        {
            return false;
        }

        else
        {
            return true;
        }
    }

    public void SetCanLaunch(bool value)
    {
        set_count = 0.05f;
        set_set = value;
    }

    void Update()
    {
        if (GameManager.instance.currentState == GameManager.States.InfiniteGame || GameManager.instance.currentState == GameManager.States.InLevel)
        {

            mousePos = c.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -2;
            mouseAnchor.position = mousePos;

            if (mousePos.x > transform.position.x)
            {
                if (!canlaunch)
                {
                    theImage.sprite = emptySprite;
                }
                else if (canlaunch)
                {
                    LAUNCH_IMAGE.transform.rotation = Quaternion.identity;
                    theImage.sprite = readySprite;
                    Parabola(transform.position, mouseAnchor.position);
                    LAUNCH_IMAGE.transform.Rotate(0, 0, Mathf.Asin((mouseAnchor.position.y - transform.position.y) / Vector2.Distance(transform.position, mouseAnchor.position)) * Mathf.Rad2Deg);

                    if (Input.GetKeyDown(KeyCode.Mouse0) && TargetsManager.instance.TargetsExist())//refactor input later
                    {
                        DoTheLaunch();
                    }
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (transform.position.y < 4)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 2, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, 4, transform.position.z);
                }
            }
            //maybe implement a sensitivity option here later
            //and control rebinding
            else if (Input.GetKey(KeyCode.S))
            {
                if (transform.position.y > -4)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 2, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, -4, transform.position.z);
                }
            }

            GameManager.instance.AdjustLauncherImage(UI_Lerping.instance.LerpTheY(UI_Lerping.instance.Lerp_Points()[8], UI_Lerping.instance.Lerp_Points()[9], transform));
            POWERUP_IMAGE.transform.position = GameManager.instance.GetLauncherPosition() + new Vector3(0, 75);

            if (set_count > 0)
            {
                set_count -= Time.deltaTime;
                if (set_count < 0 && GameObject.FindGameObjectWithTag("arrow") == null)
                {
                    set_count = -1;
                    canlaunch = set_set;
                }
            }

        }
    }

    void DoTheLaunch()
    {
        GameManager.instance.CleanThisUP("printing");
        scoring.somethingWasHit = false;

        canlaunch = false;

        if (specialLaunch == 0)
        {
            LaunchProjectile(mouseAnchor.position, 0);
        }
        else
        {
            switch (specialLaunch)
            {
                case 1:
                    {
                        LaunchProjectile(mouseAnchor.position, 0);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(0, 1));
                        LaunchProjectile(mouseAnchor.position + new Vector3(0, 1), 1);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(0, -1));
                        LaunchProjectile(mouseAnchor.position + new Vector3(0, -1), 2);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(-0.5f, -0.5f));
                        LaunchProjectile(mouseAnchor.position + new Vector3(-0.5f, -0.5f), 3);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(0.5f, -0.5f));
                        LaunchProjectile(mouseAnchor.position + new Vector3(0.5f, -0.5f), 4);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(-0.5f, 0.5f));
                        LaunchProjectile(mouseAnchor.position + new Vector3(-0.5f, 0.5f), 5);

                        Parabola(transform.position, mouseAnchor.position + new Vector3(0.5f, 0.5f));
                        LaunchProjectile(mouseAnchor.position + new Vector3(0.5f, 0.5f), 6);
                    }
                    break;
                case 2:
                    {
                        elapseTime = 2.5f;
                        LaunchProjectile(mouseAnchor.position, 0);
                        elapseTime = baseElapse;
                    }
                    break;
                case 3:
                    {
                        LaunchProjectile(mouseAnchor.position, 0, 3);
                    }
                    break;
            }
            specialLaunch = 0;
        }
        UpdateSpecial();
        scoring.ShotTaken();
        Sounds.instance.PlayClip(1);
    }

    void LaunchProjectile(Vector3 pos, int index = 0, int special = 0)
    {
        Debug.LogError(special);
        if (launched.GetComponent<projectile>().getPhysics())
        {
            thePro = Instantiate(launched, transform.position, Quaternion.identity).GetComponent<projectile>();
            Rigidbody2D rb2d = thePro.GetComponent<Rigidbody2D>();
            if (Mathf.Abs(pos.y - transform.position.y) < 1)
            {
                rb2d.gravityScale = Mathf.Abs(pos.y - transform.position.y);
            }
            if (pos.y < transform.position.y)
            {
                rb2d.gravityScale *= -1;

            }
            Vector2 theImpulse = new Vector2((pos.x - transform.position.x) * 1 / elapseTime, (pos.y + 0.5f * -(Physics.gravity.y * rb2d.gravityScale) * (elapseTime * elapseTime) - transform.position.y) / elapseTime);
            thePro.GetComponent<Rigidbody2D>().AddForce(theImpulse, ForceMode2D.Impulse);
            thePro.special = special;
        }
        else
        {
            thePro = Instantiate(launched, transform.position, Quaternion.identity).GetComponent<projectile>();
            thePro.SetParabola(parabolaTerms, pos, 1 / elapseTime);
            thePro.gameObject.GetComponent<indexer>().INDEX = index;
            thePro.special = special;
        }
    }

    void Parabola(Vector2 otherPoint, Vector2 vertexPoint)
    {
        //y = (x-h)^2 + k
        equationVariables[0] = (otherPoint.x - vertexPoint.x) * (otherPoint.x - vertexPoint.x);
        equationVariables[1] = vertexPoint.y / equationVariables[0];
        equationVariables[2] = (otherPoint.y / equationVariables[0]) - equationVariables[1];

        parabolaTerms[0] = equationVariables[2];
        parabolaTerms[1] = ((vertexPoint.x * -1) * 2) * equationVariables[2];
        parabolaTerms[2] = ((vertexPoint.x * -1) * (vertexPoint.x * -1) * equationVariables[2]) + vertexPoint.y;
    }

    void UpdateSpecial()
    {
        if (specialLaunch == 0)
        {
            POWERUP_IMAGE.SetActive(false);
        }
        else
        {
            POWERUP_IMAGE.SetActive(true);
            power_up.sprite = powerupSprites[specialLaunch];
            POWERUP_IMAGE.transform.position = GameManager.instance.GetLauncherPosition() + new Vector3(0, 75); 
        }
    }

    public void LoadSpecial(int incoming)
    {
        specialLaunch = incoming;
        UpdateSpecial();
    }

    public void ResetSpecial()
    {
        specialLaunch = 0;
    }
}
