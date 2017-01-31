using UnityEngine;
using System.Collections;

public class DungeonGenerationMenuHandler : MonoBehaviour
{
    GameObject dungeonGenerationHandler;
    GameObject previousMenu;
    // Use this for initialization
    void Start ()
    {
        previousMenu = GameObject.Find("MainMenu");
        previousMenu.SetActive(false);
        dungeonGenerationHandler = GameObject.Find("DungeonGenerationHandler");
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
                dungeonGenerationHandler.GetComponent<DungeonGeneration>().Generate();
                break;

            case 1:
                GameStateManager.Instance.EnterState("MainMenuState");
                break;

        }
    }

    void OnDestroy()
    {
        previousMenu.SetActive(true);
    }
}
