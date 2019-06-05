using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine.AdaptivePerformance;

namespace UnityEditor.AdaptivePerformance.Editor.Metadata
{
    /// <summary>
    /// Provides an interface for describing specific loader metadata. Package authors should implement
    /// this interface for each loader they provide in their package.
    /// </summary>
    public interface IAdaptivePerformanceLoaderMetadata
    {
        /// <summary>
        /// The user facing name for this loader. Will be used to populate the
        /// list in the Adaptive Performance Provider Management UI.
        /// </summary>
        string loaderName { get; }

        /// <summary>
        /// The full type name for this loader. This is used to allow management to find and
        /// create instances of supported loaders for your package.
        ///
        /// When your package is first installed, the Adaptive Performance Provider Management system will
        /// use this information to create instances of your loaders in Assets/Adaptive Performance/Loaders.
        /// </summary>
        string loaderType { get; }

        /// <summary>
        /// The full list of supported buildtargets for this loader. This allows the UI to only show the
        /// loaders appropriate for a specific build target.
        ///
        /// Returning an empty list or a list containing just <see cref="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.Unknown.html">BuildTargetGroup.Unknown</see>. will make this
        /// loader invisible in the ui.
        /// </summary>
        List<BuildTargetGroup> supportedBuildTargets { get; }
    }

    /// <summary>
    /// Top level package metadata interface. Create an instance of this interface to
    /// provide metadata information for your package.
    /// </summary>
    public interface IAdaptivePerformancePackageMetadata
    {
        /// <summary>
        /// User facing package name. Should be the same as the value for the
        /// displayName keyword in the package.json file.
        /// </summary>
        string packageName { get; }

        /// <summary>
        /// The package id used to track and install the package. Must be the same value
        /// as the name keyword in the package.json file, otherwise installation will
        /// not be possible.
        /// </summary>
        string packageId { get; }

        /// <summary>
        /// This is the full type name for the settings type for your package.
        ///
        /// When your package is first installed, the Adaptive Performance Provider Management system will
        /// use this information to create an instance of your settings in Assets/Adaptive Performance/Settings.
        /// </summary>
        string settingsType { get; }

        /// <summary>
        /// This is the full URL for the license for your package.
        ///
        /// The full URL is used to inform the user about your package license. Must point to the same
        /// license as in the package.json file to inform the user in the provider list as it is
        /// accepted by installing the provider.
        /// </summary>
        string licenseURL { get; }

        /// <summary>
        /// List of <see cref="IAdaptivePerformanceLoaderMetadata"/> instances describing the data about the loaders
        /// your package supports.
        /// </summary>
        List<IAdaptivePerformanceLoaderMetadata> loaderMetadata { get; }
    }


    /// <summary>
    /// Provide access to the metadata store. Currently only usable as a way to assign and remove loaders
    /// to/from an <see cref="AdaptivePerformanceManagerSettings"/> instance.
    /// </summary>
    [InitializeOnLoad]
    public class AdaptivePerformancePackageMetadataStore
    {
        private const string k_WaitingPackmanQuery = "Waiting Packman Query.";
        private const string k_RebuildCache = "Rebuilding Cache.";
        private const string k_InstallingPackage = "Installing Adaptive Performance Package.";
        private const string k_AssigningPackage = "Assigning Adaptive Performance Package.";
        private const string k_UninstallingPackage = "Uninstalling Adaptive Performance Package.";

        private static float k_TimeOutDelta = 30f;

        enum InstallationState
        {
            New,
            RebuildInstalledCache,
            StartInstallation,
            Installing,
            Assigning,
            Complete,
            Uninstalling,
            Error
        }

        [Serializable]
        struct LoaderAssignmentRequest
        {
            [SerializeField]
            public string packageId;
            [SerializeField]
            public string loaderType;
            [SerializeField]
            public BuildTargetGroup buildTargetGroup;
            [SerializeField]
            public bool rebuildRequestOnly;
            [SerializeField]
            public bool needsAddRequest;
            [SerializeField]
            public ListRequest packageListRequest;
            [SerializeField]
            public AddRequest packageAddRequest;
            [SerializeField]
#pragma warning disable CS0649
            public RemoveRequest packageRemoveRequest;
#pragma warning restore CS0649
            [SerializeField]
            public float timeOut;
            [SerializeField]
            public InstallationState installationState;
            [SerializeField]
            public string errorText;
        }

        [Serializable]
        struct LoaderAssignmentRequests
        {
            [SerializeField]
            public List<LoaderAssignmentRequest> activeRequests;
        }

        private static List<LoaderAssignmentRequest> m_AddRequests = new List<LoaderAssignmentRequest>();

        private static Dictionary<string, IAdaptivePerformancePackage> s_Packages = new Dictionary<string, IAdaptivePerformancePackage>();
        private static HashSet<string> s_InstalledPackages = new HashSet<string>();

        private static SearchRequest s_SearchRequest = null;
        private static HashSet<string> s_InstallablePackages = new HashSet<string>();

        internal static bool isCheckingInstallationRequirements => EditorPrefs.HasKey(k_WaitingPackmanQuery);
        internal static bool isRebuildingCache => EditorPrefs.HasKey(k_RebuildCache);
        internal static bool isInstallingPackages => EditorPrefs.HasKey(k_InstallingPackage);
        internal static bool isUninstallingPackages => EditorPrefs.HasKey(k_UninstallingPackage);
        internal static bool isAssigningLoaders => EditorPrefs.HasKey(k_AssigningPackage);

        internal static bool isDoingQueueProcessing =>
            isCheckingInstallationRequirements || isInstallingPackages || isUninstallingPackages || isAssigningLoaders;

        private static void UpdateInstallablePackages()
        {
            EditorApplication.update -= UpdateInstallablePackages;

            if (s_SearchRequest == null)
            {
                return;
            }

            if (!s_SearchRequest.IsCompleted)
            {
                EditorApplication.update += UpdateInstallablePackages;
                return;
            }

            s_InstallablePackages.Clear();

            foreach (var package in s_SearchRequest.Result)
            {
                s_InstallablePackages.Add(package.name);
            }

            s_SearchRequest = null;

            RebuildInstalledCache();
        }

        internal static void InitKnownPackages()
        {
            foreach (var knownPackage in AdaptivePerformanceKnownPackages.Packages)
            {
                AddPackage(knownPackage);
            }
        }

        static AdaptivePerformancePackageMetadataStore()
        {
            InitKnownPackages();

            EditorApplication.playModeStateChanged += PlayModeStateChanged;

            if (EditorApplication.isPlaying || EditorApplication.isPaused)
                return;

            s_SearchRequest = Client.SearchAll(false);

            EditorApplication.update += UpdateInstallablePackages;
            EditorApplication.update += WaitingOnSearchQuery;
            EditorApplication.update += MonitorPackageInstallation;
            EditorApplication.update += MonitorPackageUninstall;
            EditorApplication.update += AssignAnyRequestedLoadersUpdate;
            EditorApplication.update += RebuildCache;
        }

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
            // Transfer installed package list over to play mode so that we don't need to
            // rebuild the cache with an expensive Package Manager call.
            const string k_InstalledPackagesKey = "Adaptive Performance Management Installed Packages Cache";
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (EditorPrefs.HasKey(k_InstalledPackagesKey))
                        EditorPrefs.DeleteKey(k_InstalledPackagesKey);
                    StringBuilder sb = new StringBuilder();
                    foreach (string packageId in s_InstalledPackages)
                    {
                        sb.AppendFormat($"{packageId};");
                    }
                    if (sb.Length > 0)
                        EditorPrefs.SetString(k_InstalledPackagesKey, sb.ToString());
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    string installedPackages = "";
                    if (EditorPrefs.HasKey(k_InstalledPackagesKey))
                    {
                        installedPackages = EditorPrefs.GetString(k_InstalledPackagesKey);
                        EditorPrefs.DeleteKey(k_InstalledPackagesKey);

                        if (!String.IsNullOrEmpty(installedPackages))
                        {
                            s_InstalledPackages.Clear();
                            var packages = installedPackages.Split(new char[] {';'});
                            foreach (var package in packages)
                            {
                                s_InstalledPackages.Add(package);
                            }
                        }
                    }
                    break;
            }
        }

        internal static void AddPackage(IAdaptivePerformancePackage package)
        {
            s_Packages[package.metadata.packageId] = package;
        }

        private static void AddRequestToQueue(LoaderAssignmentRequest request, string queueName)
        {
            LoaderAssignmentRequests reqs;

            if (EditorPrefs.HasKey(queueName))
            {
                string fromJson = EditorPrefs.GetString(queueName);
                reqs = JsonUtility.FromJson<LoaderAssignmentRequests>(fromJson);
            }
            else
            {
                reqs = new LoaderAssignmentRequests();
                reqs.activeRequests = new List<LoaderAssignmentRequest>();
            }

            reqs.activeRequests.Add(request);
            string json = JsonUtility.ToJson(reqs);
            EditorPrefs.SetString(queueName, json);
        }

        private static void SetRequestsInQueue(LoaderAssignmentRequests reqs, string queueName)
        {
            string json = JsonUtility.ToJson(reqs);
            EditorPrefs.SetString(queueName, json);
        }

        private static LoaderAssignmentRequests GetAllRequestsInQueue(string queueName)
        {
            LoaderAssignmentRequests reqs = new LoaderAssignmentRequests();
            reqs.activeRequests = new List<LoaderAssignmentRequest>();

            if (EditorPrefs.HasKey(queueName))
            {
                string fromJson = EditorPrefs.GetString(queueName);
                reqs = JsonUtility.FromJson<LoaderAssignmentRequests>(fromJson);
                EditorPrefs.DeleteKey(queueName);
            }

            return reqs;
        }

        private static void AssignAnyRequestedLoadersUpdate()
        {
            EditorApplication.update -= AssignAnyRequestedLoadersUpdate;

            LoaderAssignmentRequests reqs = GetAllRequestsInQueue(k_AssigningPackage);

            if (reqs.activeRequests == null || reqs.activeRequests.Count == 0)
                return;

            while (reqs.activeRequests.Count > 0)
            {
                var req = reqs.activeRequests[0];
                reqs.activeRequests.RemoveAt(0);

                var settings = AdaptivePerformanceGeneralSettingsPerBuildTarget.AdaptivePerformanceGeneralSettingsForBuildTarget(req.buildTargetGroup);

                if (settings == null)
                    continue;

                if (settings.AssignedSettings == null)
                {
                    var assignedSettings = ScriptableObject.CreateInstance<AdaptivePerformanceManagerSettings>() as AdaptivePerformanceManagerSettings;
                    settings.AssignedSettings = assignedSettings;
                    EditorUtility.SetDirty(settings);
                }

                if (AdaptivePerformancePackageMetadataStore.AssignLoader(settings.AssignedSettings, req.loaderType, req.buildTargetGroup))
                {
                    Debug.Log($"Assigned loader {req.loaderType} for build target {req.buildTargetGroup}");
                }
                else
                {
                    req.installationState = InstallationState.Error;
                    req.errorText = $"Unable to assign {req.packageId} for build target {req.buildTargetGroup}.";
                    QueueLoaderRequest(req);
                }
            }

            AdaptivePerformanceSettingsManager.Instance.ResetUi = true;
        }

        internal static void AssignAnyRequestedLoaders()
        {
            EditorApplication.update += AssignAnyRequestedLoadersUpdate;
        }

        internal struct LoaderBuildTargetQueryResult
        {
            public string packageName;
            public string packageId;
            public string loaderName;
            public string loaderType;
            public string licenseURL;
        }


        internal static List<LoaderBuildTargetQueryResult> GetAllLoadersForBuildTarget(BuildTargetGroup buildTarget)
        {
            var ret = from pm in (from p in s_Packages.Values select p.metadata)
                from lm in pm.loaderMetadata
                where lm.supportedBuildTargets.Contains(buildTarget)
                orderby lm.loaderName
                select new LoaderBuildTargetQueryResult() { packageName = pm.packageName, packageId = pm.packageId, loaderName = lm.loaderName, loaderType = lm.loaderType };
            var retList = ret.Distinct().ToList<LoaderBuildTargetQueryResult>();
            return retList;
        }

        internal static List<LoaderBuildTargetQueryResult> GetLoadersForBuildTarget(BuildTargetGroup buildTargetGroup)
        {
            var ret = from pm in (from p in s_Packages.Values select p.metadata)
                from lm in pm.loaderMetadata
                where lm.supportedBuildTargets.Contains(buildTargetGroup)
                orderby lm.loaderName
                select new LoaderBuildTargetQueryResult() { packageName = pm.packageName, packageId = pm.packageId, loaderName = lm.loaderName, loaderType = lm.loaderType, licenseURL = pm.licenseURL };
            var retList = ret.ToList<LoaderBuildTargetQueryResult>();
            return retList;
        }

        internal static IAdaptivePerformancePackageMetadata GetMetadataForPackage(string packageId)
        {
            IAdaptivePerformancePackageMetadata ret = null;
            var query = s_Packages.Values
                .Select(x => x.metadata)
                .Where(x => String.Compare(x.packageId, packageId) == 0);

            if (query.Any())
            {
                ret = query.First();
            }

            return ret;
        }

        internal static void RebuildInstalledCache()
        {
            if (isRebuildingCache)
                return;

            LoaderAssignmentRequest req = new LoaderAssignmentRequest();
            req.installationState = InstallationState.RebuildInstalledCache;
            req.rebuildRequestOnly = true;
            QueueLoaderRequest(req);
        }

        private static void RebuildCache()
        {
            EditorApplication.update -= RebuildCache;

            if (EditorApplication.isPlaying && EditorApplication.isPaused)
                return; // Use the cached data that should have been passed in the play state change.

            LoaderAssignmentRequests reqs = GetAllRequestsInQueue(k_RebuildCache);

            if (reqs.activeRequests == null || reqs.activeRequests.Count == 0)
                return;

            var req = reqs.activeRequests[0];

            if (!req.rebuildRequestOnly && IsPackageInstalled(req.packageId))
            {
                reqs.activeRequests.Remove(req);
                req.installationState = InstallationState.Assigning;
                QueueLoaderRequest(req);
            }
            else if (req.packageListRequest.IsCompleted)
            {
                reqs.activeRequests.Remove(req);

                if (req.packageListRequest.Status == StatusCode.Success)
                {
                    s_InstalledPackages.Clear();

                    List<string> installedPackages = new List<string>();

                    foreach (var packageInfo in req.packageListRequest.Result)
                    {
                        installedPackages.Add(packageInfo.name);
                    }

                    foreach (var p in s_Packages.Values)
                    {
                        if (installedPackages.Contains(p.metadata.packageId))
                        {
                            s_InstalledPackages.Add(p.metadata.packageId);
                        }
                    }

                    if (!req.rebuildRequestOnly)
                    {
                        if (IsPackageInstalled(req.packageId))
                        {
                            req.installationState = InstallationState.Assigning;
                        }
                        else
                        {
                            req.installationState = InstallationState.StartInstallation;
                        }
                        QueueLoaderRequest(req);
                    }
                }
                else
                {
                    req.errorText = $"Error installing package {req.packageId}. Error Code: {req.packageListRequest.Status} Error Message: {req.packageListRequest.Error.message}";
                    req.installationState = InstallationState.Error;
                    QueueLoaderRequest(req);
                }
            }

            if (reqs.activeRequests.Count > 0)
            {
                SetRequestsInQueue(reqs, k_RebuildCache);
                EditorApplication.update += RebuildCache;
            }
        }

        internal static bool HasInstallablePackageData()
        {
            return s_InstallablePackages.Any();
        }

        internal static bool IsPackageInstalled(string package)
        {
            return s_InstalledPackages.Contains(package) && File.Exists($"Packages/{package}/package.json");
        }

        internal static bool IsPackageInstallable(string package)
        {
            return s_InstallablePackages.Contains(package);
        }

        internal static bool IsLoaderAssigned(string loaderTypeName, BuildTargetGroup buildTargetGroup)
        {
            var settings = AdaptivePerformanceGeneralSettingsPerBuildTarget.AdaptivePerformanceGeneralSettingsForBuildTarget(buildTargetGroup);
            if (settings == null)
                return false;

            foreach (var loader in settings.AssignedSettings.loaders)
            {
                if (loader != null && String.Compare(loader.GetType().FullName, loaderTypeName) == 0)
                    return true;
            }
            return false;
        }

        private static void MonitorPackageInstallation()
        {
            EditorApplication.update -= MonitorPackageInstallation;
            LoaderAssignmentRequests reqs = GetAllRequestsInQueue(k_InstallingPackage);

            if (reqs.activeRequests.Count > 0)
            {
                var request = reqs.activeRequests[0];
                reqs.activeRequests.RemoveAt(0);

                if (request.needsAddRequest)
                {
                    request.packageAddRequest = Client.Add(request.packageId);
                    request.needsAddRequest = false;
                    request.installationState = InstallationState.Installing;
                    QueueLoaderRequest(request);
                }
                else if (request.packageAddRequest.IsCompleted && File.Exists($"Packages/{request.packageId}/package.json"))
                {
                    if (request.packageAddRequest.Status == StatusCode.Success)
                    {
                        if (!String.IsNullOrEmpty(request.loaderType))
                        {
                            request.packageAddRequest = null;
                            request.installationState = InstallationState.Assigning;
                            QueueLoaderRequest(request);
                        }
                    }
                    else
                    {
                        request.errorText = $"Error installing package {request.packageId}. Error Code: {request.packageAddRequest.Status} Error Message: {request.packageAddRequest.Error.message}";
                        request.installationState = InstallationState.Error;
                        QueueLoaderRequest(request);
                    }
                }
                else if (request.timeOut < Time.realtimeSinceStartup)
                {
                    request.errorText = $"Error installing package {request.packageId}. Package installation timed out. Check Package Manager UI to see if the package is installed and/or retry your operation.";

                    if (request.packageAddRequest.IsCompleted)
                    {
                        request.errorText += $" Error message: {request.packageAddRequest.Error.message}";
                    }

                    request.installationState = InstallationState.Error;
                    QueueLoaderRequest(request);
                }
                else
                {
                    QueueLoaderRequest(request);
                }
            }
        }

        private static void WaitingOnSearchQuery()
        {
            EditorApplication.update -= WaitingOnSearchQuery;
            if (s_SearchRequest != null)
            {
                EditorApplication.update += WaitingOnSearchQuery;
                return;
            }

            LoaderAssignmentRequests reqs = GetAllRequestsInQueue(k_WaitingPackmanQuery);
            if (reqs.activeRequests.Count > 0)
            {
                for (int i = 0; i < reqs.activeRequests.Count; i++)
                {
                    var req = reqs.activeRequests[i];
                    req.installationState = InstallationState.RebuildInstalledCache;
                    QueueLoaderRequest(req);
                }
            }
        }

        private static void MonitorPackageUninstall()
        {
            EditorApplication.update -= MonitorPackageUninstall;
            LoaderAssignmentRequests reqs = GetAllRequestsInQueue(k_UninstallingPackage);
            if (reqs.activeRequests.Count > 0)
            {
                for (int i = 0; i < reqs.activeRequests.Count; i++)
                {
                    var req = reqs.activeRequests[i];
                    if (!req.packageRemoveRequest.IsCompleted)
                        QueueLoaderRequest(req);

                    if (req.packageRemoveRequest.Status == StatusCode.Failure)
                    {
                        req.installationState = InstallationState.Error;
                        req.errorText = req.packageRemoveRequest.Error.message;
                        QueueLoaderRequest(req);
                    }
                }
            }
        }

        private static void QueueLoaderRequest(LoaderAssignmentRequest req)
        {
            switch (req.installationState)
            {
                case InstallationState.New:
                    if (!HasInstallablePackageData() && s_SearchRequest == null)
                    {
                        s_SearchRequest = Client.SearchAll(false);
                        EditorApplication.update += UpdateInstallablePackages;
                    }
                    AddRequestToQueue(req, k_WaitingPackmanQuery);
                    EditorApplication.update += WaitingOnSearchQuery;
                    break;

                case InstallationState.RebuildInstalledCache:
                    req.packageListRequest = Client.List(true, false);
                    AddRequestToQueue(req, k_RebuildCache);
                    EditorApplication.update += RebuildCache;
                    break;

                case InstallationState.StartInstallation:
                    req.needsAddRequest = true;
                    req.packageAddRequest = null;
                    req.timeOut = Time.realtimeSinceStartup + k_TimeOutDelta;
                    AddRequestToQueue(req, k_InstallingPackage);
                    EditorApplication.update += MonitorPackageInstallation;
                    break;

                case InstallationState.Installing:
                    AddRequestToQueue(req, k_InstallingPackage);
                    EditorApplication.update += MonitorPackageInstallation;
                    break;

                case InstallationState.Assigning:
                    AddRequestToQueue(req, k_AssigningPackage);
                    EditorApplication.update += AssignAnyRequestedLoadersUpdate;
                    break;

                case InstallationState.Uninstalling:
                    AddRequestToQueue(req, k_UninstallingPackage);
                    EditorApplication.update += MonitorPackageUninstall;
                    break;

                case InstallationState.Error:
                    Debug.LogError($"Could not install or assign any package with id {req.packageId}. Check if there are any other errors in the console and make sure they are corrected before trying again.\n Failure reason: {req.errorText}");
                    AdaptivePerformanceSettingsManager.Instance.ResetUi = true;
                    break;
            }
        }

        internal static void InstallPackageAndAssignLoaderForBuildTarget(string package, string loaderType, BuildTargetGroup buildTargetGroup)
        {
            LoaderAssignmentRequest req = new LoaderAssignmentRequest();
            req.packageId = package;
            req.loaderType = loaderType;
            req.buildTargetGroup = buildTargetGroup;
            req.installationState = InstallationState.New;
            QueueLoaderRequest(req);
        }

        internal static bool IsLoaderAssigned(AdaptivePerformanceManagerSettings settings, string loaderTypeName)
        {
            if (settings == null)
                return false;

            bool wasFound = false;
            foreach (var l in settings.loaders)
            {
                if (l != null && String.Compare(l.GetType().FullName, loaderTypeName) == 0)
                    wasFound = true;
            }
            return wasFound;
        }

        /// <summary>
        /// Assigns a loader of type loaderTypeName to the settings instance. Will instantiate an
        /// instance if one can't be found in the users project folder before assigning it.
        /// </summary>
        /// <param name="settings">An instance of <see cref="AdaptivePerformanceManagerSettings"/> to add the loader to.</param>
        /// <param name="loaderTypeName">The full type name for the loader instance to assign to settings.</param>
        /// <param name="buildTargetGroup">The build target group being assigned to.</param>
        /// <returns>True if assignment succeeds, false if not.</returns>
        public static bool AssignLoader(AdaptivePerformanceManagerSettings settings, string loaderTypeName, BuildTargetGroup buildTargetGroup)
        {
            var instance = EditorUtilities.GetInstanceOfTypeWithNameFromAssetDatabase(loaderTypeName);
            if (instance == null || !(instance is AdaptivePerformanceLoader))
            {
                instance  = EditorUtilities.CreateScriptableObjectInstance(loaderTypeName,
                    EditorUtilities.GetAssetPathForComponents(EditorUtilities.s_DefaultLoaderPath));
                if (instance == null)
                    return false;
            }

            var assignedLoaders = settings.loaders;
            AdaptivePerformanceLoader newLoader = instance as AdaptivePerformanceLoader;

            if (!assignedLoaders.Contains(newLoader))
            {
                assignedLoaders.Add(newLoader);
                settings.loaders = new List<AdaptivePerformanceLoader>();

                var allLoaders = GetAllLoadersForBuildTarget(buildTargetGroup);

                foreach (var ldr in allLoaders)
                {
                    var newInstance = EditorUtilities.GetInstanceOfTypeWithNameFromAssetDatabase(ldr.loaderType) as AdaptivePerformanceLoader;

                    if (newInstance != null && assignedLoaders.Contains(newInstance))
                    {
                        settings.loaders.Add(newInstance);
#if UNITY_EDITOR
                        var loaderHelper = newLoader as AdaptivePerformanceLoaderHelper;
                        loaderHelper?.WasAssignedToBuildTarget(buildTargetGroup);
#endif
                    }
                }

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }

            return true;
        }

        /// <summary>
        /// Remove a previously assigned loader from settings. If the loader type is unknown or
        /// an instance of the loader can't be found in the project folder no action is taken.
        ///
        /// Removal will not delete the instance from the project folder.
        /// </summary>
        /// <param name="settings">An instance of <see cref="AdaptivePerformanceManagerSettings"/> to add the loader to.</param>
        /// <param name="loaderTypeName">The full type name for the loader instance to remove from settings.</param>
        /// <param name="buildTargetGroup">The build target group being removed from.</param>
        /// <returns>True if removal succeeds, false if not.</returns>
        public static bool RemoveLoader(AdaptivePerformanceManagerSettings settings, string loaderTypeName, BuildTargetGroup buildTargetGroup)
        {
            var instance = EditorUtilities.GetInstanceOfTypeWithNameFromAssetDatabase(loaderTypeName);
            if (instance == null || !(instance is AdaptivePerformanceLoader))
                return false;

            AdaptivePerformanceLoader loader = instance as AdaptivePerformanceLoader;

            if (settings.loaders.Contains(loader))
            {
                settings.loaders.Remove(loader);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
#if UNITY_EDITOR
                var loaderHelper = loader as AdaptivePerformanceLoaderHelper;
                loaderHelper?.WasUnassignedFromBuildTarget(buildTargetGroup);
#endif
            }

            return true;
        }

        internal static IAdaptivePerformancePackage GetPackageForSettingsTypeNamed(string settingsTypeName)
        {
            var ret = from p in s_Packages.Values
                where String.Compare(p.metadata.settingsType, settingsTypeName, true) == 0
                select p;
            return ret.Any() ? ret.First() : null;
        }

        internal static void ReportProgressOnActiveWork()
        {
            if (AdaptivePerformancePackageMetadataStore.isCheckingInstallationRequirements)
            {
                EditorUtility.DisplayProgressBar("Adaptive Performance Management", "Checking installation requirements for packages...", 0.2f);
            }
            else if (AdaptivePerformancePackageMetadataStore.isRebuildingCache)
            {
                EditorUtility.DisplayProgressBar("Adaptive Performance Management", "Rebuilding package cache...", 0.4f);
            }
            else if (AdaptivePerformancePackageMetadataStore.isInstallingPackages)
            {
                EditorUtility.DisplayProgressBar("Adaptive Performance Management", "Installing packages...", 0.5f);
            }
            else if (AdaptivePerformancePackageMetadataStore.isUninstallingPackages)
            {
                EditorUtility.DisplayProgressBar("Adaptive Performance Management", "Uninstalling packages...", 0.5f);
            }
            else if (AdaptivePerformancePackageMetadataStore.isAssigningLoaders)
            {
                EditorUtility.DisplayProgressBar("Adaptive Performance Management", "Assigning all requested loaders...", 0.8f);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
