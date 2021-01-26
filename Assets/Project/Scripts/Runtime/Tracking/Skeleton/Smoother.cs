using System.Collections.Generic;

namespace Project.Scripts.Runtime.Tracking.Skeleton
{
    public class Smoother
    {
        private int maxSize = 100;
        private bool hasEnoughForSmoothing = false;
        public int NumberSmoothingFrames { get; set; } = 5;
        private List<SkeletonPosition> rawData = new List<SkeletonPosition>();
        private List<SkeletonPosition> smoothenedData = new List<SkeletonPosition>();
    
        public SkeletonPosition ReceiveNewSensorData(SkeletonPosition newData, bool smoothing)
        {
            if(rawData.Count > maxSize) Resize();
            rawData.Add(newData);
            if (NumberSmoothingFrames <= 1) return rawData[rawData.Count - 1];
            if (rawData.Count > NumberSmoothingFrames) hasEnoughForSmoothing = true;
            
            if (smoothenedData.Count == 0)
            {
                smoothenedData.Add(newData);
            }
            else
            {
                var temp = smoothenedData[smoothenedData.Count - 1] + newData;
                if(hasEnoughForSmoothing) temp -= rawData[rawData.Count - NumberSmoothingFrames];
                smoothenedData.Add(temp);
            }
            
            smoothenedData[smoothenedData.Count - 1].Timestamp = rawData[rawData.Count - 1].Timestamp;

            return smoothing && hasEnoughForSmoothing
                ? smoothenedData[smoothenedData.Count - 1] / (float)NumberSmoothingFrames
                : rawData[rawData.Count - 1];
        }
        
        public void Resize()
        {
            if (rawData.Count > NumberSmoothingFrames)
                rawData.RemoveRange(0, rawData.Count - NumberSmoothingFrames);
            
            if (smoothenedData.Count > NumberSmoothingFrames)
                smoothenedData.RemoveRange(0, smoothenedData.Count - NumberSmoothingFrames);
        }
    }
}
