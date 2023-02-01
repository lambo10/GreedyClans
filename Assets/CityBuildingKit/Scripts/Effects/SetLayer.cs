using UnityEngine;

namespace Effects
{
	public class SetLayer : MonoBehaviour {

		// Use this for initialization
		void Start () {
#if UNITY_2017			
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Foreground";
#endif	
		}
	}
}
