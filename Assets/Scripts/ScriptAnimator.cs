using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAnimator {

    Animator anim;
    private string currentState;

    public ScriptAnimator(Animator animator) {
        anim = animator;
    }

    public void SetState(string animationState) {
        if (currentState == animationState) return;

        anim.Play(animationState, 0);

        currentState = animationState;
    }
}
