using UnityEngine;
using System.Collections;

public class DungeonGenerationState : State
{
    private GameObject dungeonGenerationMenu;
    private string dungeonGenerationMenuPrefabFilePath = "Menus/DungeonGenerationMenu";

    private GameObject dungeonGenerationHandler;
    public string dungeonGenerationHandlerPrefabFilePath = "Handlers/DungeonGenerationHandler";

    public DungeonGenerationState(string a_stringName) : base(a_stringName)
    {
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        dungeonGenerationHandler = Object.Instantiate(Resources.Load(dungeonGenerationHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        dungeonGenerationHandler.name = "DungeonGenerationHandler";
        dungeonGenerationMenu = Object.Instantiate(Resources.Load(dungeonGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        dungeonGenerationMenu.name = "DungeonGenerationMenu";

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
        Object.Destroy(dungeonGenerationMenu);
        Object.Destroy(dungeonGenerationHandler);
        Process = Initialise;
    }
}