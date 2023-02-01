using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.Serialization;

public class AnimatorHelper : MonoBehaviour 
{
	[SerializeField][FormerlySerializedAs("animationController")] 
	private Animator _animationController;
	[SerializeField][FormerlySerializedAs("nameOfParameter")] 
	private string _nameOfParameter;

	public void SetBool(bool value)
	{
		if (_animationController != null)
		{
			_animationController.SetBool (_nameOfParameter, value);
		}
		else
		{
			Debug.LogWarning ("Animator shouldn't be NULL!");
		}
	}
}
