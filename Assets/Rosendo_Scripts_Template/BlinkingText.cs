using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Para UI
    public float blinkSpeed = 0.5f; // Velocidade do piscar

    private void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>(); // Obtém o componente correto para UI
        }

        // Iniciar a animação de piscar
        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true) // Loop infinito
        {
            // Alterna entre visível e invisível
            textMeshPro.alpha = textMeshPro.alpha == 1 ? 0 : 1;

            // Espera pelo próximo ciclo
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
