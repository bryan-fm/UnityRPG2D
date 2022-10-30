using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{

    public GameObject UiScreen;
    public GameObject Player;
    public GameObject gameMan;

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

        if(GameManager.instance == null) {
            Instantiate(gameMan);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
