using Project.Scripts.Runtime.Tracking.Provider;
using Project.Scripts.Runtime.Utils;
using UnityEngine;

namespace Project.Scripts.Runtime.Tracking.MonoBehaviour
{
    public class MainMono : UnityEngine.MonoBehaviour
    {
        public GameObject tracker;
        public BackgroundData lastFrameData = new BackgroundData();
     
        private BackgroundDataProvider _backgroundDataProvider;
    
        const int TRACKER_ID = 0;

        void Start()
        {
            _backgroundDataProvider = new SkeletalTrackingProvider().Apply(it => { it.StartClientThread(TRACKER_ID); });
        }

    void Update()
    {
        if (!_backgroundDataProvider.IsRunning) return;
        if (!_backgroundDataProvider.GetCurrentFrameData(ref lastFrameData)) return;

        if (lastFrameData.NumOfBodies != 0)
        {
            tracker.GetComponent<TrackerHandlerMono>().updateTracker(lastFrameData);
        }
    }

        void OnDestroy()
        {
            _backgroundDataProvider?.StopClientThread();
        }
    }
}
