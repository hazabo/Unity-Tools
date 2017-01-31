using UnityEngine;
using System.Collections;

public class SphereGenerationMenuHandler : MonoBehaviour
{
    GameObject sphereGenerationHandler;
    GameObject previousMenu;
    // Use this for initialization
    void Start ()
    {
        sphereGenerationHandler = GameObject.Find("SphereGenerationHandler");
        previousMenu = GameObject.Find("ShapeGenerationMenu");
        previousMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    public void StateChange(int index)
    {
        switch (index)
        {
            case 0:
                sphereGenerationHandler.GetComponent<SphereGeneration>().Render();
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
