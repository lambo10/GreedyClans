using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnim : MonoBehaviour
{
    tk2dSpriteAnimator animator;

    public tk2dSpriteAnimation idleAnimation;
    public tk2dSpriteAnimation walkAnimation;
    public tk2dSpriteAnimation attackAnimation;
    public tk2dSpriteAnimation buildAnimation;

    public tk2dSpriteAnimation spriteAnimation;

    private string action, direction;

    void Start()
    {
        animator = GetComponent<tk2dSpriteAnimator>();

    }

        void Update()
    {

        //Manual controller to check animations
        
        bool animChanged = false;
        if(Input.anyKey){ animChanged = true; }

        if(Input.GetKey(KeyCode.F)){ direction = "E"; }
        else if(Input.GetKey(KeyCode.R)){ direction = "NE"; }
        else if(Input.GetKey(KeyCode.E)){ direction = "N"; }
        else if(Input.GetKey(KeyCode.W)){ direction = "NW"; }
        else if(Input.GetKey(KeyCode.S)){ direction = "W"; }
        else if(Input.GetKey(KeyCode.Z)){ direction = "SW"; }
        else if(Input.GetKey(KeyCode.X)){ direction = "S"; }
        else if(Input.GetKey(KeyCode.C)){ direction = "SE"; }   

        if(Input.GetKey(KeyCode.I)){action = "Idle"; animator.Library = idleAnimation;    }
        else if(Input.GetKey(KeyCode.O)){action = "Walk"; animator.Library = walkAnimation;   }
        else if(Input.GetKey(KeyCode.K)){action = "Attack"; animator.Library = attackAnimation;   }
        else if(Input.GetKey(KeyCode.L)){action = "Build"; animator.Library = buildAnimation;   }

        if(animChanged){UpdateCharacterAnimation();}
        
    }

    public void UpdateCharacterAnimation()
    {
        animator.Play(action + "_" + direction);
    }
}
