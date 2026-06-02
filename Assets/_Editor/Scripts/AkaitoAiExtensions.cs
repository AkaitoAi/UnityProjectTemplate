using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
//using System.Diagnostics;

namespace AkaitoAi.Extensions
{
    public static class AkaitoAiExtensions
    {
        #region Array
        public static T[] ArrayAdd<T>(T[] arr, T add)
        {
            T[] ret = new T[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = arr[i];
            }
            ret[arr.Length] = add;
            return ret;
        }

        public static void ShuffleArray<T>(T[] arr, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }
        public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = random.Next(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }

        public static T[] RemoveDuplicates<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        public static int IndexOf<T>(this T[] array, T element)
        {
            // Loop through the array to find the index of the element
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element))
                {
                    return i;
                }
            }

            // Return -1 if the element is not found
            return -1;
        }


        public static T GetNextInSequence<T>(this T[] array, ref int currentIndex)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array cannot be null or empty.");

            // Get the current element
            T currentElement = array[currentIndex];

            // Increment the index, and wrap around if it exceeds the length of the array
            currentIndex = (currentIndex + 1) % array.Length;

            return currentElement;
        }

        private static int switchIndex = 0;
        private static int index;
        private static bool switched;

        public static bool SequenceSwitch<T>(this T[] array)
        {
            for (index = 0; index < array.Length; index++)
            {
                if (index == switchIndex)
                {
                    switched = true;
                }
                else switched = false;
            }

            if (switchIndex < array.Length - 1) switchIndex++;
            else switchIndex = 0;

            return switched;
        }
        #endregion

        #region Vector
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }
        public static Vector2 SetY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector2 AddX(this Vector2 vector, float x)
        {
            return new Vector2(vector.x + x, vector.y);
        }

        public static Vector2 AddY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, vector.y + y);
        }

        public static Vector2Int WithX(this Vector2Int vector, int x)
        {
            return new Vector2Int(x, vector.y);
        }
        public static Vector2Int WithY(this Vector2Int vector, int y)
        {
            return new Vector2Int(vector.x, y);
        }
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }
        public static Vector3Int WithX(this Vector3Int vector, int x)
        {
            return new Vector3Int(x, vector.y, vector.z);
        }
        public static Vector3Int WithY(this Vector3Int vector, int y)
        {
            return new Vector3Int(vector.x, y, vector.z);
        }
        public static Vector3Int WithZ(this Vector3Int vector, int z)
        {
            return new Vector3Int(vector.x, vector.y, z);
        }
        public static Vector4 WithX(this Vector4 vector, float x)
        {
            return new Vector4(x, vector.y, vector.z, vector.w);
        }
        public static Vector4 WithY(this Vector4 vector, float y)
        {
            return new Vector4(vector.x, y, vector.z, vector.w);
        }
        public static Vector4 WithZ(this Vector4 vector, float z)
        {
            return new Vector4(vector.x, vector.y, z, vector.w);
        }
        public static Vector4 WithW(this Vector4 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }
        public static Vector2 GetClosestVector2From(this Vector2 vector, Vector2[] otherVectors)
        {
            if (otherVectors.Length == 0) throw new Exception("The list of other vectors is empty");
            var minDistance = Vector2.SqrMagnitude(vector - otherVectors[0]);
            var minVector = otherVectors[0];
            for (var i = otherVectors.Length - 1; i > 0; i--)
            {
                var newDistance = Vector2.SqrMagnitude(vector - otherVectors[i]);
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    minVector = otherVectors[i];
                }
            }
            return minVector;
        }

        public static Vector3 GetClosestVector3From(this Vector3 vector, Vector3[] otherVectors)
        {
            if (otherVectors.Length == 0) throw new Exception("The list of other vectors is empty");

            var minDistance = Vector3.SqrMagnitude(vector - otherVectors[0]);
            var minVector = otherVectors[0];
            for (var i = otherVectors.Length - 1; i > 0; i--)
            {
                var newDistance = Vector3.SqrMagnitude(vector - otherVectors[i]);
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    minVector = otherVectors[i];
                }
            }
            return minVector;
        }
        public static Vector3 GetClosestVector3From(this Vector3 position, IEnumerable<Vector3> otherPositions)
        {
            var closest = Vector3.zero;
            var shortestDistance = Mathf.Infinity;

            foreach (var otherPosition in otherPositions)
            {
                var distance = (position - otherPosition).sqrMagnitude;

                if (distance < shortestDistance)
                {
                    closest = otherPosition;
                    shortestDistance = distance;
                }
            }

            return closest;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDir()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDirXZ()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }


        public static Vector3 GetVectorFromAngle(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 GetVectorFromAngle(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 GetVectorFromAngleInt(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static float GetAngleFromVectorFloatXZ(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int GetAngleFromVector180(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
        {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }

        public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * vec;
        }

        public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }

        public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight)
        {
            return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
        }

        /// <summary>
        /// Calculates the signed angle between two vectors on a plane defined by a normal vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="planeNormal">The normal vector of the plane on which to calculate the angle.</param>
        /// <returns>The signed angle between the vectors in degrees.</returns>
        public static float GetAngle(Vector3 vector1, Vector3 vector2, Vector3 planeNormal)
        {
            var angle = Vector3.Angle(vector1, vector2);
            var sign = Mathf.Sign(Vector3.Dot(planeNormal, Vector3.Cross(vector1, vector2)));
            return angle * sign;
        }

        /// <summary>
        /// Calculates the dot product of a vector and a normalized direction.
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="direction">The direction vector to project onto.</param>
        /// <returns>The dot product of the vector and the direction.</returns>
        public static float GetDotProduct(Vector3 vector, Vector3 direction) =>
            Vector3.Dot(vector, direction.normalized);

        /// <summary>
        /// Removes the component of a vector that is in the direction of a given vector.
        /// </summary>
        /// <param name="vector">The vector from which to remove the component.</param>
        /// <param name="direction">The direction vector whose component should be removed.</param>
        /// <returns>The vector with the specified direction removed.</returns>
        public static Vector3 RemoveDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize();
            return vector - direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// Extracts and returns the component of a vector that is in the direction of a given vector.
        /// </summary>
        /// <param name="vector">The vector from which to extract the component.</param>
        /// <param name="direction">The direction vector to extract along.</param>
        /// <returns>The component of the vector in the direction of the given vector.</returns>
        public static Vector3 ExtractDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize();
            return direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// Rotates a vector onto a plane defined by a normal vector using a specified up direction.
        /// </summary>
        /// <param name="vector">The vector to be rotated onto the plane.</param>
        /// <param name="planeNormal">The normal vector of the target plane.</param>
        /// <param name="upDirection">The current 'up' direction used to determine the rotation.</param>
        /// <returns>The vector after being rotated onto the specified plane.</returns>
        public static Vector3 RotateVectorOntoPlane(Vector3 vector, Vector3 planeNormal, Vector3 upDirection)
        {
            // Calculate rotation;
            var rotation = Quaternion.FromToRotation(upDirection, planeNormal);

            // Apply rotation to vector;
            vector = rotation * vector;

            return vector;
        }

        /// <summary>
        /// Projects a given point onto a line defined by a starting position and direction vector.
        /// </summary>
        /// <param name="lineStartPosition">The starting position of the line.</param>
        /// <param name="lineDirection">The direction vector of the line, which should be normalized.</param>
        /// <param name="point">The point to project onto the line.</param>
        /// <returns>The projected point on the line closest to the original point.</returns>
        public static Vector3 ProjectPointOntoLine(Vector3 lineStartPosition, Vector3 lineDirection, Vector3 point)
        {
            var projectLine = point - lineStartPosition;
            var dotProduct = Vector3.Dot(projectLine, lineDirection);

            return lineStartPosition + lineDirection * dotProduct;
        }

        /// <summary>
        /// Increments a vector toward a target vector at a specified speed over a given time interval.
        /// </summary>
        /// <param name="currentVector">The current vector to be incremented.</param>
        /// <param name="speed">The speed at which to move towards the target vector.</param>
        /// <param name="deltaTime">The time interval over which to move.</param>
        /// <param name="targetVector">The target vector to approach.</param>
        /// <returns>The new vector incremented toward the target vector by the specified speed and time interval.</returns>
        public static Vector3 IncrementVectorTowardTargetVector(Vector3 currentVector, float speed, float deltaTime,
            Vector3 targetVector)
        {
            return Vector3.MoveTowards(currentVector, targetVector, speed * deltaTime);
        }

        #endregion

        #region GameObject
        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent<T>(out var attachedComponent))
            {
                attachedComponent = component.AddComponent<T>();
            }

            return attachedComponent;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent<T>(out var attachedComponent))
            {
                attachedComponent = gameObject.AddComponent<T>();
            }

            return attachedComponent;
        }

        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out _);
        }
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.TryGetComponent<T>(out _);
        }

        public static GameObject FindObjectByName(this Scene scene, string name)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name == name)
                {
                    return go;
                }
            }

            return null;
        }

        public static bool TryGetComponentHierarchy<T>(this Transform t, out T component) where T : Component
        {
            component =
                t.GetComponent<T>()
                ?? t.GetComponentInParent<T>()
                ?? t.GetComponentInChildren<T>();

            return component != null;
        }


        public static bool IsInCullingMask(this GameObject gameObject, LayerMask cullingMask)
        {
            return (cullingMask & (1 << gameObject.layer)) != 0;
        }

        public static IEnumerator ChangeStateAfterDelayRealtime(this GameObject go, bool state, float delay)
        {
            go.SetActive(state);
            yield return new WaitForSecondsRealtime(delay);
            go.SetActive(!state);
        }
        public static IEnumerator ChangeStateAfterDelay(this GameObject go, bool state, float delay)
        {
            go.SetActive(state);
            yield return new WaitForSeconds(delay);
            go.SetActive(!state);
        }

        public static GameObject GetChildGameObjectByName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
                return childTrans.gameObject;
            else
                return null;
        }

        #endregion

        #region List
        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                var value = list[j];
                list[j] = list[i];
                list[i] = value;
            }
        }

        public static List<T> RemoveDuplicates<T>(List<T> arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// Assigns items from a source collection to a target collection, skipping a specified index and avoiding duplicates.
        /// </summary>
        /// <typeparam name="TSource">Type of the source collection items.</typeparam>
        /// <typeparam name="TTarget">Type of the target collection items.</typeparam>
        /// <param name="targetCollection">The collection to populate (e.g., spawn points).</param>
        /// <param name="sourceCollection">The source collection to assign from (e.g., vehicles).</param>
        /// <param name="setupAction">Action to execute for each assignment (e.g., position setup).</param>
        /// <param name="skipIndex">Index in the source collection to skip.</param>
        public static void AssignWithSkip<TSource, TTarget>(
            this IList<TTarget> targetCollection,
            IList<TSource> sourceCollection,
            Action<TTarget, TSource> setupAction,
            int skipIndex)
        {
            if (targetCollection == null || sourceCollection == null || setupAction == null)
            {
                Debug.LogError("AssignWithSkip: One or more parameters are null.");
                return;
            }

            // Track assigned indices to avoid duplicates
            HashSet<int> assignedIndices = new HashSet<int>();

            // Add the skipped index initially to ensure it's not reused
            assignedIndices.Add(skipIndex);

            int sourceCount = sourceCollection.Count;
            int targetCount = targetCollection.Count;
            int sourceIndex = 0;

            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
            {
                // Skip already assigned indices
                while (assignedIndices.Contains(sourceIndex))
                {
                    sourceIndex = (sourceIndex + 1) % sourceCount;
                }

                // Perform the assignment
                setupAction(targetCollection[targetIndex], sourceCollection[sourceIndex]);
                assignedIndices.Add(sourceIndex);

                // Move to the next source item
                sourceIndex = (sourceIndex + 1) % sourceCount;
            }

            // Example usage
            //int currentVehicleIndex = GameManager.instance.gameData.currentCar;

            //stuntLevels[GameManager.instance.currentLevel].playerSpawn
            //    .AssignWithSkip(
            //        vehicles,
            //        (spawnPoint, vehicle) =>
            //        {
            //            RCC_CarControllerV3 carController = vehicle.car?.GetComponent<RCC_CarControllerV3>();
            //            if (carController != null)
            //            {
            //                SetupRCC(spawnPoint, carController);
            //            }
            //            else
            //            {
            //                Debug.LogWarning("Vehicle is missing RCC_CarControllerV3 or car reference!");
            //            }
            //        },
            //        currentVehicleIndex
            //    );
        }

        /// <summary>
        /// Checks if the given index is valid for the collection and not equal to the selection index.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to validate against.</param>
        /// <param name="index">The index to check.</param>
        /// <param name="selectionIndex">The current selection index to compare.</param>
        /// <returns>True if the index is valid and not the selection index; otherwise, false.</returns>
        public static bool IsValidIndex<T>(this ICollection<T> collection, int index, int selectionIndex)
        {
            return index != selectionIndex && index >= 0 && index < collection.Count;

            //    public List<GameObject> characters;
            //private int selectionIndex = 0;

            //public void SelectCharacter(int index)
            //{
            //    if (!characters.IsValidIndex(index, selectionIndex))
            //        return;

            //    // Proceed with selecting the character
            //    selectionIndex = index;
            //    Debug.Log($"Selected character: {characters[index].name}");
            //}
        }

        /// <summary>
        /// Checks if the given index is valid for the collection and not equal to the selection index.
        /// Automatically wraps the index within the valid range.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to validate against.</param>
        /// <param name="index">The index to check, which will be wrapped to stay within bounds.</param>
        /// <param name="selectionIndex">The current selection index to compare.</param>
        /// <returns>True if the index is valid and not the selection index; otherwise, false.</returns>
        public static bool IsValidIndexWithWrap<T>(this ICollection<T> collection, ref int index, int selectionIndex)
        {
            // Wrap the index to stay within the bounds of the collection
            index = (index + collection.Count) % collection.Count;

            // Check if the wrapped index is valid and not the same as the selection index
            return index != selectionIndex && index >= 0 && index < collection.Count;
        }

        #endregion

        #region Transform
        public static void DestroyChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void ResetTransformation(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static int ChildCount(this Transform transform)
        {
            return transform.childCount;
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static void RotateTowardsUp(this Transform transform, float speed)
        {
            Quaternion toRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
        }

        public static void RotateAroundAxis(this Transform transform, Vector3 axis, float angle)
        {
            transform.rotation = Quaternion.AngleAxis(angle, axis) * transform.rotation;
        }


        public static void AddChildren(this Transform transform, params GameObject[] children)
        {
            foreach (var child in children)
            {
                child.transform.parent = transform;
            }
        }
        public static void AddChildren(this Transform transform, params Component[] children)
        {
            foreach (var child in children)
            {
                child.transform.parent = transform;
            }
        }
        public static void Reset(this Transform transform, Space space = Space.Self)
        {
            switch (space)
            {
                case Space.Self:
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    break;

                case Space.World:
                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;
                    break;
            }

            transform.localScale = Vector3.one;
        }
        public static void ResetChildPositions(this Transform transform, bool recursive = false)
        {
            foreach (Transform child in transform)
            {
                child.position = Vector3.zero;

                if (recursive)
                {
                    child.ResetChildPositions(true);
                }
            }
        }
        public static void SetChildLayers(this Transform transform, string layerName, bool recursive = false)
        {
            var layer = LayerMask.NameToLayer(layerName);
            SetChildLayersHelper(transform, layer, recursive);
        }

        static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;

                if (recursive)
                {
                    SetChildLayersHelper(child, layer, true);
                }
            }
        }
        public static void SetLocalPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            var localPosition = transform.localPosition;

            if (x.HasValue)
            {
                localPosition.x = x.Value;
            }

            if (y.HasValue)
            {
                localPosition.y = y.Value;
            }

            if (z.HasValue)
            {
                localPosition.z = z.Value;
            }

            transform.localPosition = localPosition;
        }
        public static void SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            var position = transform.position;

            if (x.HasValue)
            {
                position.x = x.Value;
            }

            if (y.HasValue)
            {
                position.y = y.Value;
            }

            if (z.HasValue)
            {
                position.z = z.Value;
            }

            transform.position = position;
        }
        public static void SetX(this Transform transform, float x)
        {
            var position = transform.position;
            transform.position = new Vector3(x, position.y, position.z);
        }
        public static void SetY(this Transform transform, float y)
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, y, position.z);
        }
        public static void SetZ(this Transform transform, float z)
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, z);
        }

        // Destroy all children and randomize their names, useful if you want to do a Find() after calling destroy, since they only really get destroyed at the end of the frame
        public static void DestroyChildrenRandomizeNames(Transform parent)
        {
            foreach (Transform transform in parent)
            {
                transform.name = "" + UnityEngine.Random.Range(10000, 99999);
                GameObject.Destroy(transform.gameObject);
            }
        }

        // Destroy all children except the ones with these names
        public static void DestroyChildren(Transform parent, params string[] ignoreArr)
        {
            foreach (Transform transform in parent)
            {
                if (System.Array.IndexOf(ignoreArr, transform.name) == -1) // Don't ignore
                    GameObject.Destroy(transform.gameObject);
            }
        }

        public static Transform CloneTransform(Transform transform, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, transform.parent);

            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;

            return clone;
        }

        public static Transform CloneTransform(Transform transform, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
            {
                clone.gameObject.SetActive(true);
            }
            return clone;
        }

        public static Transform CloneTransform(Transform transform, Transform newParent, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, newParent);

            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;

            return clone;
        }

        public static Transform CloneTransform(Transform transform, Transform newParent, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
            {
                clone.gameObject.SetActive(true);
            }
            return clone;
        }

        public static Transform CloneTransformWorld(Transform transform, Transform newParent, string name, Vector3 newLocalPosition)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.localPosition = newLocalPosition;
            return clone;
        }

        public static bool GetDirectionWithRespectTo(Transform from, Transform to)
        {
            Vector3 fromVector = from.forward;
            Vector3 toVector = to.forward;

            float dot = Vector3.Dot(fromVector, toVector);

            return (dot > 0.5f && dot < 1f) || (dot > -0.5f && dot < -1f) ? true : false;
        }

        public static bool GetDirectionWithRespectTo(Transform from, Transform to, Vector2 angles)
        {
            Vector3 fromVector = from.forward;
            Vector3 toVector = to.forward;

            float dot = Vector3.Dot(fromVector, toVector);

            return (dot > angles.x && dot < angles.y) || (dot > -angles.x && dot < -angles.y) ? true : false;
        }

        public static bool GetDirectionWithRespectTo(Transform from, Transform to, Vector2 forwardAngles, Vector2 backwardAngles)
        {
            Vector3 fromVector = from.forward;
            Vector3 toVector = to.forward;

            float dot = Vector3.Dot(fromVector, toVector);

            return (dot > forwardAngles.x && dot < forwardAngles.y) || (dot > backwardAngles.x && dot < backwardAngles.y) ? true : false;
        }

        public static float WrapAngle(this float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        public static float UnwrapAngle(this float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }

        #endregion

        #region Rigidbody
        public static void ChangeDirection(this Rigidbody rb, Vector3 direction)
        {
            rb.velocity = direction.normalized * rb.velocity.magnitude;
        }

        public static void NormalizeVelocity(this Rigidbody rb, float magnitude = 1)
        {
            rb.velocity = rb.velocity.normalized * magnitude;
        }
        /// <summary>
        /// Changes the direction of the Rigidbody's velocity while maintaining its speed.
        /// </summary>
        /// <param name="rigidbody">The Rigidbody to change direction.</param>
        /// <param name="direction">The new direction for the Rigidbody.</param>
        /// <returns>The modified Rigidbody for method chaining.</returns>
        public static Rigidbody ChangeDirectionReturn(this Rigidbody rigidbody, Vector3 direction)
        {
            if (direction.sqrMagnitude == 0f) return rigidbody;
            direction.Normalize();

#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = direction * rigidbody.linearVelocity.magnitude;
#else
            rigidbody.velocity = direction * rigidbody.velocity.magnitude;
#endif
            return rigidbody;
        }

        /// <summary>
        /// Stops the Rigidbody by setting its linear and angular velocities to zero.
        /// </summary>
        /// <param name="rigidbody">The Rigidbody to stop.</param>
        /// <returns>The modified Rigidbody for method chaining.</returns>
        public static Rigidbody Stop(this Rigidbody rigidbody)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = Vector3.zero;
#else
            rigidbody.velocity = Vector3.zero;
#endif
            rigidbody.angularVelocity = Vector3.zero;
            return rigidbody;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Returns true if the distance between two positions is greater than the given range.
        /// Uses sqrMagnitude (no sqrt) for maximum performance.
        /// </summary>
        public static bool IsFartherThan(this Vector3 from, Vector3 to, float distance)
        {
            float distSqr = (from - to).sqrMagnitude;
            float rangeSqr = distance * distance;
            return distSqr > rangeSqr;
        }

        /// <summary>
        /// Returns true if the distance between two positions is less than the given range.
        /// Uses sqrMagnitude (no sqrt) for maximum performance.
        /// </summary>
        public static bool IsCloserThan(this Vector3 from, Vector3 to, float distance)
        {
            float distSqr = (from - to).sqrMagnitude;
            float rangeSqr = distance * distance;
            return distSqr < rangeSqr;
        }
        public static int GetIndexWithDirectionWrap(int direction, int currentIndex, int totalCount)
        {
            if (totalCount <= 0) return 0;

            return (currentIndex + direction + totalCount) % totalCount;
        }

        public static void MethodWithInternetReachability(Action method)
        {
            if (Application.internetReachability
                == NetworkReachability.NotReachable) return;

            method?.Invoke();
        }
        public static void MethodWithInternetReachability(Action withInternet, Action withoutInternet)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                withInternet?.Invoke();
            else withoutInternet?.Invoke();
        }
        public static void MethodWithInternetReachability(Action<Action, Action> ad,
           Action reward, Action noInternet, Action adNotAvailable)
        {
            if (Application.internetReachability
                == NetworkReachability.NotReachable) noInternet?.Invoke();
            else ad?.Invoke(reward, adNotAvailable);
        }


        public static float ConvertToPositiveFloat(float _value)
        {
            return _value < 0 ? _value * -1 : _value;
        }
        public static float ConvertToNegativeFloat(float _value)
        {
            return _value > 0 ? _value * -1 : _value;
        }

        public static int boolToInt(bool val)
        {
            return val ? 1 : 0;
        }

        public static bool intToBool(int val)
        {
            return val != 0 ? true : false;
        }

        public static void SetTransparency(this UnityEngine.UI.Image p_image, float p_transparency)
        {
            if (p_image != null)
            {
                UnityEngine.Color __alpha = p_image.color;
                __alpha.a = p_transparency;
                p_image.color = __alpha;
            }
        }

        // Return random element from array
        public static T GetRandom<T>(T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        // Return a number with milli dots, 1000000 -> 1.000.000
        public static string GetMilliDots(float n)
        {
            return GetMilliDots((long)n);
        }

        public static string GetMilliDots(long n)
        {
            string ret = n.ToString();
            for (int i = 1; i <= Mathf.Floor(ret.Length / 4); i++)
            {
                ret = ret.Substring(0, ret.Length - i * 3 - (i - 1)) + "." + ret.Substring(ret.Length - i * 3 - (i - 1));
            }
            return ret;
        }


        // Return with milli dots and dollar sign
        public static string GetDollars(float n)
        {
            return GetDollars((long)n);
        }
        public static string GetDollars(long n)
        {
            if (n < 0)
                return "-$" + GetMilliDots(Mathf.Abs(n));
            else
                return "$" + GetMilliDots(n);
        }

        // Split a string into an array based on a Separator
        public static string[] SplitString(string save, string separator)
        {
            return save.Split(new string[] { separator }, System.StringSplitOptions.None);
        }

        /// <summary>
        /// Toggles the boolean value.
        /// </summary>
        /// <param name="value">The boolean variable to toggle.</param>
        /// <returns>The toggled boolean value.</returns>
        public static bool Toggle(this ref bool value)
        {
            value = !value;
            return value;
        }

        public static void DebugDrawRectangle(Vector3 minXY, Vector3 maxXY, Color color, float duration)
        {
            Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(maxXY.x, minXY.y), color, duration);
            Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(minXY.x, maxXY.y), color, duration);
            Debug.DrawLine(new Vector3(minXY.x, maxXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
            Debug.DrawLine(new Vector3(maxXY.x, minXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
        }

        public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, Vector3[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(position + points[i] * size, position + points[i + 1] * size, color, duration);
            }
        }

        public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, float[] points)
        {
            List<Vector3> vecList = new List<Vector3>();
            for (int i = 0; i < points.Length; i += 2)
            {
                Vector3 vec = new Vector3(points[i + 0], points[i + 1]);
                vecList.Add(vec);
            }
            DebugDrawLines(position, color, size, duration, vecList.ToArray());
        }

        public static void ClearLogConsole()
        {
#if UNITY_EDITOR
            //Debug.Log("################# DISABLED BECAUSE OF BUILD!");
            /*
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            System.Type logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
            //*/
#endif
        }


        public static string GetPercentString(float f, bool includeSign = true)
        {
            return Mathf.RoundToInt(f * 100f) + (includeSign ? "%" : "");
        }

        public static class ReflectionTools
        {

            public static object CallMethod(string typeName, string methodName)
            {
                return System.Type.GetType(typeName).GetMethod(methodName).Invoke(null, null);
            }
            public static object GetField(string typeName, string fieldName)
            {
                System.Reflection.FieldInfo fieldInfo = System.Type.GetType(typeName).GetField(fieldName);
                return fieldInfo.GetValue(null);
            }
            public static System.Type GetNestedType(string typeName, string nestedTypeName)
            {
                return System.Type.GetType(typeName).GetNestedType(nestedTypeName);
            }

        }

        public class SwitchCase<T>
        {
            private T value;
            private bool hasMatched;
            private Action defaultAction;

            public SwitchCase(T value)
            {
                this.value = value;
                this.hasMatched = false;
            }

            public SwitchCase<T> Case(T caseValue, Action action)
            {
                if (!hasMatched && EqualityComparer<T>.Default.Equals(value, caseValue))
                {
                    action?.Invoke();
                    hasMatched = true;
                }
                return this;
            }

            public SwitchCase<T> Default(Action action)
            {
                defaultAction = action;
                return this;
            }

            public void Execute()
            {
                if (!hasMatched)
                {
                    defaultAction?.Invoke();
                }
            }
        }

        public static SwitchCase<T> Switch<T>(this T value)
        {
            return new SwitchCase<T>(value);
        }

        //    nt number = 2;

        //    // Basic usage
        //    number.Switch()
        //        .Case(1, () => Debug.Log("Number is 1"))
        //        .Case(2, () => Debug.Log("Number is 2"))
        //        .Case(3, () => Debug.Log("Number is 3"))
        //        .Default(() => Debug.Log("Number is something else"))
        //        .Execute();

        //    // Example with string
        //    string text = "hello";
        //    text.Switch()
        //        .Case("hello", () => Debug.Log("Said hello"))
        //        .Case("goodbye", () => Debug.Log("Said goodbye"))
        //        .Default(() => Debug.Log("Said something else"))
        //        .Execute();

        //    // Example with enum
        //    MyEnum enumValue = MyEnum.Second;
        //    enumValue.Switch()
        //        .Case(MyEnum.First, () => Debug.Log("First case"))
        //        .Case(MyEnum.Second, () => Debug.Log("Second case"))
        //        .Execute();
        //}

        //enum MyEnum
        //{
        //    First,
        //    Second,
        //    Third
        //}

        // Extension method for Action that logs the caller name and invokes the action
        public static void ExecuteWithCaller(this Action action, [CallerMemberName] string callerName = null)
        {
            Debug.Log("Action called from: " + callerName);
            action?.Invoke();
        }

        //StackTrace stackTrace = new StackTrace();
        //Console.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);

        public static bool TestChance(int chance, int chanceMax = 100)
        {
            return UnityEngine.Random.Range(0, chanceMax) < chance;
        }

        public static bool ReplaceMaterial(this Renderer renderer, Material material, out int targetMaterialIndex)
        {
            targetMaterialIndex = -1;
            if (renderer == null || material == null)
            {
                Debug.LogWarning("Renderer or Material is null.");
                return false;
            }

            Material[] materials = renderer.materials;
            Material newMaterial = new Material(material);
            bool materialFound = false;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains(material.name))
                {
                    materials[i] = newMaterial;
                    targetMaterialIndex = i;
                    materialFound = true;
                    break;
                }
            }

            if (materialFound)
            {
                renderer.materials = materials;
            }
            else
            {
                Debug.LogWarning($"Material with name containing '{material.name}' not found in {renderer.name}. Current materials: {string.Join(", ", materials.Select(m => m.name))}");
            }

            return materialFound;
        }

        public static void ReplaceMaterial(this Renderer renderer, Material material)
        {
            int targetMaterialIndex = -1;
            if (renderer == null || material == null)
            {
                Debug.LogWarning("Renderer or Material is null.");
            }

            Material[] materials = renderer.materials;
            Material newMaterial = new Material(material);
            bool materialFound = false;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains(material.name))
                {
                    materials[i] = newMaterial;
                    targetMaterialIndex = i;
                    materialFound = true;
                    break;
                }
            }

            if (materialFound)
            {
                renderer.materials = materials;
            }
            else
            {
                Debug.LogWarning($"Material with name containing '{material.name}' not found in {renderer.name}. Current materials: {string.Join(", ", materials.Select(m => m.name))}");
            }
        }

        public static void ApplyOffset(this Material material, float currentSpeed, float maxSpeed, int direction, float scrollSpeedMultiplier, bool isXOffset, ref float yOffset)
        {
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            float deltaOffset = direction * speedFactor * scrollSpeedMultiplier * Time.deltaTime;

            yOffset += deltaOffset;

            Vector2 currentOffset = material.mainTextureOffset;

            if (!isXOffset)
                material.mainTextureOffset = new Vector2(currentOffset.x, yOffset);
            else
                material.mainTextureOffset = new Vector2(yOffset, currentOffset.y);
        }

        public static void InEditor(Action inEditor)
        {
#if UNITY_EDITOR
            inEditor?.Invoke();
#endif
        }
        public static void InEditor(Action inEditor, Action inBuild)
        {
#if UNITY_EDITOR
            inEditor?.Invoke();
#else
            inBuild?.Invoke();
#endif
        }

        #endregion

        #region Maths
        /// <summary>
        /// Return the square of the given value.
        /// </summary>

        public static float Square(float value)
        {
            return value * value;
        }

        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>

        public static Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            Vector3 right = direction.perpendicularTo(up);

            return normal.perpendicularTo(right);
        }

        /// <summary>
        /// Projects a given point onto the plane defined by plane origin and plane normal.
        /// </summary>

        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            Vector3 toPoint = point - planeOrigin;
            Vector3 toPointProjected = Vector3.Project(toPoint, planeNormal);

            return point - toPointProjected;
        }

        /// <summary>
        /// Clamps the given angle into 0 - 360 degrees range.
        /// </summary>

        public static float Clamp0360(float eulerAngles)
        {
            float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
            if (result < 0) result += 360f;

            return result;
        }

        /// <summary>
        /// Returns a new rotation angle (interpolated) clamped in the range (0.0f , 360.0f)
        /// </summary>

        public static float FixedTurn(float current, float target, float maxDegreesDelta)
        {
            if (maxDegreesDelta == 0.0f)
                return Clamp0360(current);

            if (maxDegreesDelta >= 360.0f)
                return Clamp0360(target);

            float result = Clamp0360(current);
            current = result;
            target = Clamp0360(target);

            if (current > target)
            {
                if (current - target < 180.0f)
                    result -= Mathf.Min(current - target, Mathf.Abs(maxDegreesDelta));
                else
                    result += Mathf.Min(target + 360.0f - current, Mathf.Abs(maxDegreesDelta));
            }
            else
            {
                if (target - current < 180.0f)
                    result += Mathf.Min(target - current, Mathf.Abs(maxDegreesDelta));
                else
                    result -= Mathf.Min(current + 360.0f - target, Mathf.Abs(maxDegreesDelta));
            }

            return Clamp0360(result);
        }
        #endregion

        #region Task
        /// <summary>
        /// Wraps the provided object into a completed Task.
        /// </summary>
        /// <param name="obj">The object to be wrapped in a Task.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>A completed Task containing the object.</returns>
        public static Task<T> AsCompletedTask<T>(this T obj) => Task.FromResult(obj);

        /// <summary>
        /// Converts the Task into an IEnumerator for Unity coroutine usage.
        /// </summary>
        /// <param name="task">The Task to convert.</param>
        /// <returns>An IEnumerator representation of the Task.</returns>
        public static IEnumerator AsCoroutine(this Task task)
        {
            while (!task.IsCompleted) yield return null;
            // When used on a faulted Task, GetResult() will propagate the original exception. 
            // see: https://devblogs.microsoft.com/pfxteam/task-exception-handling-in-net-4-5/
            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Marks a task to be forgotten, meaning any exceptions thrown by the task will be caught and handled.
        /// </summary>
        /// <param name="task">The task to be forgotten.</param>
        /// <param name="onException">The optional action to execute when an exception is caught. If provided, the exception will not be rethrown.</param>
        public static async void Forget(this Task task, Action<Exception> onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                if (onException == null)
                    throw exception;

                onException(exception);
            }
        }
        #endregion

        #region Normalize

        public static float Normalize01(int _value)
        {
            return _value / 100f;
        }
        public static float Normalize01(float _value)
        {
            return _value / 100f;
        }
        public static float Normalize01(float _value, float max)
        {
            return _value / max;
        }
        public static float Normalize01(int _value, float max)
        {
            return _value / max;
        }

        public static float UnNormalize01(int _value)
        {
            return _value * 100f;
        }
        public static float UnNormalize01(float _value)
        {
            return _value * 100f;
        }
        public static float UnNormalize01(float _value, float max)
        {
            return _value * max;
        }
        public static float UnNormalize01(int _value, float max)
        {
            return _value * max;
        }

        #endregion

        #region Lerp
        public static IEnumerator LerpTo(float from, float to, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                from = Mathf.LerpUnclamped(from, to, counter / duration);
                yield return null;
            }

            from = to;
        }

        public static IEnumerator LerpFrom(float to, float from, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                to = Mathf.LerpUnclamped(to, from, counter / duration);
                yield return null;
            }

            to = from;
        }

        public static IEnumerator LerpToPingPong(float from, float to, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                float pingpong = Mathf.PingPong(counter, duration) / duration;

                from = Mathf.LerpUnclamped(from, to, pingpong);
                yield return null;
            }

            from = to;
        }
        public static IEnumerator LerpMoveTo(this Transform transform, Vector3 target, float duration)
        {
            Vector3 diff = target - transform.position;
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.position += diff * (Time.deltaTime / duration);
                yield return null;
            }

            transform.position = target;
        }

        public static IEnumerator LerpRotateTo(this Transform transform, Quaternion target, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, target, Time.deltaTime / duration);
                yield return null;
            }

            transform.rotation = transform.rotation;
        }

        public static IEnumerator LerpRotateTo(this Transform transform, Vector3 target, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.localEulerAngles
                    = new Vector3
                    (
                        Mathf.LerpUnclamped(transform.localEulerAngles.x, target.x, Time.deltaTime / duration),
                        Mathf.LerpUnclamped(transform.localEulerAngles.y, target.y, Time.deltaTime / duration),
                        Mathf.LerpUnclamped(transform.localEulerAngles.z, target.z, Time.deltaTime / duration)
                    );

                yield return null;
            }

            transform.localEulerAngles = new Vector3(target.x, target.y, target.z);
        }

        #endregion

        #region IEnumerator
        public static IEnumerator SimpleDelay(float delay, Action action)
        {
            yield return Seconds(delay);

            action?.Invoke();
        }
        public static IEnumerator SimpleRealtimeDelay(float delay, Action action)
        {
            yield return new WaitForSecondsRealtime(delay);

            action?.Invoke();
        }

        public static IEnumerator While(MonoBehaviour mono, float duration,
            Action _while = null, Action _after = null)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                _while?.Invoke();

                yield return null;
            }

            _after?.Invoke();
        }

        public static IEnumerator While(MonoBehaviour mono, float duration,
            Action _while = null)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                _while?.Invoke();

                yield return null;
            }

            _while?.Invoke();
        }
        public static IEnumerator WaitWhile(this MonoBehaviour mono,
            System.Func<bool> condition,
            Action preAction = null, Action postAction = null)
        {
            while (condition())
            {
                preAction?.Invoke();

                yield return null; // Waits until the next frame
            }

            postAction?.Invoke();
        }

        public static IEnumerator WaitWhile(this MonoBehaviour mono,
            bool condition,
            Action preAction = null, Action postAction = null)
        {
            while (condition)
            {
                preAction?.Invoke();

                yield return null; // Waits until the next frame
            }

            postAction?.Invoke();
        }


        public static void PlayParticleWithLoopDelay(this ParticleSystem ps, MonoBehaviour host, float loopDelay)
        {
            if (host == null)
            {
                Debug.LogError("Host MonoBehaviour is required to run Coroutine.");
                return;
            }

            host.StartCoroutine(LoopWithDelay(ps, loopDelay));

            static IEnumerator LoopWithDelay(ParticleSystem ps, float loopDelay)
            {
                var main = ps.main;

                while (true)
                {
                    ps.Play();
                    yield return new WaitForSeconds(main.duration);
                    ps.Stop();

                    yield return new WaitForSeconds(loopDelay);
                }
            }
        }
        public static IEnumerator WaitForAnimation(this MonoBehaviour mono, Animator animator, string animationName, int layer = 0, Action onComplete = null)
        {
            while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName))
                yield return null;
            while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
                yield return null;
            onComplete?.Invoke();
        }

        public static IEnumerator WaitForAnimation(this MonoBehaviour mono, Animator animator, int animationHash, Action onComplete, int layer = 0)
        {
            while (animator.GetCurrentAnimatorStateInfo(layer).fullPathHash != animationHash)
                yield return null;
            while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
                yield return null;
            onComplete?.Invoke();
        }
        public static IEnumerator WaitForAnimation(this MonoBehaviour mono, Animator animator, int animationHash, Action onComplete)
        {
            while (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != animationHash)
                yield return null;
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                yield return null;
            onComplete?.Invoke();
        }

        public static IEnumerator Countdown(this MonoBehaviour mono, float countdownTime, Action onComplete)
        {
            float currentTime = countdownTime;
            while (currentTime > 0)
            {
                yield return Seconds(1f);
                currentTime--;
            }
            onComplete?.Invoke();
        }

        public static IEnumerator CountupCoroutine(this MonoBehaviour mono, float countupTime, Action onComplete)
        {
            float currentTime = 0;
            while (currentTime < countupTime)
            {
                yield return Seconds(1f);
                currentTime++;
            }
            onComplete?.Invoke();
        }

        public static IEnumerator WaitForAnimation(this MonoBehaviour mono, Animator animator, int animationHash, Action onComplete, int layer = 0, float timeout = 10f)
        {
            float elapsedTime = 0f;

            AnimatorStateInfo initialState = animator.GetCurrentAnimatorStateInfo(layer);

            while (animator.GetCurrentAnimatorStateInfo(layer).fullPathHash != animationHash)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > timeout)
                {
                    Debug.LogError($"Timeout waiting for animation hash {animationHash} to start");
                    yield break;
                }

                if (animator.IsInTransition(layer))
                {
                    Debug.Log("Animator is in transition...");
                }

                yield return null;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            while (stateInfo.fullPathHash == animationHash && stateInfo.normalizedTime < 1.0f)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > timeout)
                    yield break;

                stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator WaitForAnimationWithDebug(this MonoBehaviour mono, Animator animator, int animationHash, Action onComplete, int layer = 0, float timeout = 10f)
        {
            float elapsedTime = 0f;

            while (true)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

                if (animator.IsInTransition(layer))
                {
                    Debug.Log("Animator is in transition...");
                    AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layer);
                    Debug.Log($"Transition normalized time: {transitionInfo.normalizedTime}");
                }
                else if (stateInfo.fullPathHash == animationHash)
                {
                    Debug.Log($"Target animation {animationHash} started");
                    break;
                }

                elapsedTime += Time.deltaTime;
                if (elapsedTime > timeout)
                {
                    Debug.LogError($"Timeout waiting for animation {animationHash} to start");
                    yield break;
                }
                yield return null;
            }

            // Wait for animation completion
            elapsedTime = 0f;
            while (true)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

                if (animator.IsInTransition(layer))
                {
                    Debug.Log("Transition detected during animation playback");
                }

                // Check if we're still on the target animation and it hasn't completed
                if (stateInfo.fullPathHash == animationHash && stateInfo.normalizedTime < 1.0f)
                {
                    Debug.Log($"Animation progress: {stateInfo.normalizedTime}");
                }
                else if (stateInfo.fullPathHash == animationHash && stateInfo.normalizedTime >= 1.0f)
                {
                    Debug.Log($"Animation {animationHash} completed");
                    onComplete?.Invoke();
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"Animation changed unexpectedly. Current hash: {stateInfo.fullPathHash}");
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                if (elapsedTime > timeout)
                {
                    Debug.LogError($"Timeout waiting for animation {animationHash} to complete");
                    yield break;
                }
                yield return null;
            }
        }

        public static IEnumerator WaitForAnimationWithDebug(this MonoBehaviour mono, Animator animator, int animationHash, float clipLength, Action onComplete, int layer = 0, float timeout = 10f)
        {
            float elapsedTime = 0f;
            bool animationDetected = false;
            float timeWaitedAfterStart = 0f;

            Debug.Log($"Expected animation hash: {animationHash}, waiting for {clipLength}s duration");

            while (elapsedTime < timeout)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
                bool isInTransition = animator.IsInTransition(layer);

                Debug.Log($"Layer {layer} - Hash: {stateInfo.fullPathHash}, NormalizedTime: {stateInfo.normalizedTime:F2}, InTransition: {isInTransition}, Elapsed: {elapsedTime:F2}s");

                if (!animationDetected && stateInfo.fullPathHash == animationHash)
                {
                    Debug.Log($"Animation {animationHash} detected on layer {layer}");
                    animationDetected = true;
                }

                if (animationDetected && stateInfo.fullPathHash == animationHash)
                {
                    timeWaitedAfterStart += Time.deltaTime;
                    if (timeWaitedAfterStart >= clipLength && !isInTransition)
                    {
                        Debug.Log($"Animation {animationHash} completed after {timeWaitedAfterStart:F2}s");
                        onComplete?.Invoke();
                        yield break;
                    }
                }
                else if (animationDetected && stateInfo.fullPathHash != animationHash)
                {
                    Debug.LogWarning($"Animation changed to hash {stateInfo.fullPathHash} before completion");
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Debug.LogError($"Timeout after {timeout}s waiting for animation {animationHash}");
        }

        public static IEnumerator WaitForAudio(this MonoBehaviour mono, AudioSource audioSource, Action onComplete, float timeout = 10f)
        {
            if (audioSource == null)
            {
                Debug.LogWarning("WaitForAudio failed: AudioSource is null.");
                yield break;
            }

            float timer = 0f;

            // Wait until the audio starts playing (but don't timeout here)
            while (!audioSource.isPlaying && audioSource.clip != null && audioSource.time <= 0f)
            {
                yield return null;
            }

            // Now wait until it's done or timeout
            while (audioSource.isPlaying)
            {
                if (timer >= timeout)
                {
                    Debug.LogWarning("WaitForAudio timed out.");
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator Countdown(this MonoBehaviour mono, float duration, Action onTick = null, Action onComplete = null)
        {
            float remainingTime = duration;

            while (remainingTime > 0)
            {
                onTick?.Invoke();
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();

            //    StartCoroutine(CountdownUtility.Countdown(
            //    5f, // 5 seconds
            //    (timeLeft) => Debug.Log($"Time remaining: {timeLeft:F1} seconds"),
            //    () => Debug.Log("Countdown finished!")
            //));
        }

        public static IEnumerator Countdown(this MonoBehaviour mono, float duration, System.Action<float> onTick = null, System.Action onComplete = null)
        {
            float remainingTime = duration;

            while (remainingTime > 0)
            {
                onTick?.Invoke(remainingTime);
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            onTick?.Invoke(0f);
            onComplete?.Invoke();

            //    StartCoroutine(CountdownUtility.Countdown(
            //    5f, // 5 seconds
            //    (timeLeft) => Debug.Log($"Time remaining: {timeLeft:F1} seconds"),
            //    () => Debug.Log("Countdown finished!")
            //));
        }

        public static IEnumerator Countup(this MonoBehaviour mono, float duration, System.Action<float> onTick = null, System.Action onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                onTick?.Invoke(elapsedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            onTick?.Invoke(duration);
            onComplete?.Invoke();
        }

        #endregion

        #region WaitFor
        static readonly WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate => fixedUpdate;

        static readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame => endOfFrame;

        static readonly Dictionary<float, WaitForSeconds> WaitForSecondsDict = new(100, new FloatComparer());
        public static WaitForSeconds Seconds(float seconds)
        {
            if (seconds < 1f / Application.targetFrameRate) return null;
            if (!WaitForSecondsDict.TryGetValue(seconds, out var forSeconds))
            {
                forSeconds = new WaitForSeconds(seconds);
                WaitForSecondsDict[seconds] = forSeconds;
            }
            return forSeconds;
        }

        class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Mathf.Epsilon;
            public int GetHashCode(float obj) => obj.GetHashCode();
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            return AkaitoAiExtensions.Seconds(seconds);
        }

        /// <summary>
        /// Clears the console log in the Unity Editor.
        /// </summary
#if UNITY_EDITOR
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif
        #endregion

        #region PlayerPrefs
        static private int endianDiff1;
        static private int endianDiff2;
        static private int idx;
        static private byte[] byteBlock;

        enum ArrayType { Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color }

        public static T GetJsonFromPrefs<T>(this PlayerPrefs prefs, string key)
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        }
        public static void SetIntByID(string key, int value, int id)
        {
            PlayerPrefs.SetInt(key + id, value);
        }

        public static void GetIntByID(string key, int id)
        {
            PlayerPrefs.GetInt(key + id);
        }
        public static void SetFloatByID(string key, float value, int id)
        {
            PlayerPrefs.SetFloat(key + id, value);
        }

        public static void GetFloatByID(string key, int id)
        {
            PlayerPrefs.GetFloat(key + id);
        }

        public static bool SetBool(String name, bool value)
        {
            try
            {
                PlayerPrefs.SetInt(name, value ? 1 : 0);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetBool(String name)
        {
            return PlayerPrefs.GetInt(name) == 1;
        }

        public static bool GetBool(String name, bool defaultValue)
        {
            return (1 == PlayerPrefs.GetInt(name, defaultValue ? 1 : 0));
        }

        public static long GetLong(string key, long defaultValue)
        {
            int lowBits, highBits;
            SplitLong(defaultValue, out lowBits, out highBits);
            lowBits = PlayerPrefs.GetInt(key + "_lowBits", lowBits);
            highBits = PlayerPrefs.GetInt(key + "_highBits", highBits);

            // unsigned, to prevent loss of sign bit.
            ulong ret = (uint)highBits;
            ret = (ret << 32);
            return (long)(ret | (ulong)(uint)lowBits);
        }

        public static long GetLong(string key)
        {
            int lowBits = PlayerPrefs.GetInt(key + "_lowBits");
            int highBits = PlayerPrefs.GetInt(key + "_highBits");

            // unsigned, to prevent loss of sign bit.
            ulong ret = (uint)highBits;
            ret = (ret << 32);
            return (long)(ret | (ulong)(uint)lowBits);
        }

        private static void SplitLong(long input, out int lowBits, out int highBits)
        {
            // unsigned everything, to prevent loss of sign bit.
            lowBits = (int)(uint)(ulong)input;
            highBits = (int)(uint)(input >> 32);
        }

        public static void SetLong(string key, long value)
        {
            int lowBits, highBits;
            SplitLong(value, out lowBits, out highBits);
            PlayerPrefs.SetInt(key + "_lowBits", lowBits);
            PlayerPrefs.SetInt(key + "_highBits", highBits);
        }

        public static bool SetVector2(String key, Vector2 vector)
        {
            return SetFloatArray(key, new float[] { vector.x, vector.y });
        }

        static Vector2 GetVector2(String key)
        {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 2)
            {
                return Vector2.zero;
            }
            return new Vector2(floatArray[0], floatArray[1]);
        }

        public static Vector2 GetVector2(String key, Vector2 defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetVector2(key);
            }
            return defaultValue;
        }

        public static bool SetVector3(String key, Vector3 vector)
        {
            return SetFloatArray(key, new float[] { vector.x, vector.y, vector.z });
        }

        public static Vector3 GetVector3(String key)
        {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 3)
            {
                return Vector3.zero;
            }
            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }

        public static Vector3 GetVector3(String key, Vector3 defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetVector3(key);
            }
            return defaultValue;
        }

        public static bool SetQuaternion(String key, Quaternion vector)
        {
            return SetFloatArray(key, new float[] { vector.x, vector.y, vector.z, vector.w });
        }

        public static Quaternion GetQuaternion(String key)
        {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 4)
            {
                return Quaternion.identity;
            }
            return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }

        public static Quaternion GetQuaternion(String key, Quaternion defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetQuaternion(key);
            }
            return defaultValue;
        }

        public static bool SetColor(String key, Color color)
        {
            return SetFloatArray(key, new float[] { color.r, color.g, color.b, color.a });
        }

        public static Color GetColor(String key)
        {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 4)
            {
                return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }

        public static Color GetColor(String key, Color defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetColor(key);
            }
            return defaultValue;
        }

        public static bool SetBoolArray(String key, bool[] boolArray)
        {
            // Make a byte array that's a multiple of 8 in length, plus 5 bytes to store the number of entries as an int32 (+ identifier)
            // We have to store the number of entries, since the boolArray length might not be a multiple of 8, so there could be some padded zeroes
            var bytes = new byte[(boolArray.Length + 7) / 8 + 5];
            bytes[0] = System.Convert.ToByte(ArrayType.Bool);   // Identifier
            var bits = new BitArray(boolArray);
            bits.CopyTo(bytes, 5);
            Initialize();
            ConvertInt32ToBytes(boolArray.Length, bytes); // The number of entries in the boolArray goes in the first 4 bytes

            return SaveBytes(key, bytes);
        }

        public static bool[] GetBoolArray(String key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                if (bytes.Length < 5)
                {
                    Debug.LogError("Corrupt preference file for " + key);
                    return new bool[0];
                }
                if ((ArrayType)bytes[0] != ArrayType.Bool)
                {
                    Debug.LogError(key + " is not a boolean array");
                    return new bool[0];
                }
                Initialize();

                // Make a new bytes array that doesn't include the number of entries + identifier (first 5 bytes) and turn that into a BitArray
                var bytes2 = new byte[bytes.Length - 5];
                System.Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
                var bits = new BitArray(bytes2);
                // Get the number of entries from the first 4 bytes after the identifier and resize the BitArray to that length, then convert it to a boolean array
                bits.Length = ConvertBytesToInt32(bytes);
                var boolArray = new bool[bits.Count];
                bits.CopyTo(boolArray, 0);

                return boolArray;
            }
            return new bool[0];
        }

        public static bool[] GetBoolArray(String key, bool defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetBoolArray(key);
            }
            var boolArray = new bool[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                boolArray[i] = defaultValue;
            }
            return boolArray;
        }

        public static bool SetStringArray(String key, String[] stringArray)
        {
            var bytes = new byte[stringArray.Length + 1];
            bytes[0] = System.Convert.ToByte(ArrayType.String); // Identifier
            Initialize();

            // Store the length of each string that's in stringArray, so we can extract the correct strings in GetStringArray
            for (var i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] == null)
                {
                    Debug.LogError("Can't save null entries in the string array when setting " + key);
                    return false;
                }
                if (stringArray[i].Length > 255)
                {
                    Debug.LogError("Strings cannot be longer than 255 characters when setting " + key);
                    return false;
                }
                bytes[idx++] = (byte)stringArray[i].Length;
            }

            try
            {
                PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes) + "|" + String.Join("", stringArray));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static String[] GetStringArray(String key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var completeString = PlayerPrefs.GetString(key);
                var separatorIndex = completeString.IndexOf("|"[0]);
                if (separatorIndex < 4)
                {
                    Debug.LogError("Corrupt preference file for " + key);
                    return new String[0];
                }
                var bytes = System.Convert.FromBase64String(completeString.Substring(0, separatorIndex));
                if ((ArrayType)bytes[0] != ArrayType.String)
                {
                    Debug.LogError(key + " is not a string array");
                    return new String[0];
                }
                Initialize();

                var numberOfEntries = bytes.Length - 1;
                var stringArray = new String[numberOfEntries];
                var stringIndex = separatorIndex + 1;
                for (var i = 0; i < numberOfEntries; i++)
                {
                    int stringLength = bytes[idx++];
                    if (stringIndex + stringLength > completeString.Length)
                    {
                        Debug.LogError("Corrupt preference file for " + key);
                        return new String[0];
                    }
                    stringArray[i] = completeString.Substring(stringIndex, stringLength);
                    stringIndex += stringLength;
                }

                return stringArray;
            }
            return new String[0];
        }

        public static String[] GetStringArray(String key, String defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetStringArray(key);
            }
            var stringArray = new String[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                stringArray[i] = defaultValue;
            }
            return stringArray;
        }

        public static bool SetIntArray(String key, int[] intArray)
        {
            return SetValue(key, intArray, ArrayType.Int32, 1, ConvertFromInt);
        }

        public static bool SetFloatArray(String key, float[] floatArray)
        {
            return SetValue(key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
        }

        public static bool SetVector2Array(String key, Vector2[] vector2Array)
        {
            return SetValue(key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
        }

        public static bool SetVector3Array(String key, Vector3[] vector3Array)
        {
            return SetValue(key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
        }

        public static bool SetQuaternionArray(String key, Quaternion[] quaternionArray)
        {
            return SetValue(key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
        }

        public static bool SetColorArray(String key, Color[] colorArray)
        {
            return SetValue(key, colorArray, ArrayType.Color, 4, ConvertFromColor);
        }

        private static bool SetValue<T>(String key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList
        {
            var bytes = new byte[(4 * array.Count) * vectorNumber + 1];
            bytes[0] = System.Convert.ToByte(arrayType);    // Identifier
            Initialize();

            for (var i = 0; i < array.Count; i++)
            {
                convert(array, bytes, i);
            }
            return SaveBytes(key, bytes);
        }

        private static void ConvertFromInt(int[] array, byte[] bytes, int i)
        {
            ConvertInt32ToBytes(array[i], bytes);
        }

        private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
        {
            ConvertFloatToBytes(array[i], bytes);
        }

        private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i)
        {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
        }

        private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i)
        {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
            ConvertFloatToBytes(array[i].z, bytes);
        }

        private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i)
        {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
            ConvertFloatToBytes(array[i].z, bytes);
            ConvertFloatToBytes(array[i].w, bytes);
        }

        private static void ConvertFromColor(Color[] array, byte[] bytes, int i)
        {
            ConvertFloatToBytes(array[i].r, bytes);
            ConvertFloatToBytes(array[i].g, bytes);
            ConvertFloatToBytes(array[i].b, bytes);
            ConvertFloatToBytes(array[i].a, bytes);
        }

        public static int[] GetIntArray(String key)
        {
            var intList = new List<int>();
            GetValue(key, intList, ArrayType.Int32, 1, ConvertToInt);
            return intList.ToArray();
        }

        public static int[] GetIntArray(String key, int defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetIntArray(key);
            }
            var intArray = new int[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                intArray[i] = defaultValue;
            }
            return intArray;
        }

        public static float[] GetFloatArray(String key)
        {
            var floatList = new List<float>();
            GetValue(key, floatList, ArrayType.Float, 1, ConvertToFloat);
            return floatList.ToArray();
        }

        public static float[] GetFloatArray(String key, float defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetFloatArray(key);
            }
            var floatArray = new float[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                floatArray[i] = defaultValue;
            }
            return floatArray;
        }

        public static Vector2[] GetVector2Array(String key)
        {
            var vector2List = new List<Vector2>();
            GetValue(key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
            return vector2List.ToArray();
        }

        public static Vector2[] GetVector2Array(String key, Vector2 defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetVector2Array(key);
            }
            var vector2Array = new Vector2[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                vector2Array[i] = defaultValue;
            }
            return vector2Array;
        }

        public static Vector3[] GetVector3Array(String key)
        {
            var vector3List = new List<Vector3>();
            GetValue(key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
            return vector3List.ToArray();
        }

        public static Vector3[] GetVector3Array(String key, Vector3 defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))

            {
                return GetVector3Array(key);
            }
            var vector3Array = new Vector3[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                vector3Array[i] = defaultValue;
            }
            return vector3Array;
        }

        public static Quaternion[] GetQuaternionArray(String key)
        {
            var quaternionList = new List<Quaternion>();
            GetValue(key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
            return quaternionList.ToArray();
        }

        public static Quaternion[] GetQuaternionArray(String key, Quaternion defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetQuaternionArray(key);
            }
            var quaternionArray = new Quaternion[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                quaternionArray[i] = defaultValue;
            }
            return quaternionArray;
        }

        public static Color[] GetColorArray(String key)
        {
            var colorList = new List<Color>();
            GetValue(key, colorList, ArrayType.Color, 4, ConvertToColor);
            return colorList.ToArray();
        }

        public static Color[] GetColorArray(String key, Color defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return GetColorArray(key);
            }
            var colorArray = new Color[defaultSize];
            for (int i = 0; i < defaultSize; i++)
            {
                colorArray[i] = defaultValue;
            }
            return colorArray;
        }

        private static void GetValue<T>(String key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
        {
            if (PlayerPrefs.HasKey(key))
            {
                var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                if ((bytes.Length - 1) % (vectorNumber * 4) != 0)
                {
                    Debug.LogError("Corrupt preference file for " + key);
                    return;
                }
                if ((ArrayType)bytes[0] != arrayType)
                {
                    Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
                    return;
                }
                Initialize();

                var end = (bytes.Length - 1) / (vectorNumber * 4);
                for (var i = 0; i < end; i++)
                {
                    convert(list, bytes);
                }
            }
        }

        private static void ConvertToInt(List<int> list, byte[] bytes)
        {
            list.Add(ConvertBytesToInt32(bytes));
        }

        private static void ConvertToFloat(List<float> list, byte[] bytes)
        {
            list.Add(ConvertBytesToFloat(bytes));
        }

        private static void ConvertToVector2(List<Vector2> list, byte[] bytes)
        {
            list.Add(new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToVector3(List<Vector3> list, byte[] bytes)
        {
            list.Add(new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes)
        {
            list.Add(new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToColor(List<Color> list, byte[] bytes)
        {
            list.Add(new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        public static void ShowArrayType(String key)
        {
            var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
            if (bytes.Length > 0)
            {
                ArrayType arrayType = (ArrayType)bytes[0];
                Debug.Log(key + " is a " + arrayType.ToString() + " array");
            }
        }

        private static void Initialize()
        {
            if (System.BitConverter.IsLittleEndian)
            {
                endianDiff1 = 0;
                endianDiff2 = 0;
            }
            else
            {
                endianDiff1 = 3;
                endianDiff2 = 1;
            }
            if (byteBlock == null)
            {
                byteBlock = new byte[4];
            }
            idx = 1;
        }

        private static bool SaveBytes(String key, byte[] bytes)
        {
            try
            {
                PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes));
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void ConvertFloatToBytes(float f, byte[] bytes)
        {
            byteBlock = System.BitConverter.GetBytes(f);
            ConvertTo4Bytes(bytes);
        }

        private static float ConvertBytesToFloat(byte[] bytes)
        {
            ConvertFrom4Bytes(bytes);
            return System.BitConverter.ToSingle(byteBlock, 0);
        }

        private static void ConvertInt32ToBytes(int i, byte[] bytes)
        {
            byteBlock = System.BitConverter.GetBytes(i);
            ConvertTo4Bytes(bytes);
        }

        private static int ConvertBytesToInt32(byte[] bytes)
        {
            ConvertFrom4Bytes(bytes);
            return System.BitConverter.ToInt32(byteBlock, 0);
        }

        private static void ConvertTo4Bytes(byte[] bytes)
        {
            bytes[idx] = byteBlock[endianDiff1];
            bytes[idx + 1] = byteBlock[1 + endianDiff2];
            bytes[idx + 2] = byteBlock[2 - endianDiff2];
            bytes[idx + 3] = byteBlock[3 - endianDiff1];
            idx += 4;
        }

        private static void ConvertFrom4Bytes(byte[] bytes)
        {
            byteBlock[endianDiff1] = bytes[idx];
            byteBlock[1 + endianDiff2] = bytes[idx + 1];
            byteBlock[2 - endianDiff2] = bytes[idx + 2];
            byteBlock[3 - endianDiff1] = bytes[idx + 3];
            idx += 4;
        }

        #endregion

        #region Color
        public static Color WithR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color WithG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }
        public static Color WithB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }
        public static Color WithA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value)
        {
            return value.ToString("X2");
        }

        // Returns 0-255
        public static int Hex_to_Dec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value)
        {
            return Dec_to_Hex((int)Mathf.Round(value * 255f));
        }

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex)
        {
            return Hex_to_Dec(hex) / 255f;
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color)
        {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color)
        {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color) + alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
        {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b)
        {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a)
        {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r, g, b) + alpha;
        }

        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color)
        {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8)
            {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }

        // Return a color going from Red to Yellow to Green, like a heat map
        public static Color GetRedGreenColor(float value)
        {
            float r = 0f;
            float g = 0f;
            if (value <= .5f)
            {
                r = 1f;
                g = value * 2f;
            }
            else
            {
                g = 1f;
                r = 1f - (value - .5f) * 2f;
            }
            return new Color(r, g, 0f, 1f);
        }


        public static Color GetRandomColor()
        {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
        }

        private static int sequencialColorIndex = -1;
        private static Color[] sequencialColors = new[] {
            GetColorFromString("26a6d5"),
            GetColorFromString("41d344"),
            GetColorFromString("e6e843"),
            GetColorFromString("e89543"),
            GetColorFromString("0f6ad0"),//("d34141"),
		    GetColorFromString("b35db6"),
            GetColorFromString("c45947"),
            GetColorFromString("9447c4"),
            GetColorFromString("4756c4"),
        };

        public static void ResetSequencialColors()
        {
            sequencialColorIndex = -1;
        }

        public static Color GetSequencialColor()
        {
            sequencialColorIndex = (sequencialColorIndex + 1) % sequencialColors.Length;
            return sequencialColors[sequencialColorIndex];
        }

        public static Color GetColor255(float red, float green, float blue, float alpha = 255f)
        {
            return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
        }

        // Take two color arrays (pixels) and merge them
        public static void MergeColorArrays(Color[] baseArray, Color[] overlay)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (overlay[i].a > 0)
                {
                    // Not empty color
                    if (overlay[i].a >= 1)
                    {
                        // Fully replace
                        baseArray[i] = overlay[i];
                    }
                    else
                    {
                        // Interpolate colors
                        float alpha = overlay[i].a;
                        baseArray[i].r += (overlay[i].r - baseArray[i].r) * alpha;
                        baseArray[i].g += (overlay[i].g - baseArray[i].g) * alpha;
                        baseArray[i].b += (overlay[i].b - baseArray[i].b) * alpha;
                        baseArray[i].a += overlay[i].a;
                    }
                }
            }
        }

        // Replace color in baseArray with replaceArray if baseArray[i] != ignoreColor
        public static void ReplaceColorArrays(Color[] baseArray, Color[] replaceArray, Color ignoreColor)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (baseArray[i] != ignoreColor)
                {
                    baseArray[i] = replaceArray[i];
                }
            }
        }

        public static void MaskColorArrays(Color[] baseArray, Color[] mask)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (baseArray[i].a > 0f)
                {
                    baseArray[i].a = mask[i].a;
                }
            }
        }

        public static void TintColorArray(Color[] baseArray, Color tint)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                // Apply tint
                baseArray[i].r = tint.r * baseArray[i].r;
                baseArray[i].g = tint.g * baseArray[i].g;
                baseArray[i].b = tint.b * baseArray[i].b;
            }
        }
        public static void TintColorArrayInsideMask(Color[] baseArray, Color tint, Color[] mask)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (mask[i].a > 0)
                {
                    // Apply tint
                    Color baseColor = baseArray[i];
                    Color fullyTintedColor = tint * baseColor;
                    float interpolateAmount = mask[i].a;
                    baseArray[i].r = baseColor.r + (fullyTintedColor.r - baseColor.r) * interpolateAmount;
                    baseArray[i].g = baseColor.g + (fullyTintedColor.g - baseColor.g) * interpolateAmount;
                    baseArray[i].b = baseColor.b + (fullyTintedColor.b - baseColor.b) * interpolateAmount;
                }
            }
        }

        public static Color TintColor(Color baseColor, Color tint)
        {
            // Apply tint
            baseColor.r = tint.r * baseColor.r;
            baseColor.g = tint.g * baseColor.g;
            baseColor.b = tint.b * baseColor.b;

            return baseColor;
        }

        public static bool IsColorSimilar255(Color colorA, Color colorB, int maxDiff)
        {
            return IsColorSimilar(colorA, colorB, maxDiff / 255f);
        }

        public static bool IsColorSimilar(Color colorA, Color colorB, float maxDiff)
        {
            float rDiff = Mathf.Abs(colorA.r - colorB.r);
            float gDiff = Mathf.Abs(colorA.g - colorB.g);
            float bDiff = Mathf.Abs(colorA.b - colorB.b);
            float aDiff = Mathf.Abs(colorA.a - colorB.a);

            float totalDiff = rDiff + gDiff + bDiff + aDiff;
            return totalDiff < maxDiff;
        }

        public static float GetColorDifference(Color colorA, Color colorB)
        {
            float rDiff = Mathf.Abs(colorA.r - colorB.r);
            float gDiff = Mathf.Abs(colorA.g - colorB.g);
            float bDiff = Mathf.Abs(colorA.b - colorB.b);
            float aDiff = Mathf.Abs(colorA.a - colorB.a);

            float totalDiff = rDiff + gDiff + bDiff + aDiff;
            return totalDiff;
        }


        #endregion

        #region LayerMask
        public static LayerMask WithLayers(this LayerMask layerMask, params int[] layers)
        {
            foreach (var layer in layers)
            {
                layerMask |= 1 << layer;
            }

            return layerMask;
        }

        public static LayerMask WithLayers(this LayerMask layerMask, params string[] layerNames)
        {
            foreach (var name in layerNames)
            {
                var layer = LayerMask.NameToLayer(name);
                layerMask |= 1 << layer;
            }

            return layerMask;
        }

        public static LayerMask WithoutLayers(this LayerMask layerMask, params int[] layers)
        {
            foreach (var layer in layers)
            {
                layerMask &= ~(1 << layer);
            }

            return layerMask;
        }

        public static LayerMask WithoutLayers(this LayerMask layerMask, params string[] layerNames)
        {
            foreach (var name in layerNames)
            {
                var layer = LayerMask.NameToLayer(name);
                layerMask &= ~(1 << layer);
            }

            return layerMask;
        }
        #endregion

        #region UI
        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);

        public const int sortingOrderDefault = 5000;

        // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
        {
            return (int)(baseSortingOrder - position.y) + offset;
        }

        private static Transform cachedCanvasTransform;
        public static Transform GetCanvasTransform()
        {
            if (cachedCanvasTransform == null)
            {
                Canvas canvas = MonoBehaviour.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    cachedCanvasTransform = canvas.transform;
                }
            }
            return cachedCanvasTransform;
        }

        // Get Default Unity Font, used in text objects if no font given
        public static Font GetDefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // Create a Sprite in the World, no parent
        public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
        {
            return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
        }
        // Create a Sprite in the World
        public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            return gameObject;
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        // Draw a UI Sprite
        public static RectTransform DrawSprite(Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
        {
            RectTransform rectTransform = DrawSprite(null, color, parent, pos, size, name);
            return rectTransform;
        }

        // Draw a UI Sprite
        public static RectTransform DrawSprite(Sprite sprite, Transform parent, Vector2 pos, Vector2 size, string name = null)
        {
            RectTransform rectTransform = DrawSprite(sprite, Color.white, parent, pos, size, name);
            return rectTransform;
        }

        // Draw a UI Sprite
        public static RectTransform DrawSprite(Sprite sprite, Color color, Transform parent, Vector2 pos, Vector2 size, string name = null)
        {
            // Setup icon
            if (name == null || name == "") name = "Sprite";
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            RectTransform goRectTransform = go.GetComponent<RectTransform>();
            goRectTransform.SetParent(parent, false);
            goRectTransform.sizeDelta = size;
            goRectTransform.anchoredPosition = pos;

            Image image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;

            return goRectTransform;
        }

        public static Text DrawTextUI(string textString, Vector2 anchoredPosition, int fontSize, Font font)
        {
            return DrawTextUI(textString, GetCanvasTransform(), anchoredPosition, fontSize, font);
        }

        public static Text DrawTextUI(string textString, Transform parent, Vector2 anchoredPosition, int fontSize, Font font)
        {
            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(parent, false);
            Transform textGoTrans = textGo.transform;
            textGoTrans.SetParent(parent, false);
            textGoTrans.localPosition = Vector3zero;
            textGoTrans.localScale = Vector3one;

            RectTransform textGoRectTransform = textGo.GetComponent<RectTransform>();
            textGoRectTransform.sizeDelta = new Vector2(0, 0);
            textGoRectTransform.anchoredPosition = anchoredPosition;

            Text text = textGo.GetComponent<Text>();
            text.text = textString;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.alignment = TextAnchor.MiddleLeft;
            if (font == null) font = GetDefaultFont();
            text.font = font;
            text.fontSize = fontSize;

            return text;
        }


        // Parse a float, return default if failed
        public static float Parse_Float(string txt, float _default)
        {
            float f;
            if (!float.TryParse(txt, out f))
            {
                f = _default;
            }
            return f;
        }

        // Parse a int, return default if failed
        public static int Parse_Int(string txt, int _default)
        {
            int i;
            if (!int.TryParse(txt, out i))
            {
                i = _default;
            }
            return i;
        }

        public static int Parse_Int(string txt)
        {
            return Parse_Int(txt, -1);
        }



        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }



        // Is Mouse over a UI Element? Used for ignoring World clicks through UI
        public static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            else
            {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }

        // Get UI Position from World Position
        public static Vector2 GetWorldUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera)
        {
            Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }

        public static Vector3 GetWorldPositionFromUIZeroZ()
        {
            Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        // Get World Position from UI Position
        public static Vector3 GetWorldPositionFromUI()
        {
            return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI(Camera worldCamera)
        {
            return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetWorldPositionFromUI_Perspective()
        {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera)
        {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera)
        {
            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        #endregion

        #region JSON
        [System.Serializable]
        private class JsonDictionary
        {
            public List<string> keyList = new List<string>();
            public List<string> valueList = new List<string>();
        }

        // Take a Dictionary and return JSON string
        public static string SaveDictionaryJson<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            JsonDictionary jsonDictionary = new JsonDictionary();
            foreach (TKey key in dictionary.Keys)
            {
                jsonDictionary.keyList.Add(JsonUtility.ToJson(key));
                jsonDictionary.valueList.Add(JsonUtility.ToJson(dictionary[key]));
            }
            string saveJson = JsonUtility.ToJson(jsonDictionary);
            return saveJson;
        }

        // Take a JSON string and return Dictionary<T1, T2>
        public static Dictionary<TKey, TValue> LoadDictionaryJson<TKey, TValue>(string saveJson)
        {
            JsonDictionary jsonDictionary = JsonUtility.FromJson<JsonDictionary>(saveJson);
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
            for (int i = 0; i < jsonDictionary.keyList.Count; i++)
            {
                TKey key = JsonUtility.FromJson<TKey>(jsonDictionary.keyList[i]);
                TValue value = JsonUtility.FromJson<TValue>(jsonDictionary.valueList[i]);
                ret[key] = value;
            }
            return ret;
        }
        #endregion

        #region String
        // Returns a random script that can be used to id
        public static string GetIdString()
        {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywz";
            string ret = "";
            for (int i = 0; i < 8; i++)
            {
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            }
            return ret;
        }

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong()
        {
            return GetIdStringLong(10);
        }

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong(int chars)
        {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
            string ret = "";
            for (int i = 0; i < chars; i++)
            {
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            }
            return ret;
        }



        // Get a random male name and optionally single letter surname
        public static string GetRandomName(bool withSurname = false)
        {
            List<string> firstNameList = new List<string>(){"Gabe","Cliff","Tim","Ron","Jon","John","Mike","Seth","Alex","Steve","Chris","Will","Bill","James","Jim",
                                        "Ahmed","Omar","Peter","Pierre","George","Lewis","Lewie","Adam","William","Ali","Eddie","Ed","Dick","Robert","Bob","Rob",
                                        "Neil","Tyson","Carl","Chris","Christopher","Jensen","Gordon","Morgan","Richard","Wen","Wei","Luke","Lucas","Noah","Ivan","Yusuf",
                                        "Ezio","Connor","Milan","Nathan","Victor","Harry","Ben","Charles","Charlie","Jack","Leo","Leonardo","Dylan","Steven","Jeff",
                                        "Alex","Mark","Leon","Oliver","Danny","Liam","Joe","Tom","Thomas","Bruce","Clark","Tyler","Jared","Brad","Jason"};

            if (!withSurname)
            {
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)];
            }
            else
            {
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)] + " " + alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
            }
        }




        public static string GetRandomCityName()
        {
            List<string> cityNameList = new List<string>(){"Alabama","New York","Old York","Bangkok","Lisbon","Vee","Agen","Agon","Ardok","Arbok",
                            "Kobra","House","Noun","Hayar","Salma","Chancellor","Dascomb","Payn","Inglo","Lorr","Ringu",
                            "Brot","Mount Loom","Kip","Chicago","Madrid","London","Gam",
                            "Greenvile","Franklin","Clinton","Springfield","Salem","Fairview","Fairfax","Washington","Madison",
                            "Georgetown","Arlington","Marion","Oxford","Harvard","Valley","Ashland","Burlington","Manchester","Clayton",
                            "Milton","Auburn","Dayton","Lexington","Milford","Riverside","Cleveland","Dover","Hudson","Kingston","Mount Vernon",
                            "Newport","Oakland","Centerville","Winchester","Rotary","Bailey","Saint Mary","Three Waters","Veritas","Chaos","Center",
                            "Millbury","Stockland","Deerstead Hills","Plaintown","Fairchester","Milaire View","Bradton","Glenfield","Kirkmore",
                            "Fortdell","Sharonford","Inglewood","Englecamp","Harrisvania","Bosstead","Brookopolis","Metropolis","Colewood","Willowbury",
                            "Hearthdale","Weelworth","Donnelsfield","Greenline","Greenwich","Clarkswich","Bridgeworth","Normont",
                            "Lynchbrook","Ashbridge","Garfort","Wolfpain","Waterstead","Glenburgh","Fortcroft","Kingsbank","Adamstead","Mistead",
                            "Old Crossing","Crossing","New Agon","New Agen","Old Agon","New Valley","Old Valley","New Kingsbank","Old Kingsbank",
            "New Dover","Old Dover","New Burlington","Shawshank","Old Shawshank","New Shawshank","New Bradton", "Old Bradton","New Metropolis","Old Clayton","New Clayton"
        };
            return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
        }

        /// <summary>Checks if a string is Null or white space</summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>Checks if a string is Null or empty</summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>Checks if a string contains null, empty or white space.</summary>
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>Checks if a string is null and returns an empty string if it is.</summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        /// <summary>
        /// Shortens a string to the specified maximum length. If the string's length
        /// is less than the maxLength, the original string is returned.
        /// </summary>
        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }

        /// <summary>Slices a string from the start index to the end index.</summary>
        /// <result>The sliced string.</result>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
            {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (startIndex < 0 || startIndex > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Converts the input string to an alphanumeric string, optionally allowing periods.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <param name="allowPeriods">A boolean flag indicating whether periods should be allowed in the output string.</param>
        /// <returns>
        /// A new string containing only alphanumeric characters, underscores, and optionally periods.
        /// If the input string is null or empty, an empty string is returned.
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            List<char> filteredChars = new List<char>();
            int lastValidIndex = -1;

            // Iterate over the input string, filtering and determining valid start/end indices
            foreach (char character in input
                         .Where(character => char
                             .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                         .Where(character => filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.')))
            {

                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // Update lastValidIndex for valid characters
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
            {
                lastValidIndex--;
            }

            // Return the filtered string
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1) : string.Empty;
        }

        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(this string text) => $"<b>{text}</b>";
        public static string RichItalic(this string text) => $"<i>{text}</i>";
        public static string RichUnderline(this string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";
        public static string RichGradient(this string text, string color1, string color2) => $"<gradient={color1},{color2}>{text}</gradient>";
        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";


        #endregion

        #region Sound
        public static void PlayAudioClip(this AudioSource audioSource, AudioClip clip)
        {
            if (audioSource == null || clip == null)
            {
                Debug.LogError("AudioSource or AudioClip is null.");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

        public static void StopAudioClip(this AudioSource audioSource, AudioClip clip)
        {
            if (audioSource == null || clip == null)
            {
                Debug.LogError("AudioSource or AudioClip is null.");
                return;
            }

            if (audioSource.clip != clip) return;

            audioSource.Stop();
            audioSource.clip = null;
        }

        public static void StopAudioClip(this AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is null.");
                return;
            }

            audioSource.Stop();
            audioSource.clip = null;
        }

        public static void PlayClipAndWait(this AudioSource audioSource, AudioClip clip, MonoBehaviour monoBehaviour, Action onFinish)
        {
            if (audioSource == null || clip == null || monoBehaviour == null)
            {
                Debug.LogError("AudioSource, AudioClip, or MonoBehaviour is null.");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
            monoBehaviour.StartCoroutine(WaitForAudioToFinish(audioSource, onFinish));
        }

        public static IEnumerator WaitForAudioToFinish(AudioSource audioSource, Action func)
        {
            while (audioSource.isPlaying)
            {
                yield return null; // Wait for the next frame
            }

            Debug.Log("AudioClip has finished playing.");
            func?.Invoke();
        }

        #endregion

        #region Animation
        public static IEnumerator WaitForAnimation(this Animator animator, string animationName, Action onFinish)
        {
            // Wait for the animation to start
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return null;
            }

            // Wait until the animation has finished
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            onFinish?.Invoke();
        }
        public static IEnumerator WaitForAnimation(this Animator animator, string animationName, Action onStart, Action onFinish)
        {
            onStart?.Invoke();

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            onFinish?.Invoke();
        }


        #endregion

        #region Camera
        // Extension method for Camera to change viewport
        public static void ChangeViewport(this Camera camera, Vector2 XY, Vector2 WidthHeight)
        {
            if (camera != null)
            {
                camera.rect = new Rect(XY.x, XY.y, WidthHeight.x, WidthHeight.y);
            }
            else
            {
                Debug.LogWarning("Camera is null! Unable to change viewport.");
            }
        }

        public static void ChangeViewport(this Camera camera, Vector2 XY)
        {
            if (camera != null)
            {
                camera.rect = new Rect(XY.x, XY.y, 1f, 1f);
            }
            else
            {
                Debug.LogWarning("Camera is null! Unable to change viewport.");
            }
        }

        public static void ChangeViewport(this Camera camera, float x, float y)
        {
            if (camera != null)
            {
                camera.rect = new Rect(x, y, 1f, 1f);
            }
            else
            {
                Debug.LogWarning("Camera is null! Unable to change viewport.");
            }
        }
        public static void ChangeViewport(this Camera camera, float x, float y, float width, float height)
        {
            if (camera != null)
            {
                camera.rect = new Rect(x, y, width, height);
            }
            else
            {
                Debug.LogWarning("Camera is null! Unable to change viewport.");
            }
        }
        #endregion

        #region BinarySerialization
        public static void SaveAsBinary<T>(this T data, string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data to {filePath}: {ex.Message}");
            }
        }

        public static T LoadFromBinary<T>(this string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading data from {filePath}: {ex.Message}");
                return default(T);
            }
        }
    }

    //    // Usage Example
    //    [Serializable]
    //    public class PlayerData
    //    {
    //    // ... your data properties
    //}

    //// ...

    //PlayerData playerData = new PlayerData
    //{
    //    // ... initialize player data
    //};

    //playerData.SaveAsBinary("player_data.bin"); 

    //PlayerData loadedData = "player_data.bin".LoadFromBinary<PlayerData>();
    #endregion
}
