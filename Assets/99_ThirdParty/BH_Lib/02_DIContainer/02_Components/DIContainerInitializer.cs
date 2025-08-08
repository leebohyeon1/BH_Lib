using UnityEngine;
using UnityEngine.SceneManagement;

namespace BH_Lib.DI
{
    using Log;
    /// <summary>
    /// DI 컨테이너를 초기화하는 MonoBehaviour 컴포넌트입니다.
    /// 게임 초기화 단계에서 가장 먼저 실행되도록 설정되어 있습니다.
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class DIContainerInitializer : MonoBehaviour
    {
        [SerializeField, Tooltip("게임 전체에서 DI 컨테이너를 유지할지 여부")]
        private bool _dontDestroyOnLoad = true;
        
        [SerializeField, Tooltip("초기화 로그를 출력할지 여부")]
        private bool _enableLogs = true;

        private IDIContainer _container;
        
        private void Awake()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            // DI 컨테이너 인스턴스 참조
            _container = DIContainer.Instance;

            // [Register] 어트리뷰트를 가진 클래스 검색 및 등록
            _container.RegisterAssemblyTypes();
            
            // 현재 씬 컬테이너 초기화
            InitializeCurrentSceneContainer();
            
            // 씬 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (_enableLogs)
            {
                Log.Print("DI Container initialized");
            }
        }
        
        private void OnDestroy()
        {
            // 씬 이벤트 구독 해제
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        private void InitializeCurrentSceneContainer()
        {
            var currentScene = SceneManager.GetActiveScene();
            
            if (_container is DIContainer diContainer)
            {
                diContainer.InitializeSceneContainer(currentScene.name);
                
                if (_enableLogs)
                {
                    Log.Print($"Initialized scene container for: {currentScene.name}");
                }
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_container is DIContainer diContainer)
            {
                diContainer.InitializeSceneContainer(scene.name);
                
                if (_enableLogs)
                {
                    Log.Print($"Scene loaded: {scene.name}, initialized scene container");
                }
            }
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            if (_container is DIContainer diContainer)
            {
                diContainer.CleanupSceneContainer(scene.name);
                
                if (_enableLogs)
                {
                    Log.Print($"Scene unloaded: {scene.name}, cleaned up scene container");
                }
            }
        }
    }
}