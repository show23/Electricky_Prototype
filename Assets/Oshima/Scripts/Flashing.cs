using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Material))]
public class Flashing : MonoBehaviour
{

    Material material;
    Color defaultColor;
    Color flashColor;

    void Start()

    {
        material = GetComponent<MeshRenderer>().materials[0];
        defaultColor = material.GetColor("_EmissionColor");
        flashColor = new Color(0, 0, 0, 255f);
        material.SetColor("_EmissionColor", flashColor);
    }



    void Update()

    {
        if (Input.GetKey(KeyCode.V))
        {//“K“–

            material.SetColor("_EmissionColor", flashColor);
        }
        else
        {
            material.SetColor("_EmissionColor", defaultColor);

        }

    }

}
