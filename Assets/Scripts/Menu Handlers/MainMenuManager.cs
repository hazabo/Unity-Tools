using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StateChange(int index)
    {
        switch (index)
        {
            case 0:
                GameStateManager.Instance.EnterState("ShapeRenderingState");
                break;

            case 1:
                GameStateManager.Instance.EnterState("TerrainGenerationState");
                break;

            case 2:
                GameStateManager.Instance.EnterState("DungeonGenerationState");
                break;

            case 3:
                GameStateManager.Instance.EnterState("MazePathFindingState");
                break;

            case 4:
                Application.Quit();
                break;
        }
    }
}
