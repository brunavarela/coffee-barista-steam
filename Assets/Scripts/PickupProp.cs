using UnityEngine;

// Base pra objetos "pegáveis" que devem voltar sozinhos pro lugar de origem
// quando soltos fora de um destino válido (xícara, jarra de leite).
public abstract class PickupProp : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected virtual void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ReturnToOriginalSpot()
    {
        transform.SetParent(null);
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
