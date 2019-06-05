using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnityEditor.AdaptivePerformance.Editor
{
    internal class AdaptivePerformancePostProcess : IPreprocessBuildWithReport
    {
        static string s_ProviderPackageNotFound = L10n.Tr("No Adaptive Performance provider package installed. Adaptive Performance requires a provider to get information during runtime. Please install a provider such as, Adaptive Performance Samsung (Android), via the Adaptive Performance Settings.");
        static string s_Title = L10n.Tr("No Adaptive Performance provider found");
        static string s_Ok = L10n.Tr("Go to Settings");
        static string s_Cancel = L10n.Tr("Ignore");


        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckInstalledProvider();
        }

        static ListRequest Request;

        /// <summary>
        /// Requests a list of all installed packages from PackageManager which are processed in CheckInstalledPackages.
        /// </summary>
        static void CheckInstalledProvider()
        {
            Request = Client.List();    // List packages installed for the Project
            EditorApplication.update += CheckInstalledPackages;
        }

        /// <summary>
        /// Processes a list of all installed packages and notifies user via console if no Adaptive Performance Provider package is installed.
        /// </summary>
        static void CheckInstalledPackages()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    var installedPackageCount = 0;

                    foreach (var package in Request.Result)
                        if (package.name.StartsWith("com.unity.adaptiveperformance."))
                            installedPackageCount++;

                    if (installedPackageCount == 0)
                    {
                        if (EditorUtility.DisplayDialog(s_Title, s_ProviderPackageNotFound , s_Ok, s_Cancel))
                        {
                            PackageManager.UI.Window.Open("com.unity.adaptiveperformance.samsung.android");
                            SettingsService.OpenProjectSettings("Project/Adaptive Performance");
                        }
                        else
                        {
                            Debug.LogWarning(s_ProviderPackageNotFound);
                        }
                    }
                }
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= CheckInstalledPackages;
            }
        }
    }
}
