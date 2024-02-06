using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public Gradient gradient;
    public Image staminaImage;
    private float maxStamina;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetMaxStamina(float stamina){
        maxStamina = stamina;
        staminaImage.fillAmount= stamina/maxStamina;

        staminaImage.color = gradient.Evaluate(stamina/maxStamina);
    }

    public void SetStamina(float stamina){
        staminaImage.fillAmount = stamina/maxStamina;
        staminaImage.color = gradient.Evaluate(stamina/maxStamina);
    }
}
