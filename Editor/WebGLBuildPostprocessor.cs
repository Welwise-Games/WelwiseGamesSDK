using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
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

            var html = File.ReadAllText(indexHtmlPath);
                
            // Заменяем конфигурационные значения
            html = html.Replace(
                "aspectRatioMode: 'default',", 
                $"aspectRatioMode: {aspectRatioValue},");
        
            html = html.Replace(
                "backgroundImageUrl: 'background.jpg'", 
                $"backgroundImageUrl: '{backgroundImagePath}'");
    
            // Добавляем замену для ThreeJS опции
            html = html.Replace(
                "useThreeJsLoader: true", 
                $"useThreeJsLoader: {settings.UseThreeJsLoader.ToString().ToLower()}");
        
            File.WriteAllText(indexHtmlPath, html);
        }
    }
}