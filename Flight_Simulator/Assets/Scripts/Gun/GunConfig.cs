using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Config", menuName = "Gun Config")]
public class GunConfig : ScriptableObject
{
    public uint GunRate, GunMaximumAmmo;
    public float RoundSpeed, RoundDispersion;
    public GameObject Round;
}
