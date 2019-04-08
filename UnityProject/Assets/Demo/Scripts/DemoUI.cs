using UnityEngine;
using UnityEngine.UI;

namespace UnifiedTextSize.Demo.Scripts
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField]
        private UnifyTextSize unifyTextSizeController;
        [SerializeField]
        private Text overfilledText;
        [SerializeField]
        private Text textToAdd;

        [Space]
        [SerializeField]
        private Button recalculateBestFitButton;
        [SerializeField]
        private Button addTextToControllerButton;

        private void Start()
        {
            recalculateBestFitButton.onClick.AddListener(OnRecalculateClicked);
            addTextToControllerButton.onClick.AddListener(OnAddTextClicked);
        }

        private void OnRecalculateClicked()
        {
            overfilledText.text = "The Font Resized!";
            unifyTextSizeController.RecalculateBestFit();
        }

        private void OnAddTextClicked()
        {
            unifyTextSizeController.AddText(textToAdd);
        }
    }
}
