using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cubiquity;
using UnityEngine.Events;
public class VoxelTerrainTakeDamage : VoxelVolumeTakeDamage {
   

    public TerrainVolume volume;
    
   
    // Use this for initialization
    void Start() {
        generator = volume.GetComponent<SidescrollingVoxelWorldGenerator>();

    }

    // Update is called once per frame
    void Update() {

    }
    
    

    public override void ApplyDamage(float damage, Vector3 position, Vector3 incomingDirection) {
        // Perform the raycasting. If there's a hit the position will be stored in these ints.
        //PickVoxelResult pickResult;
        PickSurfaceResult pickResult;
        bool hit = Picking.PickSurface(volume, new Ray(position - incomingDirection/2, incomingDirection), 1.0f, out pickResult);
        //bool hit = Picking.PickFirstSolidVoxel(volume, new Ray(position, -incomingDirection), 10.0f, out pickResult);
        bool doDestroy = UnityEngine.Random.Range(0f, damage) > 0.5f;
        // If we hit a solid voxel then create an explosion at this point.
        if (hit && doDestroy) {
            dust.transform.position = position;
            EmitParticles(particlesPerCube);
            float range = 0.6f;
            int x = Mathf.RoundToInt(pickResult.volumeSpacePos.x);
            int y = Mathf.RoundToInt(pickResult.volumeSpacePos.y);
            DestroyVoxels(x, y,range);
        }
    }

    



    void DestroyVoxels(float xPos, float yPos, float range) {
        // Set up a material which we will apply to the cubes which we spawn to replace destroyed voxels.
        
        float rangeSquared = range * range;
        int rangeCeil = Mathf.CeilToInt(range);
        // Later on we will be deleting some voxels, but we'll also be looking at the neighbours of a voxel.
        // This interaction can have some unexpected results, so it is best to first make a list of voxels we
        // want to delete and then delete them later in a separate pass.
        List<Vector3i> voxelsToDelete = new List<Vector3i>();

        // Iterage over every voxel in a cubic region defined by the received position (the center) and
        // the range. It is quite possible that this will be hundreds or even thousands of voxels.
        //for (int z = zPos - range; z < zPos + range; z++) {
        for (int z = 0; z < invincibleZLayer; z++) {
            for (int y = Mathf.FloorToInt( yPos - rangeCeil); y < Mathf.CeilToInt( yPos + rangeCeil); y++) {
                for (int x =Mathf.FloorToInt( xPos - rangeCeil); x < Mathf.CeilToInt(xPos + rangeCeil); x++) {
                    // Compute the distance from the current voxel to the center of our explosion.
                    float xDistance = x - xPos;
                    float yDistance = y - yPos;
                    //int zDistance = z - zPos;
                    //int zDistance = 0;

                    // Working with squared distances avoids costly square root operations.
                    //float distSquared = xDistance * xDistance + yDistance * yDistance;//euclidean norm
                    float distSquared = Mathf.Max(xDistance, yDistance);//infinity norm
                    // We're iterating over a cubic region, but we want our explosion to be spherical. Therefore 
                    // we only further consider voxels which are within the required range of our explosion center. 
                    // The corners of the cubic region we are iterating over will fail the following test.
                    if (distSquared <= rangeSquared) {
                        // Get the current color of the voxel
                        MaterialSet material = volume.data.GetVoxel(x, y, z);
                        bool isVisible = material.weights[0] > 10 ||
                            material.weights[1] > 10 ||
                            material.weights[2] > 10;
                        // Check the alpha to determine whether the voxel is visible. 
                        if (isVisible) {
                            Vector3i voxel = new Vector3i(x, y, z);
                            voxelsToDelete.Add(voxel);
                           
                        }
                    }
                }
            }

            foreach (Vector3i voxel in voxelsToDelete) // Loop through List with foreach
            {
                generator.DeleteVoxel(voxel);
            }
        }
    }
}