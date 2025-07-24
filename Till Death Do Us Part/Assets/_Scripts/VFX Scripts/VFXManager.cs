using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public void TriggerFlashRed()
    {
        GetComponent<FlashRed>().Trigger(Color.softRed, 0.3f);
    }
}
