using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using Cubiquity;

[ExecuteInEditMode]
public class SidescrollingVoxelWorldGenerator : VoxelGenerator {
    /// <summary>
    /// An event used to tell a listener that the terrain has been edited in a certain area.
    /// Parameters are xMin, xMax, yMin, yMax
    /// </summary>
    [System.Serializable] public class TerrainUpdateEvent : UnityEvent<float, float, float, float> { }
    [System.Serializable]
    public class SimplexNoiseGenerator {
        public enum NoiseType{
            SPACE_1D,
            SPACE_2D,
            SPACE_3D,
        }
        public NoiseType noiseType = NoiseType.SPACE_2D;
        public AnimationCurve densityOfDepth = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0, 1));
        [Range(-2, 2), Header("Shifts the mean value")]
        public float meanValue = 0.5f;
        public float shiftX = 0;
        public float shiftY = 0;
        public float shiftZ = 0;
        
        public float noiseScale = 1f;

        [Header("Distribution is normally[-noisePower..noisePower]")]
        public float noisePower = 5f;

        private float xPos;
        private float yPos;
        private float zPos;
        private const float range = 10000f;
        public float Evaluate(float x, float y, float z, float maxDepth) {
            float invNoiseScale = 1.0f / noiseScale;
            
            float sampleX = (x + shiftX + xPos) * invNoiseScale;
            float sampleY = noiseType >= NoiseType.SPACE_2D ?  (y+ shiftY + yPos) * invNoiseScale : 0;
            float sampleZ = noiseType == NoiseType.SPACE_3D ? (z + shiftZ + zPos) * invNoiseScale : 0;

            // Get the noise value for the current position.
            // Returned value should be in the range -1 to +1.
            //float simplexNoiseValue = SimplexNoise.Noise.Generate(sampleX, sampleY, sampleZ);
            float simplexNoiseValue = SimplexNoise.Noise.Generate(sampleX, sampleY, sampleZ);
            simplexNoiseValue *= noisePower;
            simplexNoiseValue += meanValue;
            simplexNoiseValue *= densityOfDepth.Evaluate(y/maxDepth);
            return simplexNoiseValue;
        }

        public void Setup(int randomSeed) {
            int oldSeed = Random.seed;
            Random.seed = randomSeed;
            xPos = Random.Range(-range, range);
            yPos = Random.Range(-range, range);
            zPos = Random.Range(-range, range);
            Random.seed = oldSeed;
        }
    }

    public int randomSeed = 0;
    public bool randomizeSeed = true;
    
    public SimplexNoiseGenerator surfaceGenerator;
    public SimplexNoiseGenerator rockGenerator;
    public SimplexNoiseGenerator caveGenerator;
    public SimplexNoiseGenerator oreGenerator;

    public float grassThickness = 5f;
    public bool doForceBackgroundLayer = false;
    public int forcedBackgroundLayer = 1;
    
    public Vector2 tex1Scale = new Vector2(0.6f, 0.6f);
    public Vector2 tex2Scale = new Vector2(0.3f, 0.3f);
    public Vector2 tex3Scale = new Vector2(0.125f, 0.125f);
    public Vector2 tex4Scale = new Vector2(0.125f, 0.125f);
    TerrainVolume volume;
    /// <summary>
    /// We keep this as a cache of what has been deleted. When the mesh has been updated we can use these to say which area
    /// should be updated in e.g. pathfinding graphs
    /// </summary>
    [HideInInspector]
    public HashSet<Vector3i> deletedVoxels = new HashSet<Vector3i>();
    /// <summary>
    /// Fires when terrain has been updated. 
    /// Parmeters are: xMin,xMax, yMin, yMax
    /// </summary>
    public TerrainUpdateEvent OnTerrainUpdated;
    /// <summary>
    /// This generator is not done until all these sub-generators are done
    /// This is mainly used forr buildings that are to rreside in the terrain
    /// </summary>
    public VoxelGenerator[] dependentGenerators;
    

    // Use this for initialization
    void Start() {
        hasLoaded = false;
        if (volume == null)
            volume = GetComponent<TerrainVolume>();
        volume.OnMeshSyncComplete += HandleTerrainUpdated;
        Generate();


    }

    public void TerrainUpdatedCallback() {
        if (deletedVoxels.Count > 0) {
            float xMin = float.PositiveInfinity,
                    xMax = float.NegativeInfinity,
                    yMin = float.PositiveInfinity,
                    yMax = float.NegativeInfinity;
            foreach (var block in deletedVoxels) {
                xMin = Mathf.Min(block.x, xMin);
                xMax = Mathf.Max(block.x, xMax);
                yMin = Mathf.Min(block.y, yMin);
                yMax = Mathf.Max(block.y, yMax);
            }
            deletedVoxels.Clear();
            OnTerrainUpdated.Invoke(xMin, xMax, yMin, yMax);
        }
    }

    [ContextMenu("Generate terrain")]
    void GenerateTerrain() {
        Generate();
    }

    public void DependentVolumeFinishedChange() {
        HandleTerrainUpdated();
    }

    void HandleTerrainUpdated() {
        if (hasLoaded && isReadyToStart) {
            TerrainUpdatedCallback(); 
            
        } else if(!hasLoaded) {
           //First run so make sure all subsystems are ok
            StartCoroutine(WaitForAllDependentTerrainsToFinish());
           
            for (int i = 0; i < transform.childCount; i++) {
                CreateTakeDamageScriptForWorldChildren(transform.GetChild(i), GetComponent<VoxelTerrainTakeDamage>());
            }
            hasLoaded = true;
        }
    }

    IEnumerator WaitForAllDependentTerrainsToFinish() {
        while(!isAllSubSystemsDone()) {
            yield return null;
        }
        isReadyToStart = true;
        OnFinishedLoading.Invoke();
    }

    bool isAllSubSystemsDone() {
        foreach (var item in dependentGenerators) {
            if (!item.isReadyToStart) return false;
        }
        return true;
    }
    
    protected override void CreateTakeDamageScriptForWorldChildren(Transform trans, VoxelVolumeTakeDamage takeDamage) {
        VoxelTerrainTakeDamage newTakeDamage = trans.gameObject.AddComponent<VoxelTerrainTakeDamage>();
        newTakeDamage.dust = takeDamage.dust;
        newTakeDamage.particlesPerCube = takeDamage.particlesPerCube;
        newTakeDamage.volume = ((VoxelTerrainTakeDamage)takeDamage).volume;

        for (int i = 0; i < trans.childCount; i++) {
            CreateTakeDamageScriptForWorldChildren(trans.GetChild(i), newTakeDamage);
        }
    }



    public override void Generate() {
        //Setup all maps with random seeds
        if (randomizeSeed) {
            randomSeed = System.Environment.TickCount;
            
        }
        int oldSeed = Random.seed;
        Random.seed = randomSeed;
        surfaceGenerator.Setup(Random.Range( int.MinValue, int.MaxValue));
        rockGenerator.Setup(Random.Range(int.MinValue, int.MaxValue));
        caveGenerator.Setup(Random.Range(int.MinValue, int.MaxValue));
        oreGenerator.Setup(Random.Range(int.MinValue, int.MaxValue));
        Random.seed = oldSeed;


        // FIXME - Where should we delete this?
        /// [DoxygenSnippet-CreateEmptyTerrainVolumeData]
        //Create an empty TerrainVolumeData with dimensions width * height * depth
        TerrainVolumeData data = VolumeData.CreateEmptyVolumeData<TerrainVolumeData>(new Region(0, 0, 0, width - 1, height - 1, depth - 1));
        /// [DoxygenSnippet-CreateEmptyTerrainVolumeData]

        if (volume == null)
            volume = GetComponent<TerrainVolume>();
        TerrainVolumeRenderer volumeRenderer = GetComponent<TerrainVolumeRenderer>();

        volume.data = data;

        // This example looks better if we adjust the scaling factors on the textures.
        volumeRenderer.material.SetTextureScale("_Tex0", tex1Scale);
        volumeRenderer.material.SetTextureScale("_Tex1", tex2Scale);
        volumeRenderer.material.SetTextureScale("_Tex2", tex3Scale);
        volumeRenderer.material.SetTextureScale("_Tex3", tex4Scale);
        // At this point our volume is set up and ready to use. The remaining code is responsible
        // for iterating over all the voxels and filling them according to our noise functions.

        // Let's keep the allocation outside of the loop.
        MaterialSet materialSet = new MaterialSet();

        // Iterate over every voxel of our volume
        for (int x = 0; x < width; x++) {
            //First get a value saying how high the dirt should be in the range of [0..height]
            float dirtHeight = height * surfaceGenerator.Evaluate(x, 0, 0, height);
            //Next, we want a layer of grass on the top of the dirt layer
            float grassHeight = grassThickness + height * surfaceGenerator.Evaluate(x, 0, 0, height);
            for (int z = 0; z < depth; z++) {
            
                for (int y = height - 1; y > 0; y--) {
                    // Make sure we don't have anything left in here from the previous voxel
                    materialSet.weights[0] = 0;
                    materialSet.weights[1] = 0;
                    materialSet.weights[2] = 0;
                    materialSet.weights[3] = 0;

                    //Next, compare this value to the actual height also in the range of [0..1]
                    //If it is lower than the height should be then set the dirt value to being 1
                    float dirtValue = dirtHeight >= y ? 255 : 0;
                    
                    float grassValue = grassHeight >= y ? 255 : 0;

                    float rockValue = 255 * rockGenerator.Evaluate(x, y, z, height);
                    float caveValue = 255 * caveGenerator.Evaluate(x, y, z, height);
                    float oreValue = 255 * oreGenerator.Evaluate(x, y, z, height);
                    grassValue = Mathf.Max(grassValue - dirtValue, 0);
                    oreValue = Mathf.Clamp(oreValue, 0, 255);
                    caveValue = Mathf.Clamp(caveValue, 0, 255);

                    if (z == depth - 1) {
                        caveValue = 0;
                    }
                    
                    
                    dirtValue = Mathf.Max(dirtValue - caveValue, 0);
                    rockValue = Mathf.Max(rockValue - caveValue, 0);
                    
                    if (grassValue == 0 && dirtValue == 0) {
                        rockValue = 0;
                        oreValue = 0;
                    }
                    grassValue = Mathf.Max(grassValue - rockValue - caveValue, 0);



                    dirtValue = dirtValue - rockValue - oreValue;
                    rockValue = rockValue - oreValue;

                    grassValue = Mathf.Clamp(grassValue - rockValue, 0, 255);
                    dirtValue = Mathf.Clamp(dirtValue, 0, 255);
                    rockValue = Mathf.Clamp(rockValue, 0, 255);
                    oreValue = Mathf.Clamp(oreValue, 0, 255);
                    float sum = dirtValue + grassValue + rockValue + oreValue;

                   
                    if(sum > 255) {

                        oreValue = 255f * oreValue / sum;
                        rockValue = 255f * rockValue / sum;
                        dirtValue = 255f * dirtValue / sum;
                        grassValue = 255f * grassValue / sum;
                        sum = 255;
                    }
                    //Force the background layer if needed
                    if (forcedBackgroundLayer > 0 && z == depth - 1) {
                        materialSet.weights[(uint)forcedBackgroundLayer] = (byte)(sum);
                    } else {
                        // Write the final value value into the material channels
                        materialSet.weights[3] = (byte)(oreValue);
                        materialSet.weights[2] = (byte)(rockValue);
                        materialSet.weights[1] = (byte)(dirtValue);
                        materialSet.weights[0] = (byte)(grassValue);
                    }
                   


                    // We can now write our computed voxel value into the volume.
                    data.SetVoxel(x, y, z, materialSet);
                }
            }
        }
        
        //Once we finished generating the world, tell any dependent voxel volumes they can start generating
        foreach (var generator in dependentGenerators) {
            generator.Generate();
        }
    }

    public override void DeleteVoxel(Vector3i voxel) {
        MaterialSet empty = new MaterialSet();
        volume.data.SetVoxel(voxel.x, voxel.y, voxel.z, empty);
        deletedVoxels.Add(voxel);
    }
}
