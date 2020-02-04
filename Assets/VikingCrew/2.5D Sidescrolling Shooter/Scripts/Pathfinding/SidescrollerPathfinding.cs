﻿using UnityEngine;
using System.Collections;
using Pathfinding;

namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// The responsibility of this class is to handle all pathfinding for a character. This class will be used by
    /// AIControl which will be making the actual decisions of where to go and what to do
    /// </summary>
    [System.Serializable]
    public class SidescrollerPathfinding  {

        //TODO: Encapsulate these so that only those that need to are public
        private int currentWaypoint = 0;
        private Path path = null;
        private Seeker pathSeeker;

        #region Settings
        [Header("Settings"), SerializeField, Tooltip("If the characters devoid more than this then path will be replanned")]
        private float maxDevoidFromPath = 5;
        [Tooltip("This is the distance in meters the AI will try to get within of its next waypoint position to consider having reached it and proceeding to the next one")]
        public float nextWaypointDistance = 1;
        #endregion

        #region Astar node settings
        public int walkableNodes = 1;
        public int jumpableNodes = 2;
        public int flyableNodes = 3;
        public int diggableNodes = 31;
        #endregion

        #region Movement variables
        [Header("Movement variables")]
        public Vector2 lastPos;
        [SerializeField]
        public float movedDistanceTowardWaypoint = 0;
        public bool hasRequestedPath = false;
        public Vector3 targetPosWhenRequesting;
        #endregion

        private Rigidbody2D body;
        private Rigidbody body3D;
        private Transform transform;
        private AIControls ai;
        #region Properties
        protected bool is2D { get { return body != null; } }
        protected Vector2 position { get { return transform.position; } }
        protected Vector2 velocity { get { if (is2D) return body.velocity; else return body3D.velocity; } }
        #endregion
        
        public void Setup(AIControls controls) {
            this.ai = controls;
            this.pathSeeker = controls.GetComponent<Seeker>();
            this.transform = controls.transform;
            this.body = controls.GetComponent<Rigidbody2D>();
            this.body3D = controls.GetComponent<Rigidbody>();
        }

        public bool HasPath() {
            return path != null;
        }

        /// <summary>
        /// Call this whenever the path we are on is leading to the wrong place or is in any other way
        /// considered not valid anymore. A new path will be requested at a later time
        /// </summary>
        public void SetPathUnvalid() {
            path = null;
        }

        /// <summary>
        /// Determines if we are on the correct path towards our next waypoint by checking 
        /// if we are getting closer to it. If we are consistently failing to do so it will request a new path
        /// as we may have fallen into a hole or something
        /// 
        /// </summary>
        public bool IsOnPath() {
            if (path == null) return false;
            float oldDist = (GetCurrentWaypoint() - lastPos).magnitude;
            float newDist = (GetCurrentWaypoint() - position).magnitude;
            float improvement = oldDist - newDist;
            movedDistanceTowardWaypoint += improvement;
            movedDistanceTowardWaypoint -= ai.minGetCloserPerSecond / ai.thinkPerSecond;
            if (!hasRequestedPath && movedDistanceTowardWaypoint < -maxDevoidFromPath) {
                if (ai.verboseDebug)
                    Debug.Log(ai.name + ": Progress was too slow so I requested a new path");
                return false;
            }
            lastPos = position;
            return true;
        }

        public bool IsWaypointWalkable() {
            if (path == null) return false;
            return path.path[currentWaypoint].Tag == walkableNodes;
        }

        public bool IsWaypointJumpable() {
            if (path == null) return false;
            return path.path[currentWaypoint].Tag == jumpableNodes;
        }
        public bool IsWaypointFlyable() {
            if (path == null) return false;
            return path.path[currentWaypoint].Tag == flyableNodes;
        }
        public bool IsWaypointDiggable() {
            if (path == null) return false;
            try {
                return path.path[currentWaypoint].Tag == diggableNodes;
            } catch (System.Exception e) {
                Debug.LogWarning(ai.GetStateDescription() + "\n" + e);
                return false;
            }
           
        }
        /// <summary>
        /// Requests a path to the closest node to the target position
        /// </summary>
        /// <param name="enemy"></param>
        public void RequestPath(Transform target) {
            targetPosWhenRequesting = target.position;
            RequestPath(targetPosWhenRequesting);
        }

        /// <summary>
        /// Requests a path to the closest node to the enemy position
        /// </summary>
        /// <param name="enemy"></param>
        public void RequestPath(Rigidbody2D enemy) {
            targetPosWhenRequesting = enemy.position;
            RequestPath(targetPosWhenRequesting);
        }

        /// <summary>
        /// Requests a path to the node closest to the target position
        /// </summary>
        /// <param name="position"></param>
        public void RequestPath(Vector3 position) {
            hasRequestedPath = true;
            //NNInfo nearest = AstarPath.active.GetNearest(position);
            //Vector3 closest = nearest.clampedPosition;
            pathSeeker.StartPath(transform.position, position, PathCalculatedCallback);
        }

        public void RequestRandomPath(Rect insideArea) {
            Vector2 pos = new Vector2(Random.Range(insideArea.xMin, insideArea.xMax), Random.Range(insideArea.yMin, insideArea.yMax));
            targetPosWhenRequesting = AstarPath.active.GetNearest(pos).clampedPosition;
            RequestPath(targetPosWhenRequesting);
        }

        public Vector2 GetCurrentWaypoint() {
            return (Vector2)path.vectorPath[currentWaypoint];
        }

        /// <summary>
        /// Gets the position of the waypoint after this one.
        /// Note that you must make sure to check that you're not on the last waypoint before calling this
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNextWaypoint() {
            return (Vector2)path.vectorPath[currentWaypoint];
        }

        public bool IsOnLastWaypointInPath() {
            return currentWaypoint >= path.vectorPath.Count - 1;
        }

        /// <summary>
        /// Checks if the next waypoint is positioned on the ground and can thus be walked on
        /// </summary>
        /// <param name="terrainLayers"></param>
        /// <returns></returns>
        public bool IsWaypointOnGround(LayerMask terrainLayers) {
            bool isOnGround;

            if (is2D) {
                isOnGround = Physics2D.Raycast(path.vectorPath[currentWaypoint], Vector3.down, 1.5f, terrainLayers).collider != null;
            } else {
                isOnGround = Physics.Raycast(path.vectorPath[currentWaypoint], Vector3.down, 1.5f, terrainLayers);
            }

            if (ai.verboseDebug) {
                DebugDrawPhysics.DebugDrawCircle(path.vectorPath[currentWaypoint], 0.25f, isOnGround ? Color.green : Color.red, 1f / ai.thinkPerSecond);
            }

            return isOnGround;
        }

        void PathCalculatedCallback(Path p) {
            hasRequestedPath = false;
            movedDistanceTowardWaypoint = 0;
            if (!p.error) {
                if(ai.verboseDebug)
                    Debug.Log(ai.name + " found path");
                path = p;
                //Reset the waypoint counter
                currentWaypoint = 0;
            } else {
                //If we could not find path directly to enemy then we will settle for the closest
                if (ai.verboseDebug)
                    Debug.Log(ai.name + " could not find path " + p.ToString());
            }
        }

        /// <summary>
        /// If close enough to current waypoint then we select the next waypoint
        /// 
        /// Returns true if we now reached the end of the path
        /// </summary>
        public bool SelectNextWaypointIfCloseEnough() {
            if (GetDistanceToCurrentWaypoint() < nextWaypointDistance) {

                if (currentWaypoint == path.vectorPath.Count - 1) {
                    //The path has been used up as index has now been increased too higgh
                    SetPathUnvalid();
                    return true;
                }else {//Proceed to next waypoint
                    currentWaypoint++;
                    movedDistanceTowardWaypoint = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Will find next waypoint that is on the ground. If the last one is in the air then we should jump to it
        /// so in that case it will keep the last one.
        /// </summary>
        public void FindNextVisibleGroundedWaypoint(LayerMask terrainLayers) {
            while (currentWaypoint < path.vectorPath.Count - 1 && //Only step past if it is not the last waypoint
                IsWaypointJumpable() && //Only step past if waypoint is jumpable, because these are sometimes easy to miss, resulting in the character backtracking
                GetDistanceToCurrentWaypoint() < 5 && // Is within reasonable distance
                CanSeeNextWaypoint()) //AND if the waypoint after that is visible. 
            {
                currentWaypoint++;
                movedDistanceTowardWaypoint = 0;
            }
        }

        public bool CanSeeWaypoint(int index) {
            if (path == null) return false;
            Vector3 dist = path.vectorPath[index] - transform.position;
            return ai.CheckLineOfSight(path.vectorPath[index]);
        }

        public bool CanSeeWaypoint() {
            return CanSeeWaypoint(currentWaypoint);
        }

        public bool CanSeeNextWaypoint() {
            return CanSeeWaypoint(currentWaypoint + 1);
        }

        public float GetDistanceToCurrentWaypoint() {
            if (HasPath()) {
                return (GetCurrentWaypoint() - position).magnitude;
            } else {
                return float.PositiveInfinity;
            }
        }
    }
}
