using UnityEngine;

namespace Effects
{
	public class Scale : MonoBehaviour {		//scales some of the particle effects

#if UNITY_2017
		public ParticleEmitter[] emitters;
#else
		public ParticleSystem[] emitters;
#endif		
		public float scale = 1;

		private float[] _minSize;
		private float[] _maxSize;
		private Vector3[] _worldVelocity;
		private Vector3[] _localVelocity;
		private Vector3[] _rndVelocity;
		private Vector3[] _scaleBackUp;

		void Start () {

			ScaleEmitters ();

		}

		private void ScaleEmitters()
		{
			int length = emitters.Length;
		
			_minSize = new float[length];
			_maxSize = new float[length];
			_worldVelocity = new Vector3[length];
			_localVelocity = new Vector3[length];
			_rndVelocity = new Vector3[length];
			_scaleBackUp = new Vector3[length];
		
			for (int i = 0; i < length; i++) 
			{ 
#if UNITY_2017	
			minSize[i] = emitters[i].minSize;
			maxSize[i] = emitters[i].maxSize;
			worldVelocity[i] = emitters[i].worldVelocity;
			localVelocity[i] = emitters[i].localVelocity;
			rndVelocity[i] = emitters[i].rndVelocity;
#endif			
				_scaleBackUp[i] = emitters[i].transform.localScale;

#if UNITY_2017			
			emitters[i].minSize = minSize[i] * scale;
			emitters[i].maxSize = maxSize[i] * scale;
			emitters[i].worldVelocity = worldVelocity[i] * scale;
			emitters[i].localVelocity = localVelocity[i] * scale;
			emitters[i].rndVelocity = rndVelocity[i] * scale;
#endif
				emitters[i].transform.localScale = _scaleBackUp[i] * scale;			
			}
		}
	}
}