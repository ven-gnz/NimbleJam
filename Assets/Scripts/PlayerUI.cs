using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{



    [SerializeField] private TextMeshProUGUI dirtCountText;

    public void UpdateDirtCount(int count)
    {
        dirtCountText.text = $"Dirt: {count}";
    }

}
