using UnityEngine;
using Vuforia;

public class BarcodeScanner : MonoBehaviour
{
    public BarcodeBehaviour barcodeBehaviour;
    public ARModelLoader modelLoader;

    private string lastID = "";

    void Update()
    {
        if (barcodeBehaviour == null)
            return;

        var instanceData = barcodeBehaviour.InstanceData;

        if (instanceData != null && !string.IsNullOrEmpty(instanceData.Text))
        {
            string id = instanceData.Text;

            if (id != lastID)
            {
                lastID = id;

                Debug.Log("QR detected: " + id);

                if (modelLoader != null)
                    modelLoader.OnQRCodeDetected(id);
            }
        }
    }
}