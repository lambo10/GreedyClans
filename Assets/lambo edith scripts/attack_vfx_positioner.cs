using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_vfx_positioner : MonoBehaviour
{
    FighterController unitfighterController;
    Shooter unitShooter;
    tk2dSpriteAnimator spriteAnimator;
    public GameObject[] vfxEffect = new GameObject[8];
    Transform[] vfxEffectTransform = new Transform[8];
    ParticleSystem[] vfxEffectPsystem = new ParticleSystem[8];
    public float z = -10;


    void Start()
    {
        unitfighterController = GetComponent<FighterController>();
        unitShooter = GetComponent<Shooter>();
        spriteAnimator = GetComponent<tk2dSpriteAnimator>();
        vfxEffectTransform[0] = vfxEffect[0].GetComponent<Transform>();
        vfxEffectPsystem[0] = vfxEffect[0].GetComponent<ParticleSystem>();
        vfxEffectTransform[1] = vfxEffect[1].GetComponent<Transform>();
        vfxEffectPsystem[1] = vfxEffect[1].GetComponent<ParticleSystem>();
        vfxEffectTransform[2] = vfxEffect[2].GetComponent<Transform>();
        vfxEffectPsystem[2] = vfxEffect[2].GetComponent<ParticleSystem>();
        vfxEffectTransform[3] = vfxEffect[3].GetComponent<Transform>();
        vfxEffectPsystem[3] = vfxEffect[3].GetComponent<ParticleSystem>();
        vfxEffectTransform[4] = vfxEffect[4].GetComponent<Transform>();
        vfxEffectPsystem[4] = vfxEffect[4].GetComponent<ParticleSystem>();
        vfxEffectTransform[5] = vfxEffect[5].GetComponent<Transform>();
        vfxEffectPsystem[5] = vfxEffect[5].GetComponent<ParticleSystem>();
        vfxEffectTransform[6] = vfxEffect[6].GetComponent<Transform>();
        vfxEffectPsystem[6] = vfxEffect[6].GetComponent<ParticleSystem>();
        vfxEffectTransform[7] = vfxEffect[7].GetComponent<Transform>();
        vfxEffectPsystem[7] = vfxEffect[7].GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (unitShooter.shoot)
        {

            if (spriteAnimator.currentClip != null)
            {

                Vector3 moveDirection = unitfighterController.targetPoint;

                string clipName = spriteAnimator.currentClip.name;
                string[] splitArray = clipName.Split(char.Parse("_"));
                string direction = splitArray[1];

                if (direction.Equals("E"))
                {

                    vfxEffect[0].SetActive(true);
                    vfxEffectPsystem[0].Play(true);

                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);

                }
                else if (direction.Equals("N"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);

                    vfxEffectPsystem[1].Play(true);
                    vfxEffect[1].SetActive(true);

                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("NW"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);

                    vfxEffectPsystem[2].Play(true);
                    vfxEffect[2].SetActive(true);

                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("NW"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);

                    vfxEffectPsystem[3].Play(true);
                    vfxEffect[3].SetActive(true);

                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("S"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);

                    vfxEffectPsystem[4].Play(true);
                    vfxEffect[4].SetActive(true);

                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("SE"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);

                    vfxEffectPsystem[5].Play(true);
                    vfxEffect[5].SetActive(true);

                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);
                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("SW"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);

                    vfxEffectPsystem[6].Play(true);
                    vfxEffect[6].SetActive(true);

                    vfxEffectPsystem[7].Pause(true);
                    vfxEffect[7].SetActive(false);
                }
                else if (direction.Equals("W"))
                {
                    vfxEffectPsystem[0].Pause(true);
                    vfxEffect[0].SetActive(false);
                    vfxEffectPsystem[1].Pause(true);
                    vfxEffect[1].SetActive(false);
                    vfxEffectPsystem[2].Pause(true);
                    vfxEffect[2].SetActive(false);
                    vfxEffectPsystem[3].Pause(true);
                    vfxEffect[3].SetActive(false);
                    vfxEffectPsystem[4].Pause(true);
                    vfxEffect[4].SetActive(false);
                    vfxEffectPsystem[5].Pause(true);
                    vfxEffect[5].SetActive(false);
                    vfxEffectPsystem[6].Pause(true);
                    vfxEffect[6].SetActive(false);

                    vfxEffectPsystem[7].Play(true);
                    vfxEffect[7].SetActive(true);
                }

            }
            else
            {
                vfxEffectPsystem[0].Pause(true);
                vfxEffect[0].SetActive(false);
                vfxEffectPsystem[1].Pause(true);
                vfxEffect[1].SetActive(false);
                vfxEffectPsystem[2].Pause(true);
                vfxEffect[2].SetActive(false);
                vfxEffectPsystem[3].Pause(true);
                vfxEffect[3].SetActive(false);
                vfxEffectPsystem[4].Pause(true);
                vfxEffect[4].SetActive(false);
                vfxEffectPsystem[5].Pause(true);
                vfxEffect[5].SetActive(false);
                vfxEffectPsystem[6].Pause(true);
                vfxEffect[6].SetActive(false);
                vfxEffectPsystem[7].Pause(true);
                vfxEffect[7].SetActive(false);

            }
        }
        else
        {
            vfxEffectPsystem[0].Pause(true);
            vfxEffect[0].SetActive(false);
            vfxEffectPsystem[1].Pause(true);
            vfxEffect[1].SetActive(false);
            vfxEffectPsystem[2].Pause(true);
            vfxEffect[2].SetActive(false);
            vfxEffectPsystem[3].Pause(true);
            vfxEffect[3].SetActive(false);
            vfxEffectPsystem[4].Pause(true);
            vfxEffect[4].SetActive(false);
            vfxEffectPsystem[5].Pause(true);
            vfxEffect[5].SetActive(false);
            vfxEffectPsystem[6].Pause(true);
            vfxEffect[6].SetActive(false);
            vfxEffectPsystem[7].Pause(true);
            vfxEffect[7].SetActive(false);
        }
    }
}
