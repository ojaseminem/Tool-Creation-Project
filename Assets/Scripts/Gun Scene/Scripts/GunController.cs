using System.Collections;
using UnityEngine;

namespace Gun_Scene
{
    public class GunController : MonoBehaviour
    {
        #region Variables
        
        //Constant Variables
        [Header("Gun Settings")] 
        public float fireRate = 0.1f;
        public int clipSize = 30;
        public int reservedAmmoCapacity = 270;
        
        //Dynamic Variables
        private bool m_CanShoot;
        private int m_CurrentAmmoInClip;
        private int m_AmmoInReserve;
        
        //Muzzle Flash
        public GameObject muzzleFlashParticleFX;
        
        //ADS
        public bool isAdsAiming;
        public Vector3 normalLocalPos;
        public Vector3 aimingLocalPos;
        public float aimSmoothAmount = 10;

        [Header("Mouse Settings")]
        public float mouseSensitivity = 1;
        private Vector2 m_CurrentRotation;
        public float weaponSwayAmount = 10;
        
        //Weapon Recoil
        public bool randomizeRecoil;
        public Vector2 randomRecoilConstraints;
        //necessary if randomize recoil is off
        public Vector2[] recoilPattern;
        
        //Scripts
        public Recoil recoilScript;
        
        #endregion

        private void Start()
        {
            m_CurrentAmmoInClip = clipSize;
            m_AmmoInReserve = reservedAmmoCapacity;
            m_CanShoot = true;
            
        }

        private void Update()
        {
            DetermineAim();
            DetermineRotation();
            if (Input.GetMouseButton(0) && m_CanShoot && m_CurrentAmmoInClip > 0)
            {
                m_CanShoot = false;
                m_CurrentAmmoInClip--;
                StartCoroutine(Shoot());
            }
            else if (Input.GetKey(KeyCode.R) && m_CurrentAmmoInClip < clipSize && m_AmmoInReserve > 0)
            {
                int amountNeeded = clipSize - m_CurrentAmmoInClip;
                if (amountNeeded >= m_AmmoInReserve)
                {
                    m_CurrentAmmoInClip += m_AmmoInReserve;
                    m_AmmoInReserve -= amountNeeded;
                }
                else
                {
                    m_CurrentAmmoInClip = clipSize;
                    m_AmmoInReserve -= amountNeeded;
                }
            }
        }

        IEnumerator Shoot()
        {
            //DetermineRecoil();
            
            StartCoroutine(MuzzleFlash());
            
            recoilScript.RecoilFire();

            DetermineHit();
            
            yield return new WaitForSeconds(fireRate);
            m_CanShoot = true;
        }
        
        private void DetermineRotation()
        {
            Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            mouseAxis *= mouseSensitivity;
            m_CurrentRotation += mouseAxis;

            m_CurrentRotation.y = Mathf.Clamp(m_CurrentRotation.y, -70, 70);

            transform.localPosition += (Vector3) mouseAxis * weaponSwayAmount / 1000;
            
            transform.root.localRotation = Quaternion.AngleAxis(m_CurrentRotation.x, Vector3.up);
            transform.parent.localRotation = Quaternion.AngleAxis(-m_CurrentRotation.y, Vector3.right);

        }
        
        private void DetermineHit()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit,
                1 << LayerMask.NameToLayer("Enemy")))
            {
                try
                {
                    if (hit.collider.CompareTag("Player")) return;
                    else
                    {
                        Debug.Log("Hit an Enemy");
                        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                        rb.constraints = RigidbodyConstraints.None;
                        rb.AddForce(transform.parent.transform.forward * 500);
                    }
                }
                catch
                {
                    // ignored
                }
                /*if(!hit.collider.CompareTag("Player"))
                    Debug.Log(hit.collider.name);
                
                if (hit.collider.transform.GetComponent<Rigidbody>() != null && hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Hit an Enemy");
                    Debug.Log(hit.collider.name);
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    rb.constraints = RigidbodyConstraints.None;
                    rb.AddForce(transform.parent.transform.forward * 500);
                }*/
            }
        }

        IEnumerator MuzzleFlash()
        {
            muzzleFlashParticleFX.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            muzzleFlashParticleFX.SetActive(false);
        }
        
        private void DetermineAim()
        {
            Vector3 target = normalLocalPos;
            if (Input.GetMouseButton(1))
            {
                isAdsAiming = true;
                target = aimingLocalPos;
            }
            else
            {
                isAdsAiming = false;
            }

            Vector3 desiredPos = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothAmount);
            
            transform.localPosition = desiredPos;
        }
        
        /*private void DetermineRecoil()
        {
            transform.localPosition -= Vector3.forward * 0.1f;

            if (randomizeRecoil)
            {
                float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
                float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

                Vector2 recoil = new Vector2(xRecoil, yRecoil);

                m_CurrentRotation += recoil;
            }
            else
            {
                int currentStep = clipSize + 1 - m_CurrentAmmoInClip;
                currentStep = Mathf.Clamp(currentStep, 0, recoilPattern.Length - 1);

                m_CurrentRotation += recoilPattern[currentStep];
            }
        }*/

    }
}
