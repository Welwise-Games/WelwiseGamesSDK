using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGames.Editor
{
    public class WebGLBuildPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        // WebGLBuildPostprocessor.cs
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

            // Добавляем код GameDistribution если выбран соответствующий SDK
            if (settings.SDKType == SupportedSDKType.GameDistribution &&
                !string.IsNullOrEmpty(settings.GameDistributionId))
            {
                var gdScript = GenerateGameDistributionScript(settings.GameDistributionId);
                html = InsertGameDistributionScript(html, gdScript);
            }

            // Существующие замены для aspect ratio и background
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

            File.WriteAllText(indexHtmlPath, html);
        }

       // WebGLBuildPostprocessor.cs - обновляем метод GenerateGameDistributionScript
private string GenerateGameDistributionScript(string gameId)
{
    return $@"
    <!-- GameDistribution SDK -->
    <script>
        // Глобальные переменные для GameDistribution
        window.gdSdkReady = false;
        window.gdSdkInitPromise = null;
        window.gdSdkInitResolve = null;
        window.gdSdkInitReject = null;
        window.gdCurrentAdType = null;
        window.gdInterstitialCallbacks = null;
        window.gdRewardedCallbacks = null;

        // Создаем Promise для инициализации
        window.gdSdkInitPromise = new Promise((resolve, reject) => {{
            window.gdSdkInitResolve = resolve;
            window.gdSdkInitReject = reject;
        }});

        window['GD_OPTIONS'] = {{
            'gameId': '{gameId}',
            'onEvent': function(event) {{
                console.log('[GD_HTML] Event received:', event.name);

                switch (event.name) {{
                    case 'SDK_READY':
                        console.log('[GD_HTML] SDK Ready');
                        window.gdSdkReady = true;
                        if (window.gdSdkInitResolve) {{
                            window.gdSdkInitResolve();
                            window.gdSdkInitResolve = null;
                        }}
                        break;

                    case 'SDK_GAME_PAUSE':
                        console.log('[GD_HTML] Game paused');
                        // Обработка открытия рекламы
                        if (window.gdCurrentAdType === 'interstitial' && window.gdInterstitialCallbacks) {{
                            console.log('[GD_HTML] Interstitial ad opened');
                            if (window.gdInterstitialCallbacks.open) {{
                                window.gdInterstitialCallbacks.open();
                            }}
                        }} else if (window.gdCurrentAdType === 'rewarded' && window.gdRewardedCallbacks) {{
                            console.log('[GD_HTML] Rewarded ad opened');
                            if (window.gdRewardedCallbacks.open) {{
                                window.gdRewardedCallbacks.open();
                            }}
                        }}
                        break;

                    case 'SDK_GAME_START':
                        console.log('[GD_HTML] Game started');
                        // Обработка закрытия рекламы
                        if (window.gdCurrentAdType === 'interstitial' && window.gdInterstitialCallbacks) {{
                            console.log('[GD_HTML] Interstitial ad closed');
                            if (window.gdInterstitialCallbacks.close) {{
                                window.gdInterstitialCallbacks.close();
                            }}
                            window.gdInterstitialCallbacks = null;
                            window.gdCurrentAdType = null;
                        }} else if (window.gdCurrentAdType === 'rewarded' && window.gdRewardedCallbacks) {{
                            console.log('[GD_HTML] Rewarded ad closed');
                            if (window.gdRewardedCallbacks.close) {{
                                window.gdRewardedCallbacks.close();
                            }}
                            window.gdRewardedCallbacks = null;
                            window.gdCurrentAdType = null;
                        }}
                        break;

                    case 'SDK_REWARDED_WATCH_COMPLETE':
                        console.log('[GD_HTML] Rewarded completed');
                        // Обработка завершения просмотра rewarded рекламы
                        if (window.gdCurrentAdType === 'rewarded' && window.gdRewardedCallbacks) {{
                            console.log('[GD_HTML] Rewarded ad completed');
                            if (window.gdRewardedCallbacks.rewarded) {{
                                window.gdRewardedCallbacks.rewarded();
                            }}
                        }}
                        break;

                    case 'SDK_ERROR':
                        console.error('[GD_HTML] SDK Error:', event);
                        // Обработка ошибок
                        if (window.gdCurrentAdType === 'interstitial' && window.gdInterstitialCallbacks) {{
                            console.error('[GD_HTML] Interstitial ad error');
                            if (window.gdInterstitialCallbacks.error) {{
                                window.gdInterstitialCallbacks.error(event.message || 'Ad error');
                            }}
                            window.gdInterstitialCallbacks = null;
                            window.gdCurrentAdType = null;
                        }} else if (window.gdCurrentAdType === 'rewarded' && window.gdRewardedCallbacks) {{
                            console.error('[GD_HTML] Rewarded ad error');
                            if (window.gdRewardedCallbacks.error) {{
                                window.gdRewardedCallbacks.error(event.message || 'Ad error');
                            }}
                            window.gdRewardedCallbacks = null;
                            window.gdCurrentAdType = null;
                        }}
                        
                        // Обработка ошибок инициализации
                        if (!window.gdSdkReady && window.gdSdkInitReject) {{
                            window.gdSdkInitReject(event.message || 'SDK initialization failed');
                            window.gdSdkInitReject = null;
                        }}
                        break;
                }}
            }}
        }};

        // Загружаем SDK
        (function(d, s, id) {{
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s);
            js.id = id;
            js.src = 'https://html5.api.gamedistribution.com/main.min.js';
            js.onload = function() {{
                console.log('[GD_HTML] SDK script loaded');
            }};
            js.onerror = function(error) {{
                console.error('[GD_HTML] Failed to load SDK script:', error);
                if (window.gdSdkInitReject) {{
                    window.gdSdkInitReject('Failed to load SDK script');
                    window.gdSdkInitReject = null;
                }}
            }};
            fjs.parentNode.insertBefore(js, fjs);
        }}(document, 'script', 'gamedistribution-jssdk'));

        // Таймаут для инициализации
        setTimeout(function() {{
            if (!window.gdSdkReady && window.gdSdkInitReject) {{
                console.error('[GD_HTML] SDK initialization timeout');
                window.gdSdkInitReject('SDK initialization timeout');
                window.gdSdkInitReject = null;
            }}
        }}, 15000);
    </script>
    ";
}

        private string InsertGameDistributionScript(string html, string gdScript)
        {
            // Вставляем после открывающего тега head
            return html.Replace("<head>", "<head>\n" + gdScript);
        }
    }
}