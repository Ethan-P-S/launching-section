using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] bool UsePhysics;
    public float speed;
    float[] parabolaTerms;
    float xPos, angle;
    Vector2 dist, vertex, startPos;
    Collider2D[] collide;
    Collider2D prevHit;

    Transform moveTowards;
    motionPoint POINT;
    List<Transform> movePoints;
    int moveCount = 0;
    int intersected = 0;

    [SerializeField] float elapsed_delta = 0;
    float elasped = 0;
    bool fading = false, cleaning = false, specialThing = true;
    Rigidbody2D rb2d;

    [HideInInspector] public int special = 0;

    SpriteRenderer[] allTheSprs;

    public bool getPhysics()
    {
        return UsePhysics;
    }

    void Start()
    {
        elasped = 0;
        movePoints = new List<Transform>();
        collide = new Collider2D[4];
        if (!UsePhysics)
        {
            xPos = -8.25f;
            startPos.x = xPos;
            startPos.y = (xPos * xPos * parabolaTerms[0]) + (xPos * parabolaTerms[1]) + parabolaTerms[2];
            Predict();
            GetMoveTarget();
        }
        else
        {
            startPos = transform.position;
            rb2d = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        elasped += Time.deltaTime;
        if (fading)
        {
            for (int i = 0; i < allTheSprs.Length; i++)
            {
                allTheSprs[i].color = new Color(allTheSprs[i].color.r, allTheSprs[i].color.g, allTheSprs[i].color.b, allTheSprs[i].color.a - (Time.deltaTime * 0.6f));
            }
            if (allTheSprs[0].color.a < 0.01f)
            {
                CleanThisUp();
            }
        }
        else if (elasped > 8)
        {
            allTheSprs = GetComponentsInChildren<SpriteRenderer>();
            fading = true;
        }

        if (specialThing)
        {
            if (special == 3)
            {
                if (((startPos.y - transform.position.y) * rb2d.gravityScale) > 0.1f)
                {
                    specialThing = false;
                    rb2d.velocity = new Vector2(-rb2d.velocity.x, 0);
                    rb2d.gravityScale = 0;
                }
            }
            if (special == 7)
            {
                if (Mathf.Abs(transform.position.y) > 5.4f)
                {
                    specialThing = false;
                    rb2d.velocity = new Vector2(rb2d.velocity.x, -rb2d.velocity.y);
                }
                else if (!GetComponent<SpriteRenderer>().isVisible)
                {
                    specialThing = false;
                    rb2d.velocity = new Vector2(-rb2d.velocity.x, rb2d.velocity.y);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!cleaning)
        {
            if (!UsePhysics)
            {
                elapsed_delta += Time.fixedDeltaTime * speed;

                transform.rotation = Quaternion.identity;
                if (!moveTowards)
                {
                    GetMoveTarget();
                }

                if (moveCount > 0 && moveCount < movePoints.Count)
                {
                    if (elapsed_delta > GetMovePoint(movePoints[moveCount]).progressValue)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            if (elapsed_delta > GetMovePoint(movePoints[moveCount]).progressValue)
                            {
                                NextPoint();
                                if (moveCount >= movePoints.Count)
                                {
                                    CleanThisUp();
                                    return;
                                }
                            }
                        }
                    }
                    float timeLerp = Mathf.InverseLerp(GetMovePoint(movePoints[moveCount - 1]).progressValue, GetMovePoint(movePoints[moveCount]).progressValue, elapsed_delta);

                    transform.position = Vector3.Lerp(movePoints[moveCount - 1].position, movePoints[moveCount].position, timeLerp);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, moveTowards.position, Time.fixedDeltaTime * speed);
                }

                if (Vector3.Distance(transform.position, moveTowards.position) < 0.001f)
                {
                    NextPoint();
                }
                else
                {
                    GetMoveTarget();
                }

                dist = moveTowards.position - transform.position;
                if (dist != Vector2.zero)
                {
                    angle = Mathf.Rad2Deg * Mathf.Asin(dist.y / Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y)));
                    transform.Rotate(0, 0, angle);
                }
            }
            else
            {
                if (special != -555)
                {
                    transform.rotation = Quaternion.identity;

                    float XVAL = rb2d.velocity.x * rb2d.velocity.x;
                    float YVAL = rb2d.velocity.y * rb2d.velocity.y;
                    angle = Mathf.Rad2Deg * Mathf.Asin(rb2d.velocity.y / (Mathf.Sqrt(XVAL + YVAL)));
                    if (rb2d.velocity.x < 0)
                    {
                        transform.Rotate(0, 0, -angle);
                        transform.Rotate(0, 0, 180);
                    }
                    else
                    {
                        transform.Rotate(0, 0, angle);
                    }
                }
            }
            if ((transform.position.y > 12 && rb2d.gravityScale < 0) || (transform.position.y < -12 && rb2d.gravityScale > 0) || transform.position.x > 11 || transform.position.x < -10)
            {
                cleaning = true;
                CleanThisUp();
            }
        }
    }

    void CleanThisUp()
    {
        if (GameObject.FindGameObjectsWithTag("arrow").Length == 1)
        {
            if (!scoring.somethingWasHit)
            {
                scoring.ResetConMultiplier();
            }
            else
            {
                scoring.AddConMultiplier();
            }
        }

        if (!UsePhysics)
        {
            Collider2D FOO = null;
            if (GetMovePoint(movePoints[movePoints.Count - 1]).col)
            {
                FOO = GetMovePoint(movePoints[movePoints.Count - 1]).col;
            }
            for (int i = 0; i < movePoints.Count; i++)
            {
                if (movePoints[i])
                {
                    Destroy(movePoints[i].gameObject);
                }
            }
            launcher.instance.SetCanLaunch(true);
            if (FOO)
            {
                transform.parent = FOO.transform;
                gameObject.tag = "Untagged";
                Destroy(GetComponent<Collider2D>());
                Destroy(this);
            }
            else
            {
                Destroy(gameObject);
            }

        }
        else
        {
            launcher.instance.SetCanLaunch(true);
            Destroy(gameObject);
        }
        scoring.ResetShotMultiplier();
        TargetsManager.instance.UpdateCombos();
    }

    motionPoint GetMovePoint(Transform t)
    {
        return t.GetComponent<motionPoint>();
    }
    void NextPoint()
    {
        POINT = moveTowards.gameObject.GetComponent<motionPoint>();
        if (POINT.col)
        {
            switch (POINT.INDEX)
            {
                case 1:
                    Sounds.instance.PlayClip(2);
                    if (POINT.col.gameObject.GetComponent<multi_hit>())
                    {
                        POINT.col.gameObject.GetComponent<multi_hit>().Hit();
                    }
                    break;
                case 2:
                    if (POINT.col.gameObject.GetComponent<target>())
                    {
                        POINT.col.gameObject.GetComponent<target>().Hit(transform);
                        Sounds.instance.PlayClip(0);
                    }
                    break;
            }
        }
        GetMoveTarget();
    }
    void GetMoveTarget()
    {
        moveCount++;
        if (moveCount < movePoints.Count)
        {
            moveTowards = movePoints[moveCount];
            POINT = moveTowards.gameObject.GetComponent<motionPoint>();
            if (POINT.col)
            {
                switch (POINT.INDEX)
                {
                    case 1:
                        Sounds.instance.PlayClip(2);
                        if (POINT.col.gameObject.GetComponent<multi_hit>())
                        {
                            POINT.col.gameObject.GetComponent<multi_hit>().Hit();
                        }
                        break;
                    case 2:
                        if (POINT.col.gameObject.GetComponent<target>())
                        {
                            POINT.col.gameObject.GetComponent<target>().Hit(transform);
                            Sounds.instance.PlayClip(0);
                        }
                        break;
                }
            }
        }
        else
        {
            moveTowards = movePoints[movePoints.Count - 1];
            cleaning = true;
            CleanThisUp();
        }
    }

    public void SetParabola(float[] newTerms, Vector2 vertex, float speed = 10f)
    {
        this.speed = speed;
        this.vertex = new Vector2(vertex.x, vertex.y);
        parabolaTerms = new float[newTerms.Length];

        for (int i = 0; i < newTerms.Length; i++)
        {
            parabolaTerms[i] = newTerms[i];
        }
    }

    Vector2 GetExitPoint()
    {
        float predictX = transform.position.x;
        float predictY = transform.position.y;

        for (int i = 0; i < 2; i++)
        {
            predictX += Time.fixedDeltaTime * speed;
            predictY = (predictX * predictX * parabolaTerms[0]) + (predictX * parabolaTerms[1]) + parabolaTerms[2];

            if (predictY > 5f || predictY < -5f || predictX > 8)
            {
                i = 2;
            }
            else
            {
                i--;
            }
        }
        return new Vector2(predictX, predictY);
    }

    void Predict()
    {
        Transform T;
        Vector2 V = Vector2.zero, preV;
        float movement = 0.2f; int counter = 0;
        float accumulatedDist = -8.25f;

        for (float i = transform.position.x; (i < 10 && i > -10) && counter < 600; i += movement)
        {
            intersected = Physics2D.OverlapBoxNonAlloc(V, new Vector2(0.75f, 0.4f), angle, collide);

            counter++;
            T = Instantiate(SpawnableObjects.instance.GetObject(2, 0), transform.position, Quaternion.identity).transform;
            T.GetComponent<indexer>().INDEX = GetComponent<indexer>().INDEX;
            preV = V;
            V = new Vector2(i, (parabolaTerms[0] * i * i) + (parabolaTerms[1] * i) + parabolaTerms[2]);
            dist = V - preV;

            accumulatedDist += dist.magnitude;

            T.gameObject.GetComponent<motionPoint>().SetFloats(accumulatedDist);

            angle = Mathf.Rad2Deg * Mathf.Asin(dist.y / Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y)));
            T.position = V;


            bool interaction = false;
            if (TargetsManager.instance.OverlappingTargetZones(V).Length > 0)
            {
                wall.RectWithReference[] rects = TargetsManager.instance.OverlappingTargetZones(V);
                for (int j = 0; j < rects.Length; j++)
                {
                    Vector2 projected_pos = rects[j].theReference.ProjectPosition(GetMovePoint(movePoints[movePoints.Count - 1]).ExpectedTicks(15));

                    float projectedDist = Vector2.Distance(V, projected_pos);

                    if (projectedDist < rects[j].theReference.GetSize().magnitude * 2)
                    {
                        Rect collisionRect = rects[j].theReference.ProjectRect(GetMovePoint(movePoints[movePoints.Count - 1]).ExpectedTicks(15));

                        //launcher.instance.DebugRect(collisionRect);

                        if (collisionRect.Contains(V))
                        {
                            interaction = true;
                            Vector2 distDiff = collisionRect.center - V;
                            motionPoint thePoint = T.gameObject.GetComponent<motionPoint>();
                            thePoint.INDEX = 1;
                            Vector2 diff = new Vector2(V.x - vertex.x, V.y - vertex.y);
                            Vector2 vect = GetExitPoint() + diff;

                            //y = (x-h)^2 + k
                            float A = (vect.x - V.x) * (vect.x - V.x);
                            float B = V.y / A;
                            float C = (vect.y / A) - B;

                            parabolaTerms[0] = C;
                            parabolaTerms[1] = ((V.x * -1) * 2) * C;
                            parabolaTerms[2] = ((V.x * -1) * (V.x * -1) * C) + V.y;

                            Vector2 VV = rects[j].theReference.GetComponent<wall>().GetSize();
                            distDiff = new Vector2(distDiff.x / VV.x, distDiff.y / VV.y);

                            if (Mathf.Abs(distDiff.x) > Mathf.Abs(distDiff.y))
                            {
                                movement *= -1;
                            }
                            else
                            {
                                //vertical bounce
                                float yVal = vertex.y - collisionRect.center.y;
                                vertex.y -= yVal;
                                vertex.y += yVal * rects[j].theReference.GetComponent<wall>().GetElastic();
                                vertex.x += (Mathf.Abs(vertex.x - collisionRect.center.x)) * (1f + rects[j].theReference.GetComponent<wall>().GetElastic());

                                A = (V.x - vertex.x) * (V.x - vertex.x);
                                B = vertex.y / A;
                                C = (V.y / A) - B;

                                parabolaTerms[0] = C;
                                parabolaTerms[1] = ((vertex.x * -1) * 2) * C;
                                parabolaTerms[2] = ((vertex.x * -1) * (vertex.x * -1) * C) + vertex.y;
                            }
                            prevHit = rects[j].theReference.GetComponent<Collider2D>();
                            thePoint.col = rects[j].theReference.gameObject.GetComponent<Collider2D>();

                        }
                    }
                }
            }
            if (!interaction)
            {
                if (intersected > 0)
                {
                    motionPoint thePoint = T.gameObject.GetComponent<motionPoint>();
                    if (collide[0].CompareTag("wall"))
                    {
                        bool collision = true;

                        if (collide[0].GetComponent<wall>().IsMobile())
                        {
                            collision = false;
                        }

                        if (collision)
                        {
                            Vector2 distDiff = (Vector2)collide[0].transform.position - V;

                            if (prevHit != collide[0])
                            {
                                thePoint.INDEX = 1;
                            }

                            Vector2 diff = new Vector2(V.x - vertex.x, V.y - vertex.y);
                            Vector2 vect = GetExitPoint() + diff;

                            //y = (x-h)^2 + k
                            float A = (vect.x - V.x) * (vect.x - V.x);
                            float B = V.y / A;
                            float C = (vect.y / A) - B;

                            parabolaTerms[0] = C;
                            parabolaTerms[1] = ((V.x * -1) * 2) * C;
                            parabolaTerms[2] = ((V.x * -1) * (V.x * -1) * C) + V.y;

                            if (Mathf.Abs(distDiff.x) > Mathf.Abs(distDiff.y))
                            {
                                if (prevHit != collide[0])
                                {
                                    movement *= -1;
                                }
                            }
                            else
                            {
                                //vertical bounce
                                if (prevHit != collide[0])
                                {
                                    float yVal = vertex.y - collide[0].transform.position.y;
                                    vertex.y -= yVal;
                                    vertex.y += yVal * collide[0].GetComponent<wall>().GetElastic();
                                    vertex.x += (Mathf.Abs(vertex.x - collide[0].transform.position.x)) * (1f + collide[0].GetComponent<wall>().GetElastic());

                                    A = (V.x - vertex.x) * (V.x - vertex.x);
                                    B = vertex.y / A;
                                    C = (V.y / A) - B;

                                    parabolaTerms[0] = C;
                                    parabolaTerms[1] = ((vertex.x * -1) * 2) * C;
                                    parabolaTerms[2] = ((vertex.x * -1) * (vertex.x * -1) * C) + vertex.y;
                                }
                            }
                            prevHit = collide[0];
                            thePoint.col = collide[0];
                        }
                    }
                    else if (collide[0].CompareTag("target"))
                    {
                        if (collide[0].GetComponent<multi_hit>())
                        {
                            thePoint.INDEX = 1;
                            counter = 601;
                        }
                        else
                        {
                            if (prevHit != collide[0])
                            {
                                thePoint.INDEX = 2;
                            }
                        }
                        thePoint.col = collide[0];
                    }
                }
            }

            movePoints.Add(T);
        }
        prevHit = null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (UsePhysics)
        {
            if (collision.gameObject.GetComponent<multi_hit>())
            {
                HitSomething();
                scoring.ResetShotMultiplier();
                TargetsManager.instance.UpdateCombos();
                transform.parent = collision.transform;
                gameObject.tag = "Untagged";
                collision.gameObject.GetComponent<multi_hit>().Hit();
                launcher.instance.SetCanLaunch(true);
                Destroy(GetComponent<Collider2D>());
                Destroy(rb2d);
                Destroy(this);
            }
            else if (collision.gameObject.CompareTag("target"))
            {
                collision.gameObject.GetComponent<target>().Hit(transform);
                HitSomething();
                if(special == 4)
                {
                    TARGET_ACQUIRED();
                }
            }
        }
    }

    void TARGET_ACQUIRED()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("target");

        if (targetObjects.Length > 0)
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;

            float DIST = 10000;
            Transform target_acquired = targetObjects[0].transform;

            foreach (GameObject obj in targetObjects)
            {
                if (Vector2.Distance(transform.position, obj.transform.position) < DIST)
                {
                    DIST = Vector2.Distance(transform.position, obj.transform.position);
                    target_acquired = obj.transform;
                }
            }
            rb2d.velocity = (transform.position - target_acquired.position) * -2;

            transform.rotation = Quaternion.identity;
            angle = Mathf.Rad2Deg * Mathf.Asin(rb2d.velocity.y / Mathf.Sqrt((rb2d.velocity.x * rb2d.velocity.x) + (rb2d.velocity.y * rb2d.velocity.y)));
            transform.Rotate(0, 0, angle);
            special = -555;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            Sounds.instance.PlayClip(2);
            if(special == 3)
            {
                specialThing = false;
            }
        }
    }

    void HitSomething()
    {
        scoring.somethingWasHit = true;
    }
}
