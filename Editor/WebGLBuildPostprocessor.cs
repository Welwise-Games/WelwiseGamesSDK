using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using WelwiseGames.Editor.SDK;
using WelwiseGamesSDK.Shared;

namespace WelwiseGames.Editor
{
    public class WebGLBuildPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            var buildFolder = report.summary.outputPath;
            var indexHtmlPath = Path.Combine(buildFolder, "index.html");

            if (!File.Exists(indexHtmlPath))
                return;

            var settings = SDKSettings.LoadOrCreateSettings();
            var html = File.ReadAllText(indexHtmlPath);

            // Получаем определение выбранного SDK
            var sdkDefinition = SDKProvider.GetSDKDefinition(settings.SelectedSDK);
            if (sdkDefinition != null)
            {
                html = InjectSDKScripts(html, settings.SelectedSDK, sdkDefinition, settings.SDKConfig);
            }

            // Обновляем настройки аспекта и фона
            html = UpdateTemplateSettings(html, settings);

            File.WriteAllText(indexHtmlPath, html);
        }

        private string InjectSDKScripts(string html, string sdkName, SDKDefinition sdkDefinition, Dictionary<string, object> config)
        {
            foreach (var script in sdkDefinition.PostBuildScripts)
            {
                var injectionCode = SDKProvider.GetInjectionCode(script);
                if (string.IsNullOrEmpty(injectionCode))
                {
                    Debug.LogWarning($"Injection code not found for SDK {sdkName} at point {script.InjectPoint}");
                    continue;
                }

                var processedCode = ProcessPlaceholders(injectionCode, config);
                
                switch (script.InjectPoint.ToLower())
                {
                    case "head":
                        html = html.Replace("<head>", "<head>\n" + processedCode);
                        break;
                    case "before_body_end":
                        html = html.Replace("</body>", processedCode + "\n</body>");
                        break;
                    case "after_body_start":
                        html = html.Replace("<body>", "<body>\n" + processedCode);
                        break;
                    default:
                        Debug.LogWarning($"Unknown inject point: {script.InjectPoint}");
                        break;
                }
            }

            return html;
        }

        private string ProcessPlaceholders(string code, Dictionary<string, object> config)
        {
            return Regex.Replace(code, @"\{(\w+)\}", match =>
            {
                var key = match.Groups[1].Value;
                return config.TryGetValue(key, out var value) ? value.ToString() : match.Value;
            });
        }

        private string UpdateTemplateSettings(string html, SDKSettings settings)
        {
            var backgroundImagePath = settings.AspectRatio != SDKSettings.AspectRatioMode.Default &&
                                      !string.IsNullOrEmpty(settings.BackgroundImagePath)
                ? settings.BackgroundImagePath
                : "background.jpg";

            var aspectRatioValue = settings.AspectRatio switch
            {
                SDKSettings.AspectRatioMode.Aspect16_9 => "'16_9'",
                SDKSettings.AspectRatioMode.Aspect9_16 => "'9_16'",
                _ => "'default'"
            };

            html = html.Replace(
                "aspectRatioMode: 'default',",
                $"aspectRatioMode: {aspectRatioValue},");

            html = html.Replace(
                "backgroundImageUrl: 'background.jpg'",
                $"backgroundImageUrl: '{backgroundImagePath}'");

            html = html.Replace(
                "useThreeJsLoader: true",
                $"useThreeJsLoader: {settings.UseThreeJsLoader.ToString().ToLower()}");

            return html;
        }
    }
}