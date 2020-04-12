using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if F_C_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace Framework.IGroundPlane
{
    /// <summary>
    /// Dynamic loading requires to use UnityEngine.XR.ARFoundation 3.0.1 +
    /// Don't forget to add Upgrade the ARCore/ArKit Package to 3.0.1
    /// </summary>
    [DisallowMultipleComponent]
    public class AsyncLoadNewLibrary : MonoBehaviour
    {
        /// <summary>
        /// Someone should add images
        /// </summary>
        public static event Action<List<DynamicImageLibraryEntry>> OnRequestImageAdd;
        [SerializeField] private Transform trackedImageManager = null;

#if F_C_AR_FOUNDATION
        private ARTrackedImageManager mARTrackedImageManager = null;
#endif

        private void Awake()
        {
#if F_C_AR_FOUNDATION
            mARTrackedImageManager = trackedImageManager.GetComponent<ARTrackedImageManager>();
            ARSession.stateChanged += AfterInitialization;
#endif
        }

#if F_C_AR_FOUNDATION
        private void AfterInitialization(ARSessionStateChangedEventArgs stateArgument)
        {
            if (stateArgument.state == ARSessionState.SessionInitializing)
            {
                // Ready to set Runtime Library
                mARTrackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();
                if (mARTrackedImageManager.descriptor.supportsMutableLibrary)
                {
                    var myRuntimeReferenceImageLibrary =
                        mARTrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;

                    var allTexturesHere = new List<DynamicImageLibraryEntry>(5);
                    OnRequestImageAdd?.Invoke(allTexturesHere);
                    var jobs = new List<Unity.Jobs.JobHandle>(allTexturesHere.Count);

                    foreach (var item in allTexturesHere)
                    {
                        jobs.Add(myRuntimeReferenceImageLibrary
                                .ScheduleAddImageJob(item.texture, item.id, item.widthInMeters));
                    }

                    foreach (var item in jobs)
                    {
                        item.Complete();
                    }

                    if (myRuntimeReferenceImageLibrary != null)
                    {
                        Debug.Log("myRuntimeReferenceImageLibrary: " + myRuntimeReferenceImageLibrary.count);
                        Debug.Log("supportedTextureFormatCount: " + myRuntimeReferenceImageLibrary.supportedTextureFormatCount);

                        mARTrackedImageManager.referenceLibrary = myRuntimeReferenceImageLibrary;
                    }
                    mARTrackedImageManager.enabled = true;
                }
            }
        }
#endif
    }
}
