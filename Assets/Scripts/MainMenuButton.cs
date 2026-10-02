using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton: MonoBehaviour
{
    public string SceneToLoad;
    public void OnButtonClick()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
