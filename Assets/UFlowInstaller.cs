using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Windows;
using Debug = UnityEngine.Debug;

namespace UFlow.Installer {
    internal static class UFlowInstaller {
        private const string c_url = "https://github.com/Danon5/uflow.git";
        private const string c_installer_path = "Assets/UFlowInstaller";
        private const string c_path_local = "UFlow";
        private const string c_work_tree_cmd = "rev-parse --is-inside-work-tree";
        private static readonly string s_path = $"{Application.dataPath}/UFlow";
        private static readonly string s_submoduleCmd = $"submodule add {c_url} \"{c_path_local}/\"";
        private static readonly string s_cloneCmd = $"clone --recurse-submodules -j8 {c_url} \"{s_path}\"";
        private static readonly List<InstallRequest> s_installRequests = new();
        private static InstallRequest s_currentRequest;

        [MenuItem("Assets/Install UFlow", false, 0)]
        private static void Install() {
            var isSubmodule = IsRootPathAGitRepository();

            ProcessStartInfo startInfo;

            if (isSubmodule) {
                startInfo = new ProcessStartInfo("git", s_submoduleCmd)
                {
                    WorkingDirectory = Application.dataPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }
            else {
                startInfo = new ProcessStartInfo("git", s_cloneCmd)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }

            var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (_, args) => {
                if (args.Data == null) return;
                Debug.Log(args.Data);
            };
            process.ErrorDataReceived += (_, args) => {
                if (args.Data == null) return;
                Debug.Log(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (Directory.Exists(c_installer_path))
                AssetDatabase.DeleteAsset(c_installer_path);
            
            EnqueuePackageInstall("com.unity.addressables");
            EnqueuePackageInstall("com.unity.mathematics");
            EnqueuePackageInstall("com.unity.collections");
            ProcessInstalls();

            AssetDatabase.Refresh();
        }
        
        private static bool IsRootPathAGitRepository() {
            var startInfo = new ProcessStartInfo("git", c_work_tree_cmd) {
                WorkingDirectory = Application.dataPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var outputLine = string.Empty;

            var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (_, args) => {
                if (args.Data == null) return;
                outputLine = args.Data;
            };
            process.ErrorDataReceived += (_, args) => {
                if (args.Data == null) return;
                outputLine = args.Data;
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            
            return outputLine.Equals("true");
        }
        
        private static void EnqueuePackageInstall(in string packageName)
        {
            s_installRequests.Add(new InstallRequest {
                name = packageName,
                request = null
            });
        }

        private static void ProcessInstalls() {
            if (s_installRequests.Count == 0) return;
            StartPackageInstall(s_installRequests[0]);
        }

        private static void StartPackageInstall(in InstallRequest request) {
            EditorApplication.update += OnInstallProgress;
            s_currentRequest = request;
            s_currentRequest.request = Client.Add(request.name);
        }
        
        private static void OnInstallProgress() {
            if (!s_currentRequest.request.IsCompleted) return;
            switch (s_currentRequest.request.Status) {
                case StatusCode.Success:
                    Debug.Log("Installed dependency: " + s_currentRequest.request.Result.packageId);
                    break;
                case >= StatusCode.Failure:
                    Debug.Log(s_currentRequest.request.Error.message);
                    break;
            }
            
            EditorApplication.update -= OnInstallProgress;
            s_installRequests.Remove(s_currentRequest);

            if (s_installRequests.Count > 0)
                StartPackageInstall(s_installRequests[0]);
        }

        private sealed class InstallRequest {
            public string name;
            public AddRequest request;
        }
    }
}
