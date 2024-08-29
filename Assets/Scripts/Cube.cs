using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour, IInteractable
{
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    void Update()
    {
        
    }
    public void Interact()
    {
        ChangeColorCube();
    }

    private void ChangeColorCube()
    {
        if (meshRenderer !=null)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            meshRenderer.material.color = randomColor;
        }
        else
        {
            Debug.Log("Küpteki Mesh Renderer bulunamadý.");
        }
    }
}
