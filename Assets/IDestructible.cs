using UnityEngine;
interface IDestructible {
    public int hitPoint {get; set;}
    public int maxHitPoint {get; set;}

    public void takeDamage(int damage);
    public bool isDestroyed();
}