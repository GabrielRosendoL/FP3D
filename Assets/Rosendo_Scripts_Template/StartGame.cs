using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Este método precisa ser público para aparecer no Unity
    public void StartGameScene()
    {
        // Certifique-se de que o nome da cena está correto e corresponde ao nome da cena que você deseja carregar
        SceneManager.LoadScene("Game");
    }
}
