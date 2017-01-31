using UnityEngine;
using System.Collections;

public class SphereGenerationState : State
{
    private GameObject sphereGenerationMenu;
    private string sphereGenerationMenuPrefabFilePath = "Menus/SphereGenerationMenu";

    private GameObject sphereGenerationHandler;
    private string sphereGenerationHandlerPrefabFilePath = "Handlers/SphereGenerationHandler";

    public SphereGenerationState(string a_stringName) : base(a_stringName)
    {
        IsBlocking = true;
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        sphereGenerationHandler = Object.Instantiate(Resources.Load(sphereGenerationHandlerPrefabFilePath, typeof(GameObject)) as GameObject);
        sphereGenerationHandler.name = "SphereGenerationHandler";
        sphereGenerationMenu = Object.Instantiate(Resources.Load(sphereGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        sphereGenerationMenu.name = "SphereGenerationMenu";

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
        Object.Destroy(sphereGenerationMenu);
        Object.Destroy(sphereGenerationHandler);
        Process = Initialise;
    }
}