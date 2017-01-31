using UnityEngine;
using System.Collections;


public class Updater : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        //This will get called each frame from the scene that our
        //loader is created in
        GameStateManager.Instance.Update();

    }

}