using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    [SerializeField] int value;
    bool hit = false;

    [SerializeField] int special;

    protected static Collider2D[] col = null;

    bool doPenCheck = true;
    int checkCount = 0;

    void Start()
    {
       if(col == null)
        {
            col = new Collider2D[10];
        }
    }

    void Update()
    {
        if(doPenCheck)
        {
            checkCount++;
            if(checkCount > 4)
            {
                FixOverlap();
                doPenCheck = false;
                if(!(GetComponent<SpriteRenderer>().isVisible))
                {
                    Destroy(gameObject);
                }
                else
                {
                    transform.Translate(-0.5f, 0, 0);
                }
            }
        }
    }

    public void Hit(Transform by)
    {
        if (!hit)
        {
            if (special != 0)
            {
                launcher.instance.LoadSpecial(special);
            }
            hit = true;
            Sounds.instance.PlayClip(0);
            scoring.Score(value);
            transform.parent = by;
            transform.Translate(0, 0, -0.1f);
            Destroy(GetComponent<Collider2D>());
            scoring.AddShotMultiplier();
            TargetsManager.instance.UpdateCombos();
        }
    }

    [ContextMenu("overlap")]
    void FixOverlap()
    {
        CircleCollider2D theCol = GetComponent<CircleCollider2D>();
        int j = Physics2D.OverlapAreaNonAlloc(transform.position - new Vector3(theCol.radius, theCol.radius), transform.position + new Vector3(theCol.radius, theCol.radius), col);
        for (int i = 0; i < j; i++)
        {
            if (col[i].gameObject.GetComponent<wall>())
            {
                Vector2 vect = (transform.position - col[i].transform.position);
                transform.Translate(vect.normalized);
            }
        }
    }
}
