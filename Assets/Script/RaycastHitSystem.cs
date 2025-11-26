using UnityEngine;
using UnityEngine.UI;

public class RaycastHitSystem : MonoBehaviour
{
    [Header("けけ管理")]
    [Tooltip("けけ管理 判定の長さ")]
    [SerializeField] private float rayLength = 10f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Color rayColor = Color.red;
    [Header("かみむー管理")]
    [SerializeField] private GameObject FocusImage;
    [SerializeField] private GameObject fallenBridges;
    [SerializeField] private SC_FPSController sC_FPSController;
    
    private const float delayTime = 5f;

    void Update()
    {
        CastRay();
    }

    void CastRay()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, rayColor);

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if (hit.collider.gameObject.name == "Bridge01_LOD0")
            {
                MeshCollider meshCollider = hit.collider.GetComponent<MeshCollider>();
                Image image = FocusImage.GetComponent<Image>();

                if (meshCollider != null)
                {
                    if (!meshCollider.enabled)
                    {
                        image.color = Color.yellow;

                        if (Input.GetMouseButtonDown(0))
                        {
                            sC_FPSController.TakesPic();
                            
                            SetLayer(hit.collider.gameObject, "All");
                            fallenBridges.SetActive(false);

                            // Instead of using a coroutine, call TimerManager to handle time tracking
                            TimerManager.Instance.StartTimer(delayTime, hit.collider.gameObject, "Old", fallenBridges);
                            sC_FPSController.SwitchCamera();
                        }
                    }
                    else
                    {
                        image.color = Color.red;

                        if (Input.GetMouseButtonDown(0))
                        {
                            sC_FPSController.TakesPic();

                            fallenBridges.SetActive(true);
                            DisableMeshCollider(hit.collider.gameObject, "Old");
                            sC_FPSController.SwitchCamera();
                        }
                    }
                }

                // Enable and move FocusImage to the center of the hit object (converted to UI position)
                if (FocusImage != null)
                {
                    FocusImage.SetActive(true);
                    Vector3 objectCenter = hit.collider.bounds.center;
                    Vector3 screenPoint = playerCamera.WorldToScreenPoint(objectCenter);
                    FocusImage.GetComponent<RectTransform>().position = screenPoint;
                }
            }
        }
        else
        {
            // Disable FocusImage if nothing is hit
            if (FocusImage != null)
            {
                FocusImage.SetActive(false);
            }
        }
    }

    void SetLayer(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }
    }

    void DisableMeshCollider(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }
    }
}
