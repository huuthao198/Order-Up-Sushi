using UnityEngine;

public class ToggleSetting : MonoBehaviour
{
    [SerializeField] private GameObject iconOff;
    [SerializeField] private GameObject toggleImgOff;
    [SerializeField] private GameObject toggleImgOn;

    public void OnToggle(bool value)
    {
        iconOff.SetActive(!value);
        toggleImgOff.SetActive(!value);
        toggleImgOn.SetActive(value);
    }
}
