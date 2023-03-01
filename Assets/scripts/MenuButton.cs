using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] SpriteRenderer textRender;

    void OnMouseUpAsButton() //refactor input later
    {
        //Debug.Log("button with index of " + index + " was pressed");
        switch (index)
        {
            case 0:
                {
                    GameManager.instance.PlayPressed();
                }
                break;
            case 1:
                {
                    GameManager.instance.GoToMenu();
                }
                break;
            case 2:
                {
                    GameManager.instance.PlayInfinite();
                }
                break;
            case 3:
                {
                    GameManager.instance.PlayLevel();
                }
                break;
            case 4:
                {
                    GameManager.instance.LevelSelect(GetComponent<indexer>().INDEX -1);
                }
                break;
            case 5:
                {
                    GameManager.instance.GoToMenu();
                    GameManager.instance.PlayPressed();
                    GameManager.instance.PlayLevel();
                }
                break;
            case 6:
                {
                    GameManager.instance.NextLevel();
                }
                break;
            case 7:
                {
                    TargetsManager.instance.ResetLevel();
                }
                break;
        }
    }

    public void Deactivate()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        if(textRender)
        {
            textRender.enabled = false;
        }
    }
    public void Activate()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
        if (textRender)
        {
            textRender.enabled = true;
        }
    }
}
