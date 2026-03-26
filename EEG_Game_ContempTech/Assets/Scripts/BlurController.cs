/*using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurController : MonoBehaviour
{
    public Volume volume;

    DepthOfField dof;

    void Start()
    {
        volume.profile.TryGet(out dof);
        dof.active = false; // start off
    }

    public void EnableBlur()
    {
        dof.active = true;
    }

    public void DisableBlur()
    {
        dof.active = false;
    }
}*/