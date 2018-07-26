using UnityEngine;

	[RequireComponent(typeof(Light))]

	public class TorchFlicker : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The amount of flicker.")]
		[Range(0f, 2f)]
		private float _flickerAmount = 0.5f;

		[SerializeField]
		[Tooltip("The time it takes for light to make a flicker check.")]
		[Range(1, 10)]
		private int _flickerDelay = 2;

		[SerializeField]
		[Tooltip("The smallest amount the intensity can reach.")]
		[Range(1, 15)]
		private int _min = 7;

		[SerializeField]
		[Tooltip("The greatest amount the intensity can reach.")]
		[Range(5, 20)]
		private int _max = 15;

		private Light _light;
		private int count;

		void Awake() {
			_light = GetComponent<Light>();
		}

		void Update () {
			count++;
			if(count >= _flickerDelay) {
				//Randomly increases or decreases the torch's light by a 50/50 chance
				if(Random.value>0.5f) {
					_light.intensity += _flickerAmount;
					if(_light.intensity > _max) {
						_light.intensity = _max;
					}
				} else {
					_light.intensity -= _flickerAmount;
					if(_light.intensity < _min) {
						_light.intensity = _min;
					}
				}
				count = 0;
			}
		}


	}
