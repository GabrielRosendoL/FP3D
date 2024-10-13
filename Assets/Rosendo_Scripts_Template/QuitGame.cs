using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Este método será chamado quando o botão de sair for clicado
    public void Quit()
    {
        // Se estiver no editor, ele não vai fechar a janela, mas no build ele fechará o jogo
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
