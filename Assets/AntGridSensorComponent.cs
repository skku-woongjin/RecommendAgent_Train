using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A SensorComponent that creates a <see cref="GridSensorBase"/>.
    /// </summary>

    public class AntGridSensorComponent : GridSensorComponent
    {
        protected override GridSensorBase[] GetGridSensors()
        {
            List<GridSensorBase> sensorList = new List<GridSensorBase>();
            var sensor = new AntGridSensor(m_SensorName + "-OneHot", base.CellScale, base.GridSize, base.DetectableTags, base.CompressionType);
            sensorList.Add(sensor);
            return sensorList.ToArray();
        }
    }
}
