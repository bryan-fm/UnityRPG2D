using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{

    public GameObject UiScreen;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        if(UiFade.instance == null) {
            UiFade.instance = Instantiate(UiScreen).GetComponent<UiFade>();
        }

        if(PlayerController.instance == null) {
            PlayerController clone = Instantiate(Player).GetComponent<PlayerController>();
            PlayerController.instance = clone;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
