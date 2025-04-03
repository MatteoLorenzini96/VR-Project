using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnowballThrow : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private XRGrabInteractable _grabInteractable; // Riferimento all'XRGrabInteractable
    [SerializeField] private Rigidbody _snowballRigidbody;         // Riferimento al Rigidbody della palla di neve

    [Header("Impostazioni di Lancio")]
    [SerializeField] private float _maxThrowSpeed = 12f;            // Velocità massima del lancio
    [SerializeField] private float _throwStrengthMultiplier = 3f; // Moltiplicatore per la forza del lancio
    [SerializeField] private float _linearDamping = 0.4f;           // Resistenza lineare (linearDamping) per rendere più realistico il volo
    [SerializeField] private float _angularDamping = 5f;          // Resistenza angolare (angularDamping)
    [SerializeField] private float _yThrowModifier = 3f;          // Componente per migliorare la parabola verso l'alto

    private Vector3 _previousPosition; // Posizione della palla nel frame precedente
    private Vector3 _throwDirection;   // Direzione del lancio
    private Vector3 _throwVelocity;    // Velocità del lancio

    private void Start()
    {
        if (_grabInteractable == null)
            _grabInteractable = GetComponent<XRGrabInteractable>();

        if (_snowballRigidbody == null)
            _snowballRigidbody = GetComponent<Rigidbody>();

        // Inizializzazione della posizione precedente
        _previousPosition = transform.position;

        // Imposta linearDamping e angularDamping
        _snowballRigidbody.linearDamping = _linearDamping;
        _snowballRigidbody.angularDamping = _angularDamping;
    }

    private void FixedUpdate()
    {
        // Solo se l'oggetto è in fase di presa (non rilasciato)
        if (_grabInteractable.isSelected)
        {
            // Calcola la velocità della mano durante il movimento (posizione corrente - posizione precedente)
            _throwVelocity = (transform.position - _previousPosition) / Time.deltaTime;
            _previousPosition = transform.position;
        }
    }

    // Questo metodo deve essere chiamato quando l'oggetto viene rilasciato
    public void OnRelease()
    {
        // Calcola la direzione del lancio in base alla velocità della mano
        _throwDirection = _throwVelocity.normalized;

        // Calcola la forza di lancio applicando un moltiplicatore
        Vector3 throwForce = _throwDirection * _throwVelocity.magnitude * _throwStrengthMultiplier;

        // Aggiungi velocità verso l'alto per simulare la parabola del lancio
        throwForce.y += _throwVelocity.y * _yThrowModifier; // Aumenta o diminuisci per regolare la parabola

        // Verifica se la velocità supera il limite massimo e regolala se necessario
        if (throwForce.magnitude > _maxThrowSpeed)
        {
            throwForce = throwForce.normalized * _maxThrowSpeed;
        }

        // Applica la forza alla palla
        _snowballRigidbody.linearVelocity = throwForce;

        // Aggiungi un po' di rotazione per rendere il lancio più naturale
        float spinStrength = 2f;  // Regola questo valore per un effetto di rotazione più o meno forte
        _snowballRigidbody.AddTorque(Vector3.Cross(_throwDirection, Vector3.up) * spinStrength);
    }
}
