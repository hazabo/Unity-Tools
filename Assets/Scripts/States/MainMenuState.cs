using UnityEngine;
using System.Collections;

public class MainMenuState : State
{
    private GameObject MainMenu;
    public string MainMenuPrefabFilePath = "Menus/MainMenu";

    public MainMenuState(string a_stringName) : base(a_stringName)
    {
        m_fDuration = -1;
        Process = Initialise;
    }

    protected override void Initialise(float a_fTimeStep)
    {
        MainMenu = Object.Instantiate(Resources.Load(MainMenuPrefabFilePath, typeof(GameObject)) as GameObject);
        MainMenu.name = "MainMenu";
        Process = Update;
    }

    protected override void Update(float a_fTimeStep)
    {
        
    }

    protected override void Leave(float a_fTimeStep)
    {

        Process = Initialise;
    }

    
}
