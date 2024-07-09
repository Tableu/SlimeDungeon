/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csFogVisibilityAgent.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */

/*
 * This script is just an example of what you can do with the visibility check interface.
 * You can create whatever agent that you want based on this script.
 * Also, I recommend you to change the part where the FogWar module is fetched with Find()...
 */



using UnityEngine;                  // Monobehaviour
using System.Collections.Generic;   // List
using System.Linq;
using UnityEngine.Events; // ToList



namespace FischlWorks_FogWar
{



    /// An example of an monobehaviour agent that utilizes the public interfaces of csFogWar class.

    /// Fetches all MeshRenderers and SkinnedMeshRenderers of child objects,\n
    /// then enables / disables them based on each FogRevealer's FOV.
    public class csFogVisibilityAgent : MonoBehaviour
    {
        [SerializeField]
        private csFogWar fogWar = null;

        [SerializeField]
        private bool visibility = false;

        public bool Visible => visibility;
        
        [SerializeField]
        [Range(0, 2)]
        private int additionalRadius = 0;

        private List<MeshRenderer> meshRenderers = null;
        private List<SkinnedMeshRenderer> skinnedMeshRenderers = null;
        private List<ParticleSystemRenderer> particleSystems = null;
        private List<Light> lights = null;



        private void Start()
        {
            // This part is meant to be modified following the project's scene structure later...
            try
            {
                fogWar = GameObject.Find("FogWar").GetComponent<csFogWar>();
            }
            catch
            {
                Debug.LogErrorFormat("Failed to fetch csFogWar component. " +
                    "Please rename the gameobject that the module is attachted to as \"FogWar\", " +
                    "or change the implementation located in the csFogVisibilityAgent.cs script.");
            }

            meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            particleSystems = GetComponentsInChildren<ParticleSystemRenderer>().ToList();
            lights = GetComponentsInChildren<Light>().ToList();
        }



        private void Update()
        {
            if (fogWar == null || fogWar.CheckWorldGridRange(transform.position) == false)
            {
                return;
            }

            visibility = fogWar.CheckVisibility(transform.position, additionalRadius);

            foreach (MeshRenderer renderer in meshRenderers)
            {
                if(renderer != null)
                    renderer.enabled = visibility;
            }

            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                if(renderer != null)
                    renderer.enabled = visibility;
            }

            foreach (ParticleSystemRenderer system in particleSystems)
            {
                if (system != null)
                {
                    system.enabled = visibility;
                }
            }

            foreach (Light l in lights)
            {
                if (l != null)
                {
                    l.enabled = visibility;
                }
            }
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fogWar == null || Application.isPlaying == false)
            {
                return;
            }

            if (fogWar.CheckWorldGridRange(transform.position) == false)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(
                    new Vector3(
                        Mathf.RoundToInt(transform.position.x),
                        0,
                        Mathf.RoundToInt(transform.position.z)),
                    (fogWar._UnitScale / 2.0f) + additionalRadius);

                return;
            }

            if (fogWar.CheckVisibility(transform.position, additionalRadius) == true)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawWireSphere(
                new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    0,
                    Mathf.RoundToInt(transform.position.z)),
                (fogWar._UnitScale / 2.0f) + additionalRadius);
        }
#endif
    }



}