using UnityEngine;
using System.Collections;

public class CubeGenerationMenuHandler : MonoBehaviour {

    GameObject previousMenu;
    // Use this for initialization
    void Start ()
    {
        previousMenu = GameObject.Find("ShapeGenerationMenu");
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
                GameStateManager.Instance.EnterState("ShapeRenderingState");
                break;



        }
    }

    void OnDestroy()
    {
        previousMenu.SetActive(true);
    }
}
