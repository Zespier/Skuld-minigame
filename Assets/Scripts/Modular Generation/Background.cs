using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : Module
{
    public FollowCamera followCamera;

    public override void ResetSpecificVariables() {
        base.ResetSpecificVariables();
        followCamera.ResetVariables();
    }
}
