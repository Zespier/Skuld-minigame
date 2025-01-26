using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCameraModule : Module {

    public FollowCamera followCamera;

    protected override void Update() {
        followCamera.CustomUpdate();

        base.Update();
    }
}
