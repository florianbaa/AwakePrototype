using UnityEngine;
using System.Collections;

using Cubiquity;

[ExecuteInEditMode]
public class VoxelBuildingGenerator : VoxelGenerator {
    [Tooltip("This map is used as a blueprint for the building to construct. If alpha is 0 nothing is built for a voxel. If alpha is 255 then walls are constructed. For any other values only the back wall is constructed. NOTE: Make sure texture is readable (set in inspector advanced settings for texture)")]
    public Texture2D buildingMap;
    public SidescrollingVoxelWorldGenerator worldGenerator;
    private ColoredCubesVolume volume;
    // Use this for initialization
    void Awake() {
        hasLoaded = false;
        if (volume == null)
            volume = GetComponent<ColoredCubesVolume>();
        volume.OnMeshSyncComplete += MeshSynchCompleteCallback;
        
    }
    [ContextMenu("Generate terrain")]
    void GenerateTerrain() {
        Generate();
    }
    public override void Generate() {

        width = buildingMap.width;
        height = buildingMap.height;
        // Start with some empty volume data and we'll write our maze into this.
        /// [DoxygenSnippet-CreateEmptyColoredCubesVolumeData]
        // Create an empty ColoredCubesVolumeData with dimensions width * height * depth
        ColoredCubesVolumeData data = VolumeData.CreateEmptyVolumeData<ColoredCubesVolumeData>(new Region(0, 0, 0, width - 1, height - 1, depth - 1));
        /// [DoxygenSnippet-CreateEmptyColoredCubesVolumeData]

        //Get the main volume component
        volume = gameObject.GetComponent<ColoredCubesVolume>();

        
        // Attach the empty data we created previously
        volume.data = data;

        // At this point we have a volume created and can now start writting our maze data into it.

        // It's best to create these outside of the loop.
        QuantizedColor red = new QuantizedColor(255, 0, 0, 255);
        QuantizedColor blue = new QuantizedColor(0, 0, 255, 255);
        QuantizedColor gray = new QuantizedColor(127, 127, 127, 255);
        QuantizedColor white = new QuantizedColor(255, 255, 255, 255);
        
        // Iterate over every pixel of our maze image.
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                
                // Iterate over every voxel in the current column.
                for (int y = height - 1; y >= 0; y--) {
                    Color32 color = buildingMap.GetPixel(x, y);

                    //If this is the back layer...
                    if (z == depth - 1 && color.a > 0) {//Build in back layer only
                        color = GetBackgroundColor(color);
                        QuantizedColor block = new QuantizedColor(color.r, color.g, color.b, 255);
                        data.SetVoxel(x, y, z, block);
                    } else if(color.a == 255){//Build in all layers
                        QuantizedColor block = new QuantizedColor(color.r, color.g, color.b, 255);
                        data.SetVoxel(x, y, z, block);
                    } else {// We shouldn't build anything

                    }
                    if(color.a > 0)
                        DeleteTerrainVoxel(new Vector3i(x, y, z));
                }
            }
        }
    }
    
    private Color32 GetBackgroundColor(Color32 color) {
        Color32 bgCol = new Color32();
        bgCol.r = (byte)(color.r * 200f / 255f);
        bgCol.g = (byte)(color.g * 200f / 255f);
        bgCol.b = (byte)(color.b * 200f / 255f);
        bgCol.a = color.a;
        return bgCol;
    }

    protected override void CreateTakeDamageScriptForWorldChildren(Transform trans, VoxelVolumeTakeDamage takeDamage) {
        VoxelBuildingTakeDamage newTakeDamage = trans.gameObject.AddComponent<VoxelBuildingTakeDamage>();
        newTakeDamage.dust = takeDamage.dust;
        newTakeDamage.particlesPerCube = takeDamage.particlesPerCube;
        newTakeDamage.volume = ((VoxelBuildingTakeDamage)takeDamage).volume;

        for (int i = 0; i < trans.childCount; i++) {
            CreateTakeDamageScriptForWorldChildren(trans.GetChild(i), newTakeDamage);
        }
    }

    void DeleteTerrainVoxel(Vector3i voxel) {
            
            worldGenerator.DeleteVoxel(voxel);
            
    }

    public override void DeleteVoxel(Vector3i voxel) {
        volume.data.SetVoxel(voxel.x, voxel.y, voxel.z, new QuantizedColor(0, 0, 0, 0));
        //Add this to keep pathfinding up to date
        worldGenerator.deletedVoxels.Add(voxel);
    }
    
    /// <summary>
    /// This is called every time MeshSynchComplete is fired which means that after a change everything has been completed and
    /// is ready to use
    /// </summary>
    void MeshSynchCompleteCallback() {
        //If this has loaded then we are in-game and just need to let the parent know it's time to check pathfinding and such
        if (hasLoaded) {
            worldGenerator.DependentVolumeFinishedChange();
       } else {//Else this is the first time so we need to setup things for gameplay
            isReadyToStart = true;
            //Make sure all subparts of the octree structure can take damage
           for (int i = 0; i < transform.childCount; i++) {
               CreateTakeDamageScriptForWorldChildren(transform.GetChild(i), GetComponent<VoxelBuildingTakeDamage>());
           }
           hasLoaded = true;
       }
    }
}
