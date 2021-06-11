using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdGameGround : MonoBehaviour
{
    private bool isGreen = false;
    [HideInInspector]
    public thirdGameManager gameManager;

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
                // Decreasing the count of ground left to be green.
                gameManager.nonGreenGround.Remove(transform);
                gameManager.groundCount--;
            }

            isGreen = true;
        }
    }
}
