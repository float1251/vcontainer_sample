using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

public class SampleScene2LifeTimeScope : LifetimeScope
{

    [SerializeField] private SampleHelloWorldView view;
    
    protected override void Configure(IContainerBuilder builder)
    {
        // componentの登録
        builder.RegisterComponent(view);
        
        // gameobjectの生成.
        builder.RegisterComponentOnNewGameObject<NewGameObjectCompoent>(Lifetime.Transient, "Test");

        // entryPointの登録.
        builder.RegisterEntryPoint<SampleScene2Presenter>();
        
        // scene1であったものがないことを確認する
        Assert.IsNull(Parent);
        
        // builder ExistsはこのLifeTimeScope内のものしか判定できないから意味無し...
        Debug.Log(builder.Exists(typeof(HelloWorldService)).ToString());
    }
}

public class SampleScene2Presenter: IInitializable
{
    private readonly SampleHelloWorldView view;
    private readonly IObjectResolver resolver;

    public SampleScene2Presenter(SampleHelloWorldView view, IObjectResolver resolver)
    {
        this.view = view;
        this.resolver = resolver;
    }
    
    
    public void Initialize()
    {
        this.view.button.onClick.AddListener(()=>
        {
            Debug.Log("SampleScene2");
            this.resolver.Resolve<NewGameObjectCompoent>();
        });
    }
}
