﻿using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using WelwiseGamesSDK.Internal;
using WelwiseGamesSDK.Shared;

namespace WelwiseGames.Editor
{
    public class WebGLBuildPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.WebGL)
            {
                string buildFolder = report.summary.outputPath;
                string indexHtmlPath = Path.Combine(buildFolder, "index.html");
                
                if (!File.Exists(indexHtmlPath)) return;

                var settings = SDKSettings.LoadOrCreateSettings();
                
                string backgroundImagePath = null;
                if (settings.BackgroundImage != null && settings.AspectRatio != SDKSettings.AspectRatioMode.Default)
                {
                    string sourcePath = AssetDatabase.GetAssetPath(settings.BackgroundImage);
                    string fileName = Path.GetFileName(sourcePath);
                    string destPath = Path.Combine(buildFolder, fileName);
                    
                    File.Copy(sourcePath, destPath, true);
                    backgroundImagePath = fileName;
                }

                // Обновляем index.html
                string html = File.ReadAllText(indexHtmlPath);
                
                // Заменяем константу aspectRatioMode
                string aspectRatioValue = settings.AspectRatio switch
                {
                    SDKSettings.AspectRatioMode.Aspect16_9 => "'16_9'",
                    SDKSettings.AspectRatioMode.Aspect9_16 => "'9_16'",
                    _ => "'default'"
                };
                
                html = html.Replace(
                    "const aspectRatioMode = 'default';", 
                    $"const aspectRatioMode = {aspectRatioValue};");
                
                if (backgroundImagePath != null)
                {
                    html = html.Replace(
                        "let backgroundImageUrl = 'background.jpg';", 
                        $"let backgroundImageUrl = '{backgroundImagePath}';");
                }
                
                File.WriteAllText(indexHtmlPath, html);
            }
        }
    }
}