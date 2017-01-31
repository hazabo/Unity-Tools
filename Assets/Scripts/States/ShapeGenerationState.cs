using UnityEngine;
using System.Collections;

public class ShapeRenderingState : State
{
    private GameObject shapeGenerationMenu;
    public string shapeGenerationMenuPrefabFilePath = "Menus/ShapeGenerationMenu";

    public ShapeRenderingState(string a_stringName) : base(a_stringName)
    {
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        shapeGenerationMenu = Object.Instantiate(Resources.Load(shapeGenerationMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        shapeGenerationMenu.name = "ShapeGenerationMenu";
        Process = Update;
    }

    protected override void Update(float a_fTimeStep)
    {
        if(GameStateManager.Instance.CurrentState!=this)
        {
            Process = Leave;
        }
    }

    protected override void Leave(float a_fTimeStep)
    {
        //pop state
        Object.Destroy(shapeGenerationMenu);
        Process = Initialise;
    }
}