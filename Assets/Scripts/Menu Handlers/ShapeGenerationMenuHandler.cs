using UnityEngine;
using System.Collections;

public class ShapeGenerationMenuHandler : MonoBehaviour
{
    GameObject previousMenu;
	// Use this for initialization
	void Start ()
    {
        previousMenu = GameObject.Find("MainMenu");
        previousMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StateChange(int index)
    {
        switch (index)
        {
            case 0:
                GameStateManager.Instance.EnterState("CubeGenerationState");
                break;

            case 1:
                GameStateManager.Instance.EnterState("ConeGenerationState");
                break;

            case 2:
                GameStateManager.Instance.EnterState("SphereGenerationState");
                break;

            case 3:
                GameStateManager.Instance.EnterState("MainMenuState");
                break;


        }
    }
    
    void OnDestroy()
    {
        previousMenu.SetActive(true);
    }
}
