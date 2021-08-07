using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class SampleScene1LifeTimeScope : LifetimeScope
{
    [SerializeField] private SampleHelloWorldView view;

    // Start is called before the first frame update
    protected override void Configure(IContainerBuilder builder)
    {
        // 1. 登録する
        builder.Register<HelloWorldService>(Lifetime.Singleton);
        
        // 2. entryPointの登録.
        builder.RegisterEntryPoint<GamePresenter>(Lifetime.Scoped);
        
        // 3. MVPのviewを登録する
        builder.RegisterComponent(view);

        // すでに登録されているErrorで怒られた.
        // 不要な模様.
        // builder.RegisterInstance<SampleScene1LifeTimeScope>(this);
    }
}

public class GamePresenter : ITickable, IInitializable, IStartable
{
    private readonly HelloWorldService service;
    private readonly SampleHelloWorldView view;
    private readonly LifetimeScope scope;

    // ConstructorはIL2CPPのstrippingで取り除かれるので、attributeをつけておくこと.
    // https://vcontainer.hadashikick.jp/resolving/constructor-injection
    [Inject]
    public GamePresenter(HelloWorldService service, SampleHelloWorldView view, LifetimeScope scope)
    {
        this.service = service;
        this.view = view;
        this.scope = scope;
    }

    public void Tick()
    {
        // this.service.Hello();
    }

    public void Initialize()
    {
        Debug.Log("Initialize");
    }

    public void Start()
    {
        this.view.button.onClick.AddListener(() => { Debug.Log("OnClick"); });
        this.view.loadSceneButton.onClick.AddListener(() => SceneManager.LoadScene("Scenes/SampleScene2"));
        this.view.loadSceneButton2.onClick.AddListener(async () =>
        {
            // ここはOK
            Assert.IsTrue(scope.GetType() == typeof(SampleScene1LifeTimeScope));
            using (LifetimeScope.EnqueueParent(scope))
            {
                // LoadSceneだと何故かparentが設定されない.
                await SceneManager.LoadSceneAsync("Scenes/SampleScene3", LoadSceneMode.Additive);
            }
        });
    }
}

public class HelloWorldService
{
    public void Hello()
    {
        Debug.Log("Hello");
    }
}