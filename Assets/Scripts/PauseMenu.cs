using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // O painel do menu de pausa
    public CubeController cubeController; // Referência ao controlador do cubo
    private bool isPaused = false; // Controla o estado do jogo (pausado ou não)

    // Update é chamado uma vez por frame
    void Update()
    {
        // Detecta se o jogador pressionou a tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Função para continuar o jogo (despausar)
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Esconde o menu de pausa
        Time.timeScale = 1f; // Retoma o tempo do jogo
        cubeController.ResumeGame(); // Chama o método para retomar a física do personagem
        isPaused = false; // Marca o jogo como despausado
    }

    // Função para pausar o jogo
    void Pause()
    {
        pauseMenuUI.SetActive(true); // Mostra o menu de pausa
        Time.timeScale = 0f; // Congela o tempo do jogo
        cubeController.PauseGame(); // Chama o método para pausar a física do personagem
        isPaused = true; // Marca o jogo como pausado
    }

    // Função para sair do jogo (voltar ao menu principal ou encerrar)
    public void QuitGame()
    {
        // Aqui você pode carregar a cena do menu principal ou sair completamente do jogo
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
