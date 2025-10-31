using g4;
using System.Collections.Generic;

using UnityEngine;

namespace ARMeasurementApp.Scripts.Objects
{
    public class AABB
    {
        private AxisAlignedBox3d _axisAlignedBox4g;

        private List<Vector3> _boxCornerPositions = new List<Vector3>();

        public AABB(AxisAlignedBox3d box4g)
        {
            if (box4g == null) return;

            _axisAlignedBox4g = box4g;

            for (int i = 0; i < 8; i++)
            {
                _boxCornerPositions.Add((Vector3)_axisAlignedBox4g.Corner(i));
            }
        }

        public List<Vector3> GetCornerPositions() 
        {
            if (_axisAlignedBox4g == null) return default;

            return _boxCornerPositions;
        }

        public double GetBoxWidth()
        {
            if (_axisAlignedBox4g == null) return default;

            return _axisAlignedBox4g.Width;
        }

        public double GetBoxHeight()
        {
            if (_axisAlignedBox4g == null) return default;

            return _axisAlignedBox4g.Height;
        }

        public double GetBoxDepth()
        {
            if (_axisAlignedBox4g == null) return default;

            return _axisAlignedBox4g.Depth;
        }

        public Vector3 GetBoxExtents()
        {
            if (_axisAlignedBox4g == null) return default;

            return (Vector3)_axisAlignedBox4g.Extents;
        }

        public Vector3 GetBoxCenter()
        {
            if (_axisAlignedBox4g == null) return default;

            return (Vector3)_axisAlignedBox4g.Center;
        }

        public List<(double Distance, Vector3 SourcePosition, Vector3 DestinationPosition)> GetBoxDimensionInfo()
        {
            if (_axisAlignedBox4g == null) return default;

            var width = GetBoxWidth();
            var depth = GetBoxDepth();
            var height = GetBoxHeight();

            List<(double Distance, Vector3 SourcePosition, Vector3 DestinationPosition)> dimensionInfo = new List<(double Distance, Vector3 SourcePosition, Vector3 DestinationPosition)>();

            // top and bottom width/depth
            dimensionInfo.Add((width, _boxCornerPositions[0], _boxCornerPositions[1]));
            dimensionInfo.Add((depth, _boxCornerPositions[1], _boxCornerPositions[5]));
            dimensionInfo.Add((width, _boxCornerPositions[4], _boxCornerPositions[5]));
            dimensionInfo.Add((depth, _boxCornerPositions[4], _boxCornerPositions[0]));

            dimensionInfo.Add((width, _boxCornerPositions[3], _boxCornerPositions[2]));
            dimensionInfo.Add((depth, _boxCornerPositions[2], _boxCornerPositions[6]));
            dimensionInfo.Add((width, _boxCornerPositions[7], _boxCornerPositions[6]));
            dimensionInfo.Add((depth, _boxCornerPositions[7], _boxCornerPositions[3]));

            //Height Dimensions
            dimensionInfo.Add((height, _boxCornerPositions[3], _boxCornerPositions[0]));
            dimensionInfo.Add((height, _boxCornerPositions[2], _boxCornerPositions[1]));
            dimensionInfo.Add((height, _boxCornerPositions[6], _boxCornerPositions[5]));
            dimensionInfo.Add((height, _boxCornerPositions[7], _boxCornerPositions[4]));

            return dimensionInfo;
        }
    }
}

