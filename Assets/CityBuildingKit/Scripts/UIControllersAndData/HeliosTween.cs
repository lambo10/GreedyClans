using Assets.Scripts.Menus;
using JetBrains.Annotations;
using UnityEngine;

namespace UIControllersAndData
{
    public class HeliosTween : MonoBehaviour
    {
        [SerializeField] private AnimationClip _fadeInClip;    
        [SerializeField] private AnimationClip _fadeOutClip;
        [SerializeField] private Animation _animation;
        [SerializeField] private MessageController _messageController;

        public static HeliosTween Instance;

        private void Awake()
        {
            Instance = this;
        }

        [UsedImplicitly]
        public void PlayFadeIn()
        {
            _animation.Play(_fadeInClip.name);
        }
    
        [UsedImplicitly]
        public void PlayFadeOut()
        {
            _animation.Play(_fadeOutClip.name);
        }

        [UsedImplicitly]
        public void Event()
        {
            _messageController.EndGarble();
        }
    }
}
