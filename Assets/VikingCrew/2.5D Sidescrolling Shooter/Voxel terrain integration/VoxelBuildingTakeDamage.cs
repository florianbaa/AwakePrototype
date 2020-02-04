using UnityEngine;
using System.Collections;
using Cubiquity;

public class VoxelBuildingTakeDamage : VoxelVolumeTakeDamage {

    public ColoredCubesVolume volume;
    public override void ApplyDamage(float damage, Vector3 position, Vector3 incomingDirection) {
        // Perform the raycasting. If there's a hit the position will be stored in these ints.
        PickVoxelResult pickResult;
        
        bool hit = Picking.PickFirstSolidVoxel(volume, new Ray(position - incomingDirection / 2, incomingDirection), 1.0f, out pickResult);
        bool doDestroy = UnityEngine.Random.Range(0f, damage) > 0.5f;
        // If we hit a solid voxel then create an explosion at this point.
        if (hit && doDestroy) {
            dust.transform.position = position;
            EmitParticles(particlesPerCube);
            
            generator.DeleteVoxel(pickResult.volumeSpacePos);
        }
    }

    // Use this for initialization
    void Start() {
        generator = volume.GetComponent<VoxelBuildingGenerator>();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
