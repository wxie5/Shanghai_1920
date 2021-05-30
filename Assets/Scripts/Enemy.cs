using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour {
    public UnityEvent onDamage;
    public BossAI boss1;
    public Boss2 boss2;

    public void TakeDamage() {
        onDamage.Invoke();
    }

    public void Activate() {
        if(boss1) boss1.enable();
        if(boss2) boss2.enable();
    }
}
