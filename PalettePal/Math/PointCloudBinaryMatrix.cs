using System.Collections.Generic;

namespace PalettePal
{
    sealed class PointCloudBinaryMatrix : List<BinaryPoint>
    {
        public bool this[Vector3i location]
        {
            get
            {
                var polarity = 0;
                var shortestDistance = int.MaxValue;

                foreach (var point in this)
                {
                    var distance = Vector3i.GetDistanceSquared(location, point.Location);
                    if (distance > shortestDistance) continue;
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        polarity = 0;
                    }
                    polarity += point.Polarity ? 1 : -1;
                }

                return polarity > 0;
            }
        }
    }
}
