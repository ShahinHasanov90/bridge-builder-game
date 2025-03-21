using UnityEngine;

public class BridgeElement : MonoBehaviour
{
    public enum ElementType
    {
        Beam,
        Joint,
        Cable
    }

    public ElementType type;
    public float strength = 100f;
    public float cost = 10f;
    public bool isPlaced = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ElementType elementType)
    {
        type = elementType;
        switch (type)
        {
            case ElementType.Beam:
                strength = 150f;
                cost = 20f;
                spriteRenderer.color = Color.brown;
                break;
            case ElementType.Joint:
                strength = 100f;
                cost = 10f;
                spriteRenderer.color = Color.gray;
                break;
            case ElementType.Cable:
                strength = 80f;
                cost = 15f;
                spriteRenderer.color = Color.yellow;
                break;
        }
    }

    public void Place()
    {
        isPlaced = true;
        rb.isKinematic = false;
    }

    public void Break()
    {
        if (strength <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlaced)
        {
            float impactForce = collision.relativeVelocity.magnitude;
            strength -= impactForce;
            Break();
        }
    }
}