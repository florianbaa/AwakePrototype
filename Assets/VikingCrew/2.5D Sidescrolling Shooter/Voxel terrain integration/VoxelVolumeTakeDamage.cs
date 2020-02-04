using UnityEngine;
using System.Collections;

public abstract class VoxelVolumeTakeDamage : MonoBehaviour, IDamageable {

    public float invincibleZLayer = 3;
    public ParticleSystem dust;
    public int particlesPerCube = 20;
    protected VoxelGenerator generator;
    public abstract void ApplyDamage(float damage, Vector3 position, Vector3 incomingDirection);

    protected void EmitParticles(int number) {
        dust.Emit(number);
        for (int i = 0; i < dust.transform.childCount; i++) {
            dust.transform.GetChild(i).GetComponent<ParticleSystem>().Emit(number / 2);
        }
    }
}
