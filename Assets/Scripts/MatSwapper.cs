using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MatSwapper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<MeshRenderer> Meshes;
    public List<Material> materials;
    [Min(0.001f)]
    public float SwapTime = 2;//Time between material swaps in seconds.
    private int ord = 0;
    void Start()
    {
        StartCoroutine(DoSwap());
    }

    private IEnumerator DoSwap()
    {
        while (true)
        {
            foreach (MeshRenderer renderer in Meshes)
            {
                renderer.material = materials[ord];
            }
            ord=(ord+1)%materials.Count;
            yield return new WaitForSeconds(SwapTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
