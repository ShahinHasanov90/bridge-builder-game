using UnityEngine;
using System.Collections.Generic;

public class BridgePhysics : MonoBehaviour
{
    [Header("Joint Settings")]
    public float breakForce = 1000f;
    public float breakTorque = 1000f;
    
    [Header("Physics Settings")]
    public float gravityScale = 1f;
    public float dampingRatio = 0.5f;
    public float frequency = 2f;

    private List<BridgeElement> connectedElements = new List<BridgeElement>();
    private List<DistanceJoint2D> joints = new List<DistanceJoint2D>();

    public void ConnectElements(BridgeElement element1, BridgeElement element2)
    {
        if (element1 == null || element2 == null) return;

        // Elemanları listeye ekle
        if (!connectedElements.Contains(element1)) connectedElements.Add(element1);
        if (!connectedElements.Contains(element2)) connectedElements.Add(element2);

        // Fiziksel bağlantıyı oluştur
        CreatePhysicalConnection(element1, element2);
    }

    void CreatePhysicalConnection(BridgeElement element1, BridgeElement element2)
    {
        // Distance Joint oluştur
        DistanceJoint2D joint = element1.gameObject.AddComponent<DistanceJoint2D>();
        joint.connectedBody = element2.GetComponent<Rigidbody2D>();
        
        // Joint ayarlarını yap
        joint.autoConfigureDistance = false;
        joint.distance = Vector2.Distance(element1.transform.position, element2.transform.position);
        joint.maxDistanceOnly = false;
        
        // Spring ayarları
        joint.enableCollision = true;
        joint.breakForce = breakForce;
        joint.breakTorque = breakTorque;

        // Spring joint ayarları
        SpringJoint2D springJoint = element1.gameObject.AddComponent<SpringJoint2D>();
        springJoint.connectedBody = element2.GetComponent<Rigidbody2D>();
        springJoint.autoConfigureDistance = false;
        springJoint.distance = joint.distance;
        springJoint.dampingRatio = dampingRatio;
        springJoint.frequency = frequency;
        
        joints.Add(joint);
    }

    public void ApplyStress(float force)
    {
        foreach (var element in connectedElements)
        {
            if (element != null)
            {
                // Elemanın dayanıklılığını azalt
                element.strength -= force * Time.deltaTime;
                
                // Dayanıklılık kontrolü
                if (element.strength <= 0)
                {
                    BreakElement(element);
                }
            }
        }
    }

    void BreakElement(BridgeElement element)
    {
        // Bağlantıları kaldır
        DistanceJoint2D[] elementJoints = element.GetComponents<DistanceJoint2D>();
        foreach (var joint in elementJoints)
        {
            joints.Remove(joint);
            Destroy(joint);
        }

        // Spring jointları kaldır
        SpringJoint2D[] springJoints = element.GetComponents<SpringJoint2D>();
        foreach (var joint in springJoints)
        {
            Destroy(joint);
        }

        // Elementi listeden çıkar
        connectedElements.Remove(element);

        // Parçalanma efekti
        CreateBreakEffect(element);

        // Elementi yok et
        Destroy(element.gameObject);
    }

    void CreateBreakEffect(BridgeElement element)
    {
        // Parçacık sistemi oluştur
        GameObject breakEffect = new GameObject("BreakEffect");
        breakEffect.transform.position = element.transform.position;

        ParticleSystem particles = breakEffect.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startSpeed = 5f;
        main.startSize = 0.1f;
        main.startLifetime = 1f;
        main.maxParticles = 50;

        // Renk ayarları
        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.gray, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = gradient;

        // Efekti başlat
        particles.Play();

        // Efekti temizle
        Destroy(breakEffect, main.startLifetime.constant);
    }

    public void ResetPhysics()
    {
        // Tüm jointları temizle
        foreach (var joint in joints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }
        joints.Clear();
        connectedElements.Clear();
    }

    public float GetTotalStress()
    {
        float totalStress = 0f;
        foreach (var joint in joints)
        {
            if (joint != null)
            {
                totalStress += joint.reactionForce.magnitude;
            }
        }
        return totalStress;
    }
}