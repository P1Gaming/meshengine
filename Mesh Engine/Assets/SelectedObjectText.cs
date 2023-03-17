using UnityEngine;
using TMPro;

public class SelectedObjectText : MonoBehaviour
{
    [SerializeField] TMP_Text blockText;

    private void OnEnable()
    {
        GameEvents.ChangeSelectedItemEvent += UpdateBlockSelected;
    }

    private void OnDisable()
    {
        GameEvents.ChangeSelectedItemEvent -= UpdateBlockSelected;
    }

    void UpdateBlockSelected(string selectedObject)
    {
        blockText.text = selectedObject;
    }
}
