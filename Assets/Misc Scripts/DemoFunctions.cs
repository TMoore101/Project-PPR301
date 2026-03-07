using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoFunctions : MonoBehaviour
{
    //== Restart Demo
    public void RestartDemo()
    {
        // Load demo scene
        SceneManager.LoadScene(SceneManager.GetSceneByName("Lvl_01_Factory").buildIndex);
    }
}
