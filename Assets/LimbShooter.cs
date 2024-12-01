using UnityEngine;

public class LimbShooter : MonoBehaviour 
{
    private ItemSO limbData;
    private float nextFireTime = 0f;
    private Transform shootPoint; // The tip/end of the limb

    public void Configure(ItemSO data, Transform endPoint)
    {
        limbData = data;
        shootPoint = endPoint;
    }

    void Update()
    {
        if (limbData != null && limbData.isGun && Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            FireGun();
        }
    }

    void FireGun()
    {
        if (limbData.bulletPrefab != null && shootPoint != null)
        {
                    GameObject bullet = Instantiate(limbData.bulletPrefab, shootPoint.position, 
            shootPoint.rotation * Quaternion.Euler(0, 0, -90));
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null)
            {
                // Use the limb's forward direction for shooting
                bulletRb.linearVelocity = -1f*shootPoint.up * limbData.bulletSpeed;
            }

            bullet.tag = "MonsterInstaKill";
            nextFireTime = Time.time + limbData.fireRate;
        }
    }
}