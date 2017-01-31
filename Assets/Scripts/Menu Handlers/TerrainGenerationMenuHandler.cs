using UnityEngine;
using System.Collections;

public class TerrainGenerationMenuHandler : MonoBehaviour
{
    GameObject terrainGenerationHandler;
    GameObject previousMenu;
    // Use this for initialization
    void Start ()
    {
        previousMenu = GameObject.Find("MainMenu");
        previousMenu.SetActive(false);
        terrainGenerationHandler = GameObject.Find("TerrainGenerationHandler");
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
                terrainGenerationHandler.GetComponent<TerrainGeneration>().Render();
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
