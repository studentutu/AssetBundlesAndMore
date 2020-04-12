using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if F_C_VUFORIA
using Vuforia;
using System.Linq;
#endif

namespace Framework.IGroundPlane
{
    public class NamedDataSet
    {
#if F_C_VUFORIA

        public DataSet set = null;
        public string name = null;
#endif
    }

    public class LoadDataset : MonoBehaviour
    {
        [SerializeField] private DislocatedImiteraImageTarget prefab = null;

#if F_C_VUFORIA
        //This function will de-activate all current datasets and activate the designated dataset.
        //It is assumed the dataset being activated has already been loaded (either through code
        //elsewhere or via the Vuforia Configuration).
        public void DisableAllAndSet(NamedDataSet setAsActive)
        {
            ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            IEnumerable<DataSet> activeDataSets = objectTracker.GetActiveDataSets();
            List<DataSet> activeDataSetsToBeRemoved = activeDataSets.ToList();

            //Loop through all the active datasets and deactivate them.
            foreach (DataSet ads in activeDataSetsToBeRemoved)
            {
                objectTracker.DeactivateDataSet(ads);
            }

            if (setAsActive == null)
            {
                return;
            }
            //Swapping of the datasets should not be done while the ObjectTracker is working at the same time.
            //So, Stop the tracker first.
            objectTracker.Stop();

            objectTracker.ActivateDataSet(setAsActive.set);

            //Finally, start the object tracker.
            objectTracker.Start();
        }
#endif

        //This function will load and activate the designated dataset. It will not de-activate
        //anything, so be sure no other Model Target datasets are active to avoid issues.
        public void ActivateTarget(DataSet setToActivate, bool startObjectTracking)
        {
#if F_C_VUFORIA

            TrackerManager trackerManager = (TrackerManager)TrackerManager.Instance;
            ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            //Stop the tracker.
            objectTracker.Stop();

            //Load and activate the dataset
            objectTracker.ActivateDataSet(setToActivate);

            //Start the object tracker.
            if (startObjectTracking)
            {
                objectTracker.Start();
            }
#endif
        }

#if F_C_VUFORIA
        /// <summary>
        /// Creates new DataSet
        /// </summary>
        /// <returns></returns>
        public NamedDataSet CreateNewDataSet(string Id)
        {
            var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            return new NamedDataSet { name = Id, set = objectTracker.CreateDataSet() };
        }
#endif

#if F_C_VUFORIA
        /// <summary>
        /// Requires Vuforia 8.6 +
        /// See https://library.vuforia.com/content/vuforia-library/en/articles/Solution/vuforia-engine-package-hosting-for-unity.html
        /// </summary>
        /// <param name="setToUse"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        private bool CreateImageTargetFromSideloadedTexture(DataSet setToUse, DynamicImageLibraryEntry entry)
        {
            if (setToUse.HasReachedTrackableLimit())
            {
                return false;
            }
            var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            // get the runtime image source and set the texture
            var runtimeImageSource = objectTracker.RuntimeImageSource;
            float width = entry.widthInMeters == null ? 0.15f : entry.widthInMeters.Value;
            runtimeImageSource.SetImage(entry.texture, width, entry.id);

            var trackableBehaviour = setToUse.CreateTrackable(runtimeImageSource, entry.id);
            //  use the source to create a new trackable
            var prev = prefab.enabled;
            prefab.enabled = false;
            var prefabInstance = GameObject.Instantiate(prefab, trackableBehaviour.transform, true);
            prefabInstance.transform.localScale = Vector3.one;
            prefab.enabled = prev;

            // add the DefaultTrackableEventHandler to the newly created game object
            trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();

            DislocatedImiteraImageTarget.AddNewImageTarget(entry, prefabInstance);
            return true;
        }
#endif
    }
}