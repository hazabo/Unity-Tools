using UnityEngine;
using System.Collections;

public class MazePathFindingState : State
{
    private GameObject mazePathFindingMenu;
    private string mazePathFindingMenuPrefabFilePath = "Menus/MazeAndPathFindingMenu";

    private GameObject mazePathFindingHandler;
    public string mazePathFindingHandlerPrefabFilePath = "Handlers/MazePathFindingHandler";

    public MazePathFindingState(string a_stringName) : base(a_stringName)
    {
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        mazePathFindingHandler = Object.Instantiate(Resources.Load(mazePathFindingHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        mazePathFindingHandler.name = "MazePathFindingHandler";
        mazePathFindingMenu = Object.Instantiate(Resources.Load(mazePathFindingMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        mazePathFindingMenu.name = "MazeAndPathFindingMenu";

        Process = Update;
    }

    protected override void Update(float a_fTimeStep)
    {
        if (GameStateManager.Instance.CurrentState != this)
        {
            Process = Leave;
        }
    }

    protected override void Leave(float a_fTimeStep)
    {
        Object.Destroy(mazePathFindingMenu);
        Object.Destroy(mazePathFindingHandler);
        Process = Initialise;
    }
}