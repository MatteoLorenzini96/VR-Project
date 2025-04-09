using UnityEngine;

public class SnowBlockHealth : MonoBehaviour
{
    [Header("Creazione VFX and SFX")]
    [SerializeField] private string _snowBlockCreationVFXName = "SnowBlockCreationEffect";
    [SerializeField] private string _snowBlockCreationSFXName = "SnowBlockCreationSound";

    [Header("Danneggiato SFX")]
    [SerializeField] private string _snowBlockDamagedSFXName = "SnowBlockDamagedSound";

    [Header("Distruzione VFX and SFX")]
    [SerializeField] private string _snowBlockDestructionVFXName = "SnowBlockDestructionEffect";
    [SerializeField] private string _snowBlockDestructionSFXName = "SnowBlockDestructionSound";

    private HealthManager _healthManager;

    private void Start()
    {
        _healthManager = GetComponent<HealthManager>();

        VFXManager.Instance.SpawnEffect(_snowBlockCreationVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_snowBlockCreationSFXName);
    }

    public void GettingDamaged()
    {
        switch (_healthManager.lives)
        {
            case 3:
                Debug.Log("Vita 3: Niente accade");
                // Puoi aggiungere il codice che desideri qui
                break;
            case 2:
                Debug.Log("Vita 2: Riduci la visibilità");

                AudioManager.Instance.PlaySFX(_snowBlockDamagedSFXName);
                // Puoi aggiungere il codice che desideri qui
                break;
            case 1:
                Debug.Log("Vita 1: Block si avvicina al danno massimo");

                AudioManager.Instance.PlaySFX(_snowBlockDamagedSFXName);
                // Puoi aggiungere il codice che desideri qui
                break;
            case 0:
                Debug.Log("Vita 0: Il blocco è distrutto!");

                VFXManager.Instance.SpawnEffect(_snowBlockDestructionVFXName, transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySFX(_snowBlockDestructionSFXName);

                // Puoi aggiungere il codice che desideri qui (ad esempio, distruggere l'oggetto)
                Destroy(gameObject); // Distrugge il blocco quando la vita è 0
                break;
            default:
                Debug.Log("Valore di lives non valido.");
                break;
        }
    }
}
