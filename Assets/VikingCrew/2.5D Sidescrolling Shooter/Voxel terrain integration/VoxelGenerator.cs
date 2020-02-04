using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cubiquity;
using UnityEngine.Events;

public abstract class VoxelGenerator : MonoBehaviour {
    public int width = 32;
    public int height = 32;
    public int depth = 1;
    protected bool hasLoaded = false;
    /// <summary>
    /// This is false until all voxels have been set AND we receive the callback that colliders have been constructed.
    /// </summary>
    public bool isReadyToStart = false;
    public UnityEvent OnFinishedLoading;
    
    public abstract void DeleteVoxel(Vector3i voxel);
    public abstract void Generate();
    protected abstract void CreateTakeDamageScriptForWorldChildren(Transform trans, VoxelVolumeTakeDamage takeDamage);
}
