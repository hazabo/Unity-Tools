using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameStateManager stateManager;
    public Updater UpdateController;
    // Use this for initialization
    void Awake()
    {
        stateManager = GameStateManager.Create();
        if (stateManager != null)
        {
            //Register game states here.
            stateManager.RegisterState<DungeonGenerationState>("DungeonGenerationState");
            stateManager.RegisterState<MazePathFindingState>("MazePathFindingState");
            stateManager.RegisterState<TerrainGenerationState>("TerrainGenerationState");
            stateManager.RegisterState<CubeGenerationState>("CubeGenerationState");
            stateManager.RegisterState<ConeGenerationState>("ConeGenerationState");
            stateManager.RegisterState<SphereGenerationState>("SphereGenerationState");
            stateManager.RegisterState<ShapeRenderingState>("ShapeRenderingState");
            stateManager.RegisterState<MainMenuState>("MainMenuState");

            stateManager.EnterState("MainMenuState");
            
        }

        Instantiate(UpdateController);
    }

    public void EnterState(string stateName)
    {
        stateManager.EnterState(stateName);
    }
}