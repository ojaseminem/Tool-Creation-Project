using System;
using UnityEditor.Timeline;
using UnityEngine;

namespace Controllers.CameraSeeThroughObstructions
{
    public class CameraSeeThroughObstructionsController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float checkDistance = 5f;

        private Transform m_Obstruction;
        private float m_ZoomSpeed = 2f;

        private void LateUpdate()
        {
            SeeThroughObstructions();
        }

        private void SeeThroughObstructions()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, player.position - transform.position, out hit, checkDistance))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    m_Obstruction = hit.transform;
                    m_Obstruction.GetComponent<MeshRenderer>().shadowCastingMode =
                        UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    
                    if (Vector3.Distance(m_Obstruction.position, transform.position) >= 3f &&
                        Vector3.Distance(transform.position, player.position) >= 1.5f)
                    {
                        transform.Translate(Vector3.forward * m_ZoomSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    if (!(m_Obstruction is null))
                        m_Obstruction.transform.GetComponent<MeshRenderer>().shadowCastingMode =
                            UnityEngine.Rendering.ShadowCastingMode.On;
                    if (Vector3.Distance(transform.position, player.position) < 4.5f)
                    {
                        transform.Translate(Vector3.back * m_ZoomSpeed * Time.deltaTime);
                    }
                }
            }
        }
        
    }
}
