using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool isGreen = false;
    [HideInInspector]
    public GameManager gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Geting ground renderer
            Material mat = GetComponent<Renderer>().material;
            // Checking if the material is already green. 
            if(mat.color != Color.green)
            {
                mat.color = Color.green;
                // Removing non transform from the list in gameManager to
                // not spawn food on this tile
                gameManager.nonGreenGround.Remove(transform);
                // Decreasing the count of ground left to be green.
                gameManager.groundCount--;
            }

            isGreen = true;
        }
    }
}
