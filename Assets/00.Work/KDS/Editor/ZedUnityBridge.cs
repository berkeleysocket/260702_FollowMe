#if UNITY_EDITOR
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

// Zed <-> Unity HTTP 브리지
// Zed의 태스크(curl)로 Unity 에디터를 원격 조작하기 위한 로컬 서버입니다.
// 위치: Assets/Editor/ZedUnityBridge.cs
//
// 지원 명령:
//   /play       플레이 모드 진입
//   /stop       플레이 모드 종료
//   /pause      일시정지 토글
//   /refresh    AssetDatabase.Refresh()
//   /compile    에셋 리프레시 + 스크립트 재컴파일 요청
//   /status     현재 상태 (playing/paused/stopped | compiling/idle)
//   /logs       최근 콘솔 로그 반환
//   /clearlogs  로그 버퍼 비우기
//   /menu?path=...  임의의 Unity 메뉴 아이템 실행 (확장 포인트)
[InitializeOnLoad]
public static class ZedUnityBridge
{
    private const string ListenPrefix = "http://localhost:17890/";
    private const int MaxLogCount = 300;

    private static HttpListener _listener;
    private static Thread _listenThread;
    private static volatile string _cachedStatus = "unknown";

    private static readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();
    private static readonly ConcurrentQueue<string> _logBuffer = new ConcurrentQueue<string>();

    static ZedUnityBridge()
    {
        EditorApplication.update += OnEditorUpdate;
        Application.logMessageReceivedThreaded += OnLogReceived;

        // 도메인 리로드/에디터 종료 시 포트를 반드시 반납해야 재시작이 가능합니다.
        AssemblyReloadEvents.beforeAssemblyReload += StopServer;
        EditorApplication.quitting += StopServer;

        StartServer();
    }

    private static void StartServer()
    {
        if (_listener != null)
        {
            return;
        }

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(ListenPrefix);
            _listener.Start();

            _listenThread = new Thread(ListenLoop)
            {
                IsBackground = true,
                Name = "ZedUnityBridge"
            };
            _listenThread.Start();

            Debug.Log($"[ZedBridge] 서버 시작: {ListenPrefix}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ZedBridge] 서버 시작 실패: {e.Message} (다른 Unity 인스턴스가 같은 포트를 사용 중일 수 있습니다)");
            _listener = null;
        }
    }

    private static void StopServer()
    {
        if (_listener == null)
        {
            return;
        }

        try
        {
            _listener.Stop();
            _listener.Close();
        }
        catch (Exception)
        {
            // 종료 과정의 예외는 무시합니다.
        }

        _listener = null;
    }

    private static void ListenLoop()
    {
        while (_listener != null && _listener.IsListening)
        {
            HttpListenerContext context;

            try
            {
                context = _listener.GetContext();
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            try
            {
                HandleRequest(context);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ZedBridge] 요청 처리 실패: {e.Message}");
            }
        }
    }

    private static void HandleRequest(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();
        string body = "ok";
        int statusCode = 200;

        switch (path)
        {
            case "/play":
                EnqueueMainThread(() => EditorApplication.EnterPlaymode());
                break;

            case "/stop":
                EnqueueMainThread(() => EditorApplication.ExitPlaymode());
                break;

            case "/pause":
                EnqueueMainThread(() => EditorApplication.isPaused = !EditorApplication.isPaused);
                break;

            case "/refresh":
                EnqueueMainThread(() => AssetDatabase.Refresh());
                break;

            case "/compile":
                EnqueueMainThread(() =>
                {
                    AssetDatabase.Refresh();
                    CompilationPipeline.RequestScriptCompilation();
                });
                break;

            case "/status":
                body = _cachedStatus;
                break;

            case "/logs":
                body = _logBuffer.Count > 0 ? string.Join("\n", _logBuffer) : "(로그 없음)";
                break;

            case "/clearlogs":
                while (_logBuffer.TryDequeue(out _)) { }
                break;

            case "/menu":
                string menuPath = context.Request.QueryString["path"];

                if (string.IsNullOrEmpty(menuPath))
                {
                    body = "path 파라미터가 필요합니다. 예: /menu?path=Window/General/Test Runner";
                    statusCode = 400;
                }
                else
                {
                    EnqueueMainThread(() =>
                    {
                        bool executed = EditorApplication.ExecuteMenuItem(menuPath);

                        if (executed == false)
                        {
                            Debug.LogWarning($"[ZedBridge] 메뉴 실행 실패: {menuPath}");
                        }
                    });
                }
                break;

            default:
                body = "지원 명령: /play /stop /pause /refresh /compile /status /logs /clearlogs /menu?path=...";
                statusCode = 404;
                break;
        }

        WriteResponse(context, body, statusCode);
    }

    private static void WriteResponse(HttpListenerContext context, string body, int statusCode)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(body);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private static void EnqueueMainThread(Action action)
    {
        if (action == null)
        {
            return;
        }

        _mainThreadActions.Enqueue(action);
    }

    private static void OnEditorUpdate()
    {
        // Unity API 대부분은 메인 스레드 전용이므로, 리스너 스레드에서 받은 명령을 여기서 실행합니다.
        while (_mainThreadActions.TryDequeue(out Action action))
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError($"[ZedBridge] 메인 스레드 작업 실패: {e.Message}");
            }
        }

        // 리스너 스레드에서 안전하게 읽을 수 있도록 상태를 캐시해 둡니다.
        string playState = EditorApplication.isPlaying ? "playing" : "stopped";

        if (EditorApplication.isPaused)
        {
            playState = "paused";
        }

        string compileState = EditorApplication.isCompiling ? "compiling" : "idle";
        _cachedStatus = $"{playState} | {compileState}";
    }

    private static void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        string line = $"[{DateTime.Now:HH:mm:ss}] [{type}] {condition}";
        _logBuffer.Enqueue(line);

        while (_logBuffer.Count > MaxLogCount)
        {
            _logBuffer.TryDequeue(out _);
        }
    }
}
#endif
