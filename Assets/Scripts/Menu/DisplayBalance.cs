using UnityEngine;
using UnityEngine.UI;

public class DisplayBalance : MonoBehaviour
{
    public Text text;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        UpdateBalance();
        PersistentCardGame.balanceChanged += OnBalanceChanged;
    }

    private void UpdateBalance()
    {
        text.text = PersistentCardGame.GetBalance().ToString();
    }

    private void OnBalanceChanged()
    {
        UpdateBalance();
    }

    private void OnDestroy()
    {
        PersistentCardGame.balanceChanged -= OnBalanceChanged;
    }
}
