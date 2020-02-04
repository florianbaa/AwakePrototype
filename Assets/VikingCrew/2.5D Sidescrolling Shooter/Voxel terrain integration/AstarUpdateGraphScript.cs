using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarUpdateGraphScript : MonoBehaviour {
    public bool isVerbose = false;
	// Use this for initialization
	void Start () {
	
	}
	
    public void UpdateGraph(float xMin, float xMax, float yMin, float yMax) {
        
        Bounds bounds = new Bounds();
        bounds.min = (new Vector3(xMin, yMin, 0));
        bounds.max = (new Vector3(xMax, yMax, 0));
        
        AstarPath.active.UpdateGraphs(bounds);
        if (isVerbose) {
            Debug.Log("Updated graph at " + bounds);
            VikingCrewTools.DebugDrawPhysics.DebugDrawBounds(bounds, Color.yellow, 3);
        }
    }
}
