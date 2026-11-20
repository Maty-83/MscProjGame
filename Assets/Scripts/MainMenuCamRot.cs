using UnityEngine;

public class MainMenuCamRot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float RotationSpeed = 45f;
    void Start()
    {
        //likely unnecessary, but trying to fix a bug where rotation doesn't start when coming back from another scene.
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }
}
