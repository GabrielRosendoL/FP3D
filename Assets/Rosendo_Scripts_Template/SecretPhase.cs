using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretPhase : MonoBehaviour
{
    // Este método precisa ser público para aparecer no Unity
    public void StartSecretScene()
    {
        // Certifique-se de que o nome da cena está correto e corresponde ao nome da cena que você deseja carregar
        SceneManager.LoadScene("Secret");
    }
}
