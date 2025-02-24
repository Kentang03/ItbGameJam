using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FootstepSound : MonoBehaviour
{
    public string footstepEvent = "event:/Step_SFX";
    public string jumpEvent = "event:/Jump";
    
    public void PlayFootstep()
    {
        EventInstance footstepInstance = RuntimeManager.CreateInstance(footstepEvent);
        footstepInstance.start();
        footstepInstance.release();
    }

        public void PlayJump()
    {
        EventInstance jumpInstance = RuntimeManager.CreateInstance(jumpEvent);
        jumpInstance.start();
        jumpInstance.release();
    }
}
