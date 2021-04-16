using System.Collections;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    public Animator blindfoldAnim;
    public Animator lightningAnim;
    public Animator backgroundAnim;
    public Animator char0Anim;
    public Animator char1Anim;
    public Animator char2Anim;

    public void LightningShock()
    {
        lightningAnim.SetTrigger("Lightning");
    }

    public IEnumerator BlindfoldFadeOutFadeIn(float waitingTime)
    {
        DisplayMain.Waiting = 2.2f + waitingTime;
        blindfoldAnim.SetBool("FadeOut", true);
        yield return new WaitForSeconds(waitingTime);
        blindfoldAnim.SetBool("FadeOut", false);
    }

    public void BlindfoldFadeIn()
    {
        blindfoldAnim.SetBool("FadeOut", false);
        DisplayMain.Waiting = 1.1f;
    }

    public void BlindfoldFadeOut()
    {
        blindfoldAnim.SetBool("FadeOut", true);
        DisplayMain.Waiting = 1.1f;
    }

    public void BackgroundShake()
    {
        backgroundAnim.SetTrigger("Shake");
        DisplayMain.Waiting = 1.0f;
    }

    public void Come(int pos)
    {
        switch(pos)
        {
            case 0:
                char0Anim.SetBool("IsCome", true);
                break;
            case 1:
                char1Anim.SetBool("IsCome", true);
                break;
            case 2:
                char2Anim.SetBool("IsCome", true);
                break;
        }
        DisplayMain.Waiting = 1.1f;
    }

    public void Shake(int pos)
    {
        switch (pos)
        {
            case 0:
                char0Anim.SetTrigger("Shake");
                break;
            case 1:
                char1Anim.SetTrigger("Shake");
                break;
            case 2:
                char2Anim.SetTrigger("Shake");
                break;
        }
        DisplayMain.Waiting = 1.1f;
    }

    public void Leave(int pos)
    {
        switch (pos)
        {
            case 0:
                char0Anim.SetBool("IsCome", false);
                break;
            case 1:
                char1Anim.SetBool("IsCome", false);
                break;
            case 2:
                char2Anim.SetBool("IsCome", false);
                break;
        }
        DisplayMain.Waiting = 1.1f;
    }

    /*
    public bool IsFadeIn()
    {
        return blindfoldAnim.GetCurrentAnimatorStateInfo(0).IsName("I");
    }

    public bool IsFadeOut()
    {
        return blindfoldAnim.GetCurrentAnimatorStateInfo(0).IsName("O");
    }

    public bool IsBKShaking()
    {
        return backgroundAnim.GetCurrentAnimatorStateInfo(0).IsName("Shake");
    }
    */
}
