using UnityEngine;
using System.Collections;

public class TerrainGenerationState : State
{
    private GameObject terrainGenerationMenu;
    private string terrainGenerationMenuPrefabFilePath = "Menus/TerrainGenerationMenu";

    private GameObject terrainGenerationHandler;
    public string terrainGenerationHandlerPrefabFilePath = "Handlers/TerrainGenerationHandler";

    public TerrainGenerationState(string a_stringName) : base(a_stringName)
    {
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        terrainGenerationHandler = Object.Instantiate(Resources.Load(terrainGenerationHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        terrainGenerationHandler.name = "TerrainGenerationHandler";
        terrainGenerationMenu = Object.Instantiate(Resources.Load(terrainGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        terrainGenerationMenu.name = "TerrainGenerationMenu";

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
        Object.Destroy(terrainGenerationMenu);
        Object.Destroy(terrainGenerationHandler);
        Process = Initialise;
    }
}
