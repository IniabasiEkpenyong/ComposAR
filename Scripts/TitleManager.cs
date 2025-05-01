using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void GoToLearnMode()
    {
        SceneManager.LoadScene("LearnScreen");
    }

    public void GoToComposeMode()
    {
        SceneManager.LoadScene("ComposeScreen");
    }

    public void GoToHelp() {
        SceneManager.LoadScene("HelpScreen");
    }
}

