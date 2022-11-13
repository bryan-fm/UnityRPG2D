using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelClose : MonoBehaviour
{
    public GameObject panel;
    public bool hasBeenOpen; 
    // Start is called before the first frame update
    void Start()
    {
        if (Time.realtimeSinceStartup < 30){

            panel.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (panel.gameObject.activeInHierarchy) {
            StartCoroutine(CloseCO());
        }
    }

    public IEnumerator CloseCO()
    {   
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
