using UnityEngine;
using System.Collections;

public class CubeGenerationState : State
{
    private GameObject cubeGenerationMenu;
    private string cubeGenerationMenuPrefabFilePath = "Menus/CubeGenerationMenu";

    private GameObject cubeGenerationHandler;
    private string cubeGenerationHandlerPrefabFilePath = "Handlers/CubeGenerationHandler";

    public CubeGenerationState(string a_stringName) : base(a_stringName)
    {
        IsBlocking = true;
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        cubeGenerationHandler = Object.Instantiate(Resources.Load(cubeGenerationHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        cubeGenerationHandler.name = "CubeGenerationHandler";
        cubeGenerationMenu = Object.Instantiate(Resources.Load(cubeGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        cubeGenerationHandler.name = "CubeGenerationMenu";
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
        Object.Destroy(cubeGenerationMenu);
        Object.Destroy(cubeGenerationHandler);
        Process = Initialise;
    }
}