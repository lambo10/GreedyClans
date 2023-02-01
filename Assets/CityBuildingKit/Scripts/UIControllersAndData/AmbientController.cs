using UIControllersAndData.Store.Categories.Ambient;
using UnityEngine;

namespace UIControllersAndData
{
	public class AmbientController : MonoBehaviour
	{
		public static AmbientController Instance;

		private void Awake()
		{
			Instance = this;
		}

		public void BuyStoreItem<T>(T data)
		{
			var itemData = data as AmbientCategory;
			if (itemData == null)
			{
				return;
			}

			
		}
	}
}
