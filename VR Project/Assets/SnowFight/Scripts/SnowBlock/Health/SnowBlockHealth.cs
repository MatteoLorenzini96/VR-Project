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

    [Header("Renderer dell'oggetto con il materiale overlay")]
    [SerializeField] private Renderer _overlayRenderer;
    [Header("Indice del materiale da modificare")]
    [SerializeField] private int _materialIndexToModify = 1;

    private Material _overlayMaterial;

    private void Start()
    {
        _healthManager = GetComponent<HealthManager>();

        VFXManager.Instance.SpawnEffect(_snowBlockCreationVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_snowBlockCreationSFXName);

        if (_overlayRenderer == null)
        {
            Debug.LogError("Renderer non assegnato per il materiale Overlay.");
            return;
        }

        Material[] materials = _overlayRenderer.materials;

        if (_materialIndexToModify < 0 || _materialIndexToModify >= materials.Length)
        {
            Debug.LogError("Indice del materiale fuori range.");
            return;
        }

        _overlayMaterial = materials[_materialIndexToModify];
    }


    public void GettingDamaged()
    {
        switch (_healthManager.lives)
        {
            case 3:
                //Debug.Log("Vita 3: Niente accade");
                _overlayMaterial.SetFloat("_Damage", 0f);

                break;
            case 2:
                //Debug.Log("Vita 2: Riduci la visibilità");
                AudioManager.Instance.PlaySFX(_snowBlockDamagedSFXName);
                _overlayMaterial.SetFloat("_Damage", 0.5f);

                break;
            case 1:
                //Debug.Log("Vita 1: Block si avvicina al danno massimo");
                AudioManager.Instance.PlaySFX(_snowBlockDamagedSFXName);
                _overlayMaterial.SetFloat("_Damage", 1f);

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
