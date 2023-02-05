using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dwarf: ScriptableObject, IDestructible {
    [SerializeField]
    public int hitPoint {get; set;}
    [SerializeField]
    public int maxHitPoint {get; set;}
    [SerializeField]
    public int miningDamage;
    [SerializeField]
    public int miningRange;
    [SerializeField]
    public int movementSpeed;
    [SerializeField]
    private int startingHitPoint;

    public int x {get; set;}
    public int y {get; set;}

    public void initFrom(Dwarf d) {
        this.miningDamage = d.miningDamage;
        this.miningRange = d.miningRange;
        this.movementSpeed = d.movementSpeed;
        this.hitPoint = d.startingHitPoint;
        this.maxHitPoint = d.startingHitPoint;
    }

    public Dwarf() {
        this.hitPoint = startingHitPoint;
        this.maxHitPoint = startingHitPoint;
    }

    public void takeDamage(int damage) {
        this.hitPoint -= damage;
    }

    public bool isDestroyed() {
        Debug.Log($"HP: {this.hitPoint}/{this.maxHitPoint}");
        return this.hitPoint <= 0;
    }

    public override String ToString() {
        return $"miningDamage: {this.miningDamage}\nminingRange: {this.miningRange}\nmovementSpeed: {this.movementSpeed}\nhitPoint: {this.hitPoint}\nmaxHitPoint: {this.maxHitPoint}\n";
    }
}