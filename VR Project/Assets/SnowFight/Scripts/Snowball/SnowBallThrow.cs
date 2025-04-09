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
    [SerializeField] private float _throwStrengthMultiplier = 3f;   // Moltiplicatore per la forza del lancio
    [SerializeField] private float _linearDamping = 0.4f;           // Resistenza lineare (linearDamping) per rendere più realistico il volo
    [SerializeField] private float _angularDamping = 5f;           // Resistenza angolare (angularDamping)
    [SerializeField] private float _yThrowModifier = 3f;            // Componente per migliorare la parabola verso l'alto
    [SerializeField] private float _velocitySmoothingFactor = 0.1f; // Fattore di smoothing per la velocità

    private Vector3 _previousPosition; // Posizione della palla nel frame precedente
    private Vector3 _throwDirection;   // Direzione del lancio
    private Vector3 _throwVelocity;    // Velocità della mano durante il movimento
    private Vector3 _smoothedThrowVelocity; // Velocità smussata della mano

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

            // Applicare il filtro per smussare la velocità
            _smoothedThrowVelocity = Vector3.Lerp(_smoothedThrowVelocity, _throwVelocity, _velocitySmoothingFactor);

            _previousPosition = transform.position;
        }
    }

    // Questo metodo deve essere chiamato quando l'oggetto viene rilasciato
    public void OnRelease()
    {
        // Calcola la direzione del lancio in base alla velocità smussata della mano
        _throwDirection = _smoothedThrowVelocity.normalized;

        // Calcola la forza di lancio applicando un moltiplicatore
        Vector3 throwForce = _throwDirection * _smoothedThrowVelocity.magnitude * _throwStrengthMultiplier;

        // Aggiungi velocità verso l'alto per simulare la parabola del lancio
        throwForce.y += _smoothedThrowVelocity.y * _yThrowModifier; // Aumenta o diminuisci per regolare la parabola

        // Verifica se la velocità supera il limite massimo e regolala se necessario
        if (throwForce.magnitude > _maxThrowSpeed)
        {
            throwForce = throwForce.normalized * _maxThrowSpeed;
        }

        // Applica la forza alla palla (usa la velocità invece di linearVelocity)
        _snowballRigidbody.linearVelocity = throwForce;

       /* // Aggiungi un po' di rotazione per rendere il lancio più naturale
        float spinStrength = 2f;  // Regola questo valore per un effetto di rotazione più o meno forte
        Vector3 spinDirection = Vector3.Cross(_throwDirection, Vector3.up);  // Calcola la direzione della rotazione
        _snowballRigidbody.AddTorque(spinDirection * spinStrength);*/

        // Log per il debug
        //Debug.Log("Throw Direction: " + _throwDirection);
        //Debug.Log("Smoothed Throw Velocity: " + _smoothedThrowVelocity);
    }
}
