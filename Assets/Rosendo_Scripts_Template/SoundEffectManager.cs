using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public AudioSource audioSource; // O AudioSource que vai tocar o som
    public AudioClip clickSound; // O efeito sonoro para o clique

    // Função para tocar o som do botão
    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
