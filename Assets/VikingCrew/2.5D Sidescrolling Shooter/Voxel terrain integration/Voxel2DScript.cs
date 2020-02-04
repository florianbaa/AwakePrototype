using UnityEngine;
using System.Collections;
using Cubiquity;
public class Voxel2DScript : MonoBehaviour {
    public TerrainVolumeCollider terrainCollider;
    public TerrainVolume terrain;
    
	// Use this for initialization
	void Start () {
        terrain.OnMeshSyncComplete += HandleUpdate;
       
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void HandleUpdate() {
        //terrainCollider.
        //terrain.node
    }
}
