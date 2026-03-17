using UnityEngine;
using GLTFast;

public enum ARState { ScanQR, Downloading, ShowingModel, Error }

public class ARModelLoader : MonoBehaviour
{
    [Header("Scene References")]
    public Transform modelAnchor;

    [Header("Web Settings")]
    public string serverUrl = "https://api.modvision.uk/products/";

    private ARState currentState = ARState.ScanQR;
    private GameObject spawnedModel;

    private string lastScannedID = "";
    private bool isLoading = false;

    void Start()
    {
    }

    public void OnQRCodeDetected(string id)
    {
        if (isLoading || id == lastScannedID)
            return;

        lastScannedID = id;
        isLoading = true;

        LoadModel(id);
    }

    async void LoadModel(string partId)
    {
        currentState = ARState.Downloading;

        string url = $"{serverUrl}{partId}/model";
        var gltf = new GltfImport();
        bool success = await gltf.Load(url);

        if (!success)
        {
            currentState = ARState.Error;
            isLoading = false;
            return;
        }

        GameObject modelHolder = new GameObject("AR_Model");
        bool spawned = await gltf.InstantiateMainSceneAsync(modelHolder.transform);

        if (!spawned)
            spawned = await gltf.InstantiateSceneAsync(modelHolder.transform, 0);

        if (spawned && modelHolder.transform.childCount > 0)
        {
            spawnedModel = modelHolder;

            spawnedModel.transform.SetParent(modelAnchor);
            spawnedModel.transform.localPosition = Vector3.zero;
            spawnedModel.transform.localRotation = Quaternion.identity;
            spawnedModel.transform.localScale = Vector3.one * 0.5f;

            var interaction = modelAnchor.GetComponent<ModelInteraction>();

            if (interaction != null)
            {
                interaction.SetModel(spawnedModel.transform);
            }

            currentState = ARState.ShowingModel;
        }
        else
        {
            Destroy(modelHolder);
            currentState = ARState.Error;
        }

        isLoading = false;
    }

    void ResetApp()
    {
        if (spawnedModel != null)
            Destroy(spawnedModel);

        lastScannedID = "";
        currentState = ARState.ScanQR;
        isLoading = false;
    }

    void OnGUI()
    {
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 55;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;

        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        if (currentState == ARState.ScanQR)
        {
            GUI.Label(new Rect(0, centerY - 300, Screen.width, 100), "Scan QR", textStyle);
        }
        else if (currentState == ARState.Downloading)
        {
            GUI.Label(new Rect(0, centerY - 300, Screen.width, 100), "Downloading...", textStyle);
        }
        else if (currentState == ARState.ShowingModel)
        {
            float btnWidth = 350;
            float btnHeight = 150;

            GUI.backgroundColor = Color.red;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 45;

            if (GUI.Button(new Rect(centerX - btnWidth / 2, Screen.height - 250, btnWidth, btnHeight), "RESET", btnStyle))
            {
                ResetApp();
            }
        }
        else if (currentState == ARState.Error)
        {
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 40;

            if (GUI.Button(new Rect(centerX - 200, centerY - 100, 400, 150), "Error. Try Again?", btnStyle))
            {
                ResetApp();
            }
        }
    }
}