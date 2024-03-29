using TMPro;
using UnityEngine;

public class Iglu : MonoBehaviour
{
    public PenguinController penguinController;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.layer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision){
        if (collision.gameObject.layer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
        }
    }

    void HandlePenguinCollision(Collision2D collision){
        if(collision.gameObject == penguinController.GetEggPenguin()){
            text.text = "PENGUINS WIN";
        }
    }
}
