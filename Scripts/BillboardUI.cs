using UnityEngine;
using TMPro;

public class BillboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }

    public void SetPlayerName(string name)
    {
        if (nameText != null)
            nameText.text = name;
    }
}