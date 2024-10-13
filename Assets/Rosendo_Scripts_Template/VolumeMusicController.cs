using UnityEngine;
using UnityEngine.UI;

public class VolumeMusicController : MonoBehaviour
{
    public GameObject settingsPanel; // Painel de configurações
    public Slider volumeSlider; // Slider de volume
    public AudioSource backgroundMusic; // Fonte de áudio da música

    private void Start()
    {
        // Esconder o painel de configurações inicialmente
        settingsPanel.SetActive(false);

        // Definir o volume inicial no slider
        if (backgroundMusic != null)
        {
            volumeSlider.value = backgroundMusic.volume;
        }

        // Adicionar listener para mudanças no slider de volume
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Função para abrir/fechar o painel de configurações
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // Função para ajustar o volume
    public void SetVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }
}
