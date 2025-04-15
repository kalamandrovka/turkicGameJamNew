using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonLoader : MonoBehaviour
{
    // This method is called when the Play button is clicked.
    // Make sure to assign this method to the OnClick event in the Button's Inspector.
    public void LoadGameScene()
    {
        // Replace "FirstScene" with the exact name of your main game scene.
        SceneManager.LoadScene("main");
    }
}

