using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    Vector2 startPos;
    [SerializeField] bool moving = false;
    [SerializeField] Vector2 endPos, size;

    [SerializeField] float speed, elasticity;

    bool movingAway = true;

    public struct RectWithReference
    {
        public Rect theRect;
        public wall theReference;
    }

    public RectWithReference MovementBox()
    {
        Vector2 min = new Vector2(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
        Vector2 max = new Vector2(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));

        Rect r = new Rect { min = min, max = max };

        r.x -= size.x * 0.5f;
        r.width += size.x;
        r.y -= size.y * 0.5f;
        r.height += size.y;

        return new RectWithReference {theRect = r, theReference = this };
    }

    public Vector2 GetSize()
    {
        return size;
    }
    public bool IsMobile()
    {
        return moving;
    }
    public float GetElastic()
    {
        return elasticity;
    }

    void Start()
    {
        startPos = transform.position;
        SetRandomPosition();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            if(Mathf.Abs(endPos.y) > 8)
            {
                SetRandomPosition();
            }
            if(Mathf.Abs(endPos.x) > 11)
            {
                SetRandomPosition();
            }

            if (movingAway)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, speed * Time.fixedDeltaTime);

                if (Vector3.Distance(transform.position, endPos) < 0.1f)
                {
                    movingAway = false;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.fixedDeltaTime);

                if (Vector3.Distance(transform.position, startPos) < 0.1f)
                {
                    movingAway = true;
                }
            }
        }
    }

    public Rect GetCollisionRect()
    {
        return new Rect((Vector2)transform.position - (size * 0.45f), size * 0.9f);
    }

    //TODO: call this a different way to support fixed directions
    void SetRandomPosition(float dist = 5f)
    {
        Vector2 vect = Random.insideUnitCircle.normalized;

        vect *= dist;

        vect = startPos + vect;

        endPos = vect;
    }

    public Vector2 ProjectPosition(float atWhen)
    {
        Vector2 v = transform.position;
        bool B_MOVE = movingAway;

        for (int i = 0; i < atWhen; i++)
        {
            if (B_MOVE)
            {
                v = Vector2.MoveTowards(v, endPos, speed * Time.fixedDeltaTime);

                if (Vector2.Distance(v, endPos) < 0.1f)
                {
                    B_MOVE = false;
                }
            }
            else
            {
                v = Vector2.MoveTowards(v, startPos, speed * Time.fixedDeltaTime);

                if (Vector2.Distance(v, startPos) < 0.1f)
                {
                    B_MOVE = true;
                }
            }
        }

        return v;
    }

    public Rect ProjectRect(float atWhen)
    {
        return new Rect(ProjectPosition(atWhen) - (size * 0.45f), size * 0.9f);
    }

    public GameObject Print(Vector2 pos)
    {
        GameObject newObject = Instantiate(gameObject, pos, transform.rotation);
        newObject.tag = "printing";
        newObject.name = "PRINTED";
        newObject.GetComponent<wall>().enabled = false;
        //Destroy(newObject.GetComponent<SpriteRenderer>());
        newObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        return newObject;
    }
}
