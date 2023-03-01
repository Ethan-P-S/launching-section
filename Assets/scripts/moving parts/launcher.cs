using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class launcher : MonoBehaviour
{
    public GameObject DebugObject;
    public static launcher instance;
    Camera c;
    Vector3 mousePos;
    projectile thePro;
    [SerializeField] GameObject LAUNCH_IMAGE; Image theImage;
    float[] equationVariables;
    float[] parabolaTerms;

    [SerializeField] Transform mouseAnchor;
    [SerializeField] GameObject launched;
    [SerializeField] float elapseTime;
    float baseElapse;
    [SerializeField] Sprite readySprite, emptySprite;

    Queue<int> specialLaunches;
    [SerializeField] SpriteRenderer[] extraReticles;

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
        specialLaunches = new Queue<int>();
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

            if (extraReticles[0].enabled)
            {
                extraReticles[0].transform.position = mouseAnchor.position + new Vector3(0, 1);
                extraReticles[1].transform.position = mouseAnchor.position + new Vector3(0, -1);
            }

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

            if (set_count > 0)
            {
                set_count -= Time.deltaTime;
                if (set_count < 0 && GameObject.FindGameObjectWithTag("arrow") == null)
                {
                    set_count = -1;
                    canlaunch = set_set;
                    if (specialLaunches.Count > 0)
                    {
                        if (specialLaunches.Peek() == 1)
                        {
                            extraReticles[0].enabled = true;
                            extraReticles[1].enabled = true;
                        }
                    }
                }
            }

        }
    }

    void DoTheLaunch()
    {
         GameManager.instance.CleanThisUP("printing");
                        scoring.somethingWasHit = false;

                        canlaunch = false;

                        if (specialLaunches.Count == 0)
                        {
                            LaunchProjectile(mouseAnchor.position, 0);
                        }
                        else
                        {
                            int theIndex = specialLaunches.Dequeue();

                            switch (theIndex)
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

                                        extraReticles[0].enabled = false;
                                        extraReticles[1].enabled = false;
                                    }
                                    break;
                                case 2:
                                    {
                                        elapseTime = 3.25f;
                                        LaunchProjectile(mouseAnchor.position, 0);
                                        elapseTime = baseElapse;
                                    }
                                    break;
                            }
                        }
                        scoring.ShotTaken();
                        Sounds.instance.PlayClip(1);
    }

    void LaunchProjectile(Vector3 pos, int index = 0)
    {
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
            Vector2 theImpulse = new Vector2((pos.x - transform.position.x) * 1/elapseTime, (pos.y + 0.5f * -(Physics.gravity.y * rb2d.gravityScale) * (elapseTime * elapseTime) - transform.position.y) / elapseTime);
            thePro.GetComponent<Rigidbody2D>().AddForce(theImpulse, ForceMode2D.Impulse);
        }
        else
        {
            thePro = Instantiate(launched, transform.position, Quaternion.identity).GetComponent<projectile>();
            thePro.SetParabola(parabolaTerms, pos, 1/elapseTime);
            thePro.gameObject.GetComponent<indexer>().INDEX = index;
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

    public void QueueSpecial(int index)
    {
        specialLaunches.Enqueue(index);
    }

    public void ResetSpecial()
    {
        for (int i = 0; i < 100; i++)
        {
            if (specialLaunches.Count > 0)
            {
                 _ = specialLaunches.Dequeue();
            }
        }
    }

    public void DebugRect(Rect r)
    {
        Instantiate(instance.DebugObject, new Vector3(r.xMin, r.yMin), Quaternion.identity);
        Instantiate(instance.DebugObject, new Vector3(r.xMin, r.yMax), Quaternion.identity);
        Instantiate(instance.DebugObject, new Vector3(r.xMax, r.yMin), Quaternion.identity);
        Instantiate(instance.DebugObject, new Vector3(r.xMax, r.yMax), Quaternion.identity);
    }
    public void DebugPoint(Vector2 v)
    {
        Instantiate(instance.DebugObject, v, Quaternion.identity);
    }

}
