using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPageScript : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionPanel;
    // Start is called before the first frame update
    void Start()
    {
        selectionPanel.SetActive(false);
    }
    public void ShowselectionPanel()
    {
        selectionPanel.SetActive(true);
    }

}
