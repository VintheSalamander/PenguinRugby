using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggLifeController : MonoBehaviour
{
    public GameObject eggLife1;
    public GameObject eggLife2;
    public GameObject eggLife3;

    private static int HP;
    // Start is called before the first frame update
    void Start()
    {
        HP = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if(HP == 2){
            Destroy(eggLife1);
            Debug.Log("2 EGG LEFT");
        }else if(HP == 1){
            Destroy(eggLife2);
            Debug.Log("1 EGG LEFT");
        }else if(HP == 0){
            Destroy(eggLife3);
            Debug.Log("SEALS WIN");
        }
    }

    public static void EggDown(){
        HP--;
    }
}
