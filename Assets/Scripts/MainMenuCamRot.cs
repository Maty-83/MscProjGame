using UnityEngine;

public class MainMenuCamRot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float RotationSpeed = 45f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }
}
