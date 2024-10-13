using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialPreGameManager : MonoBehaviour
{
    public GameObject tutorialPanel; // O painel de tutorial com instruções
    public Button okayButton; // Botão "Okay" no painel de tutorial
    public TextMeshProUGUI countdownText; // Texto da contagem regressiva
    public float countdownDuration = 3f; // Duração da contagem regressiva (3 segundos)

    private void Start()
    {
        // Congelar o jogo
        Time.timeScale = 0;

        // Mostrar o painel de tutorial
        tutorialPanel.SetActive(true);

        // Esconder o texto de contagem regressiva inicialmente
        countdownText.gameObject.SetActive(false);

        // Adicionar o evento ao botão "Okay"
        okayButton.onClick.AddListener(OnOkayClicked);
    }

    // Função chamada ao clicar no botão "Okay"
    void OnOkayClicked()
    {
        // Esconder o painel de tutorial
        tutorialPanel.SetActive(false);

        // Exibir o texto de contagem regressiva
        countdownText.gameObject.SetActive(true);

        // Começar a contagem regressiva
        StartCoroutine(StartCountdown());
    }

    // Contagem regressiva para iniciar o jogo
    IEnumerator StartCountdown()
    {
        float countdown = countdownDuration;

        while (countdown > 0)
        {
            countdownText.text = Mathf.Ceil(countdown).ToString(); // Mostra o número inteiro mais próximo
            yield return new WaitForSecondsRealtime(1f); // Usa tempo real para contagem regressiva, ignorando o Time.timeScale
            countdown--;
        }

        // Esconder o texto da contagem regressiva
        countdownText.gameObject.SetActive(false);

        // Iniciar o jogo
        StartGame();
    }

    // Função para iniciar o jogo
    void StartGame()
    {
        // Descongela o jogo
        Time.timeScale = 1;
    }
}
