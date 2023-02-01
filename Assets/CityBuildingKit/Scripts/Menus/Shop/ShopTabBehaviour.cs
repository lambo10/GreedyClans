using UnityEngine;

public class ShopTabBehaviour : MonoBehaviour 
{
	[SerializeField]
	private UnityEngine.UI.Toggle toggle;

	public void OnValueChanged(bool isActive)
	{
		toggle.interactable = !isActive;
	}
}
