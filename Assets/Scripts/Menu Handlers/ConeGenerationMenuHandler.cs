using UnityEngine;
using System.Collections;

public class ConeGenerationMenuHandler : MonoBehaviour
{
    GameObject coneGenerationHandler;
    GameObject previousMenu;
    // Use this for initialization
    void Start()
    {
        coneGenerationHandler = GameObject.Find("ConeGenerationHandler");
        previousMenu = GameObject.Find("ShapeGenerationMenu");
        previousMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StateChange(int index)
    {
        switch (index)
        {
            case 0:
                coneGenerationHandler.GetComponent<ConeGeneration>().Render();
                break;

            case 1:
                GameStateManager.Instance.EnterState("ShapeRenderingState");
                break;

        }
    }

    void OnDestroy()
    {
        previousMenu.SetActive(true);
    }
}