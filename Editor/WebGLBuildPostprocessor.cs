﻿using System.IO;
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

            // GameDistribution script injection
            if (settings.SDKType == SupportedSDKType.GameDistribution &&
                !string.IsNullOrEmpty(settings.GameDistributionId))
            {
                var gdScript = GenerateGameDistributionScript(settings.GameDistributionId);
                html = InsertGameDistributionScript(html, gdScript);
            }

            // Y8 script injection
            if (settings.SDKType == SupportedSDKType.Y8Games)
            {
                var y8Scripts = GenerateY8Scripts(settings);
                html = InsertY8Scripts(html, y8Scripts);
            }

            // PlayGamma script injection - добавляем загрузку PlayGamma SDK
            if (settings.SDKType == SupportedSDKType.PlayGamma)
            {
                var playGammaScripts = GeneratePlayGammaScripts();
                html = InsertPlayGammaScripts(html, playGammaScripts);
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
            return html.Replace("<head>", "<head>\n" + gdScript);
        }

        private string GenerateY8Scripts(SDKSettings settings)
        {
            var appId = string.IsNullOrEmpty(settings.Y8AppId) ? "YOUR_Y8_APP_ID" : settings.Y8AppId;
            var hostId = string.IsNullOrEmpty(settings.Y8HostId) ? "ca-host-pub-6129580795478709" : settings.Y8HostId;
            var adsenseId = string.IsNullOrEmpty(settings.Y8AdsenseId) ? "ca-pub-6129580795478709" : settings.Y8AdsenseId;
            var channelId = string.IsNullOrEmpty(settings.Y8ChannelId) ? "123456" : settings.Y8ChannelId;
            var adFrequency = string.IsNullOrEmpty(settings.Y8AdFrequency) ? "180s" : settings.Y8AdFrequency;
            var testAdsOn = settings.Y8TestAdsOn ? "true" : "false";
            var activateAFP = settings.Y8ActivateAFP ? "true" : "false";
            
            return $@"
    <!-- Y8 Games SDK -->
    <script src='https://cdn.y8.com/api/sdk.js' async></script>
    
    <!-- Y8 Configuration -->
    <script>
        window.Y8_APP_ID = '{appId}';
        console.log('[Y8_HTML] Y8 configuration loaded, App ID: {appId}');
    </script>

    <!-- Y8 GameBreak Ad System -->
    <script>
        (function() {{
            console.log('[Y8_HTML] Initializing Y8 GameBreak ad system...');
            
            var imported = document.createElement('script');
            var HostId = '{hostId}';
            var AdsenseId = '{adsenseId}';
            var ChannelId = '{channelId}';
            var adFrequency = '{adFrequency}';
            var testAdsOn = {testAdsOn};
            var activateAFP = {activateAFP};

            window.adsbygoogle = window.adsbygoogle || [];
            const adBreak = adConfig = function(o) {{adsbygoogle.push(o);}}
            adConfig({{
                preloadAdBreaks: 'on',
                sound: 'on', // This game has sound
                onReady: () => {{
                    console.log('[Y8_HTML] Ad system ready');
                }}, // Called when API has initialised and adBreak() is ready
            }});

            function nextAds()
            {{
                console.log('[Y8_HTML] showNextAd');
                adBreak({{
                    type: 'start', // ad shows at start of next level
                    name: 'start-game',
                    beforeAd: () => {{            
                        console.log('[Y8_HTML] beforeAd');
                        if (window.pauseGame) window.pauseGame();
                    }}, 
                    afterAd: () => {{
                        console.log('[Y8_HTML] afterAd');
                        if (window.resumeGame) window.resumeGame();
                    }}, 
                    adBreakDone: (placementInfo) => {{
                        console.log('[Y8_HTML] adBreak complete');
                        console.log('Type:', placementInfo.breakType);
                        console.log('Name:', placementInfo.breakName);
                        console.log('Format:', placementInfo.breakFormat);
                        console.log('Status:', placementInfo.breakStatus);
                        
                        // Всегда возобновляем игру
                        if (window.resumeGame) window.resumeGame();
                        
                        // Обработка различных статусов
                        if (placementInfo.breakStatus === 'viewed') {{
                            console.log('[Y8_HTML] Interstitial ad viewed successfully');
                        }} else if (placementInfo.breakStatus === 'frequencyCapped') {{
                            console.log('[Y8_HTML] Interstitial ad frequency capped');
                        }} else if (placementInfo.breakStatus === 'other') {{
                            console.log('[Y8_HTML] Interstitial ad failed with other reason');
                        }}
                    }},
                }});
            }}

            function showReward()
            {{
                console.log('[Y8_HTML] showReward');
                adBreak({{
                    type: 'reward', 
                    name: 'rewarded Ad',
                    beforeAd: () => {{            
                        console.log('[Y8_HTML] beforeAd');
                        if (window.pauseGame) window.pauseGame();
                    }}, 
                    afterAd: () => {{
                        console.log('[Y8_HTML] afterAd');
                        // Возобновление будет вызвано в adBreakDone
                    }},
                    beforeReward: (showAdFn) => {{ 
                        console.log('[Y8_HTML] beforeReward');
                        showAdFn(0);
                    }},
                    adDismissed: () => {{
                        console.log('[Y8_HTML] adDismissed');
                        if (window.rewardAdsCanceled) window.rewardAdsCanceled();
                    }},
                    adViewed: () => {{
                        console.log('[Y8_HTML] adViewed');
                        if (window.rewardAdsCompleted) window.rewardAdsCompleted();
                    }},
                    adBreakDone: (placementInfo) => {{
                        console.log('[Y8_HTML] adBreak complete');
                        console.log('Type:', placementInfo.breakType);
                        console.log('Name:', placementInfo.breakName);
                        console.log('Format:', placementInfo.breakFormat);
                        console.log('Status:', placementInfo.breakStatus);
                        
                        // Всегда возобновляем игру
                        if (window.resumeGame) window.resumeGame();
                        
                        // Обработка различных статусов
                        if (placementInfo.breakStatus === 'viewed') {{
                            console.log('[Y8_HTML] Rewarded ad viewed successfully');
                        }} else if (placementInfo.breakStatus === 'frequencyCapped') {{
                            console.log('[Y8_HTML] Rewarded ad frequency capped');
                            if (window.NoRewardedAdsTryLater) window.NoRewardedAdsTryLater();
                        }} else if (placementInfo.breakStatus === 'other') {{
                            console.log('[Y8_HTML] Rewarded ad failed with other reason');
                            if (window.NoRewardedAdsTryLater) window.NoRewardedAdsTryLater();
                        }} else if (placementInfo.breakStatus === 'dismissed') {{
                            console.log('[Y8_HTML] Rewarded ad dismissed by user');
                            if (window.rewardAdsCanceled) window.rewardAdsCanceled();
                        }}
                    }},
                }});
            }}

            // Глобальные функции для работы с Unity адаптером
            window.pauseGame = function()
            {{
                console.log('[Y8_HTML] pauseGame');
                // Вызываем паузу в Unity через адаптер
                if (window.__sdk_adapter && window.__sdk_adapter.OnGamePause) {{
                    window.__sdk_adapter.OnGamePause(true);
                }}
            }}

            window.resumeGame = function()
            {{
                console.log('[Y8_HTML] resumeGame');
                // Вызываем возобновление в Unity через адаптер
                if (window.__sdk_adapter && window.__sdk_adapter.OnGamePause) {{
                    window.__sdk_adapter.OnGamePause(false);
                }}
            }}

            window.rewardAdsCanceled = function()
            {{
                console.log('[Y8_HTML] rewardAdsCanceled');
                if (window.__sdk_adapter && window.__sdk_adapter.OnRewardCanceled) {{
                    window.__sdk_adapter.OnRewardCanceled();
                }}
            }}

            window.rewardAdsCompleted = function()
            {{
                console.log('[Y8_HTML] RewardGained');
                if (window.__sdk_adapter && window.__sdk_adapter.OnRewardCompleted) {{
                    window.__sdk_adapter.OnRewardCompleted();
                }}
            }}

            window.NoRewardedAdsTryLater = function()
            {{
                console.log('[Y8_HTML] NoRewardedAdsTryLater');
                if (window.__sdk_adapter && window.__sdk_adapter.OnNoRewardedAds) {{
                    window.__sdk_adapter.OnNoRewardedAds();
                }}
            }}

            function createAFGScript()
            {{
                console.log('[Y8_HTML] createAFGScript');
                if(activateAFP == true){{imported.setAttribute('data-ad-host', HostId)}};
                imported.setAttribute('data-ad-client', AdsenseId);
                if(activateAFP == false){{imported.setAttribute('data-ad-channel', ChannelId)}};
                imported.setAttribute('data-ad-frequency-hint', adFrequency);
                if(testAdsOn == true){{imported.setAttribute('data-adbreak-test', 'on');}}
                imported.src = 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js';
                imported.setAttribute('type', 'text/javascript');
                imported.async = true;
                imported.onload = function() {{
                    console.log('[Y8_HTML] Ad script loaded successfully');
                }};
                imported.onerror = function(error) {{
                    console.error('[Y8_HTML] Failed to load ad script:', error);
                }};
                document.head.appendChild(imported);
            }}

            // Инициализируем рекламную систему
            createAFGScript();

            // Экспортируем функции для глобального доступа
            window.nextAds = nextAds;
            window.showReward = showReward;

            console.log('[Y8_HTML] Y8 GameBreak ad system initialized');
        }})();
    </script>
    ";
        }

        private string InsertY8Scripts(string html, string y8Scripts)
        {
            // Вставляем перед закрывающим тегом head
            return html.Replace("</head>", y8Scripts + "\n</head>");
        }
        
        private string GeneratePlayGammaScripts()
        {
            return $@"
    <!-- PlayGamma Bridge SDK -->
    <script src='https://bridge.playgama.com/v1/stable/playgama-bridge.js'></script>
    
    <!-- PlayGamma Configuration Info -->
    <script>
        console.log('[PLAYGAMMA_HTML] PlayGamma SDK loaded from CDN');
        console.log('[PLAYGAMMA_HTML] Remember to add playgama-bridge-config.json to your StreamingAssets folder');
        
        // Глобальные переменные для отслеживания состояния PlayGamma SDK
        window.playGammaSdkReady = false;
        window.playGammaSdkInitPromise = null;
        
        // Инициализация будет выполнена через адаптер
        window.addEventListener('DOMContentLoaded', function() {{
            console.log('[PLAYGAMMA_HTML] DOM ready, PlayGamma SDK can be initialized by adapter');
        }});
    </script>
    ";
        }

        private string InsertPlayGammaScripts(string html, string playGammaScripts)
        {
            // Вставляем PlayGamma SDK в head для предварительной загрузки
            return html.Replace("<head>", "<head>\n" + playGammaScripts);
        }
    }
}