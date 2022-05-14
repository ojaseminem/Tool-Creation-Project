using UnityEngine;

namespace Gun_Scene
{
    public class Recoil : MonoBehaviour
    {
        #region Variables
    
        //Bool
        private bool m_IsAiming;
        
        //Scripts
        //Use the below variable to input your player script controlling the ADS state.
        [SerializeField] private GunController gunControllerScript;
        
        //Rotations
        private Vector3 m_CurrentRotation;
        private Vector3 m_TargetRotation;
    
        //Hip fire Recoil
        [SerializeField] private float recoilX; //Regular rifle -2
        [SerializeField] private float recoilY; //Regular rifle 2
        [SerializeField] private float recoilZ; //Regular rifle 0.35
    
        //ADS Recoil
        [SerializeField] private float aimRecoilX; //Regular rifle -2
        [SerializeField] private float aimRecoilY; //Regular rifle 2
        [SerializeField] private float aimRecoilZ; //Regular rifle 0.35
        
        //Settings
        [SerializeField] private float snappiness; //Regular rifle 6
        [SerializeField] private float returnSpeed; //Regular rifle 2

        #endregion
    
        private void Update()
        {
            m_IsAiming = gunControllerScript.isAdsAiming; 
            
            m_TargetRotation = Vector3.Lerp(m_TargetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            m_CurrentRotation = Vector3.Slerp(m_CurrentRotation, m_TargetRotation, snappiness * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(m_CurrentRotation);
        }
    
        //Call this function inside your gun script. Cache this script for better optimization;
        public void RecoilFire()
        {
            if(m_IsAiming) m_TargetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
            else m_TargetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
    }
}
