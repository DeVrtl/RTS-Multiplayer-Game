using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private GameObject _healthBarParent;
    [SerializeField] private Image _healthBar;

    private void Awake()
    {
        _health.ClientHealthUpdated += OnClientHealthUpdated;
    }

    private void OnDestroy()
    {
        _health.ClientHealthUpdated -= OnClientHealthUpdated;
    }

    private void OnMouseEnter()
    {
        _healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        _healthBarParent.SetActive(false);
    }

    private void OnClientHealthUpdated(int currentHealth, int maxHealth)
    {
        _healthBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
