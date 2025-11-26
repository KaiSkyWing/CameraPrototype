using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance; // Singleton pattern
    private float elapsedTime;
    private bool isRunning;
    private GameObject targetObject;
    private string targetLayer;

    [SerializeField] private  Text timeText; // Assign this in the inspector

    private GameObject fallenBridges;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTimer(float duration, GameObject obj, string layerName, GameObject fb)
    {
        targetObject = obj;
        targetLayer = layerName;
        elapsedTime = 0f;
        isRunning = true;
        fallenBridges = fb;

        StopAllCoroutines();
        StartCoroutine(TimerCoroutine(duration));
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (timeText != null)
            {
                timeText.text = elapsedTime.ToString("f1");
            }
            yield return null;
        }
        /*
        DisableMeshCollider(targetObject, targetLayer);
        fallenBridges.SetActive(true);
        ResetTimer();
        */
    }

    private void DisableMeshCollider(GameObject obj, string layerName)
    {
        if (obj == null) return;

        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        if (timeText != null)
        {
            timeText.text = "0.0";
        }
    }
}
