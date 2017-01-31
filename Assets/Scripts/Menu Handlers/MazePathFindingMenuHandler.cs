using UnityEngine;
using System.Collections;

public class MazePathFindingMenuHandler : MonoBehaviour
{
    GameObject mazePathFindingHandler;
    GameObject previousMenu;
    // Use this for initialization
    void Start ()
    {
        previousMenu = GameObject.Find("MainMenu");
        previousMenu.SetActive(false);
        mazePathFindingHandler = GameObject.Find("MazePathFindingHandler");
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
                mazePathFindingHandler.GetComponent<MazeAndPathFinding>().Generate();
                break;

            case 1:
                mazePathFindingHandler.GetComponent<MazeAndPathFinding>().PathFind();
                break;

            case 2:
                GameStateManager.Instance.EnterState("MainMenuState");
                break;
        }
    }

    void OnDestroy()
    {
        previousMenu.SetActive(true);
    }
}
