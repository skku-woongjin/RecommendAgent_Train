using UnityEngine;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    //one-hot 개조한거 
    /// Grid-based sensor with one-hot observations.
    /// </summary>
    public class AntGridSensor : GridSensorBase
    {
        /// <summary>
        /// Create a OneHotGridSensor with the specified configuration.
        /// </summary>
        /// <param name="name">The sensor name</param>
        /// <param name="cellScale">The scale of each cell in the grid</param>
        /// <param name="gridSize">Number of cells on each side of the grid</param>
        /// <param name="detectableTags">Tags to be detected by the sensor</param>
        /// <param name="compression">Compression type</param>
        public AntGridSensor(
            string name,
            Vector3 cellScale,
            Vector3Int gridSize,
            string[] detectableTags,
            SensorCompressionType compression
        ) : base(name, cellScale, gridSize, detectableTags, compression)
        {
        }

        /// <inheritdoc/>
        protected override int GetCellObservationSize()
        {
            return DetectableTags == null ? 0 : DetectableTags.Length;
        }

        /// <inheritdoc/>
        protected override bool IsDataNormalized()
        {
            return true;
        }

        /// <inheritdoc/>
        // protected internal override ProcessCollidersMethod GetProcessCollidersMethod()
        // {
        //     return ProcessCollidersMethod.ProcessClosestColliders;
        // }

        /// <summary>
        /// Get the one-hot representation of the detected game object's tag.
        /// </summary>
        /// <param name="detectedObject">The game object that was detected within a certain cell</param>
        /// <param name="tagIndex">The index of the detectedObject's tag in the DetectableObjects list</param>
        /// <param name="dataBuffer">The buffer to write the observation values.
        ///         The buffer size is configured by <seealso cref="GetCellObservationSize"/>.
        /// </param>
        protected override void GetObjectData(GameObject detectedObject, int tagIndex, float[] dataBuffer)
        {
            if (tagIndex == 0)
            {
                dataBuffer[tagIndex] = (detectedObject.transform.GetSiblingIndex() + 1.0f) / (detectedObject.transform.parent.childCount + 1);
            }
            if (tagIndex == 1)
                dataBuffer[tagIndex] = detectedObject.GetComponent<TrailPoint>().energy;

        }
    }
}
