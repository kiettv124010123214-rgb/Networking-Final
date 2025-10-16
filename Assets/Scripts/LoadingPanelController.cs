using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadingSpeed = 0.01f; // Tốc độ tăng slider
    [SerializeField] private float updateInterval = 0.1f; // Khoảng thời gian cập nhật

    private void Awake()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
    }

    private void Start()
    {
        if (loadingSlider != null)
            loadingSlider.value = 0f;
        StartCoroutine(LoadingProgress());
    }

    private IEnumerator LoadingProgress()
    {
        if (loadingSlider == null || loadingPanel == null)
            yield break;

        while (loadingSlider.value < 1f)
        {
            loadingSlider.value += loadingSpeed;
            yield return new WaitForSeconds(updateInterval);
        }

        loadingSlider.value = 1f; // Đảm bảo slider đạt 100%
        loadingPanel.SetActive(false); // Tắt panel sau khi hoàn thành
    }
}