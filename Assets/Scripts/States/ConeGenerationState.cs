using UnityEngine;
using System.Collections;

public class ConeGenerationState : State
{
    private GameObject coneGenerationMenu;
    private string coneGenerationMenuPrefabFilePath = "Menus/ConeGenerationMenu";

    private GameObject coneGenerationHandler;
    private string coneGenerationHandlerPrefabFilePath = "Handlers/ConeGenerationHandler";

    public ConeGenerationState(string a_stringName) : base(a_stringName)
    {
        IsBlocking = true;
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        coneGenerationHandler = Object.Instantiate(Resources.Load(coneGenerationHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        coneGenerationHandler.name = "ConeGenerationHandler";
        coneGenerationMenu = Object.Instantiate(Resources.Load(coneGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        coneGenerationMenu.name = "ConeGenerationMenu";
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
        Object.Destroy(coneGenerationMenu);
        Object.Destroy(coneGenerationHandler);
        Process = Initialise;
    }
}