using UnityEngine;

public class StartRing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject ShipPrefab;
    public GameObject ShipInstance=null;

    void Start()
    {
        if (ShipInstance != null)
        {
            ShipInstance.transform.position = this.transform.position;
            ShipInstance.transform.rotation = this.transform.rotation;
            //We do it this way to preserve scale
            //TODO: We may wish to introduce an offset.
        }
        else
        {
            Instantiate(ShipPrefab, this.transform.position, this.transform.rotation);
            transform.SetParent(null, true);//Could mess things up, but...
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
