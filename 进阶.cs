using Stateless;
using Stateless.Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StatelessLearning;
internal class 进阶
{
    public enum State
    {
        Watting,
        Running,
        Holding,
        //running包括以下三个子状态
        Checking,
        Writting,
        Storing
    }
    public enum Trigger
    {
        Start,
        Stop,
        Hold,

        Checked,
        Written,
        Stored,
    }
    public static void 子状态()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .Permit(Trigger.Start, State.Running);
        machine.Configure(State.Running)
           .Permit(Trigger.Stop, State.Watting);
        //Running的初始子状态为Checking
        machine.Configure(State.Running)
        .InitialTransition(State.Checking);

        machine.Configure(State.Checking)
            .SubstateOf(State.Running);
        //将自动转跳到Checking
        machine.Fire(Trigger.Start);
        Console.WriteLine($"具体子状态是：{machine.State}，是否为运行状态：{machine.IsInState(State.Running)}");

    }

    public static void 非状态转移处理()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .InternalTransition(Trigger.Start, () => { Console.WriteLine("直接处理了开始的命令，不需要转移状态"); });
        //将自动转跳到Checking
        machine.Fire(Trigger.Start);
        Console.WriteLine($"当前状态机状态为：{machine.State}");

    }
    class MyState
    {
        public State Value = State.Watting;
    }
    /// <summary>
    /// 有些时候如MVVM要额外实现BindableBase,可通过函数指定外部状态变量
    /// </summary>
    public static void 外部状态存储()
    {
        MyState myState = new MyState();
        //定义部分,通过委托函数修改状态
        var machine = new StateMachine<State, Trigger>(() => myState.Value,
        value => myState.Value = value);

        machine.Configure(State.Watting)
            .InternalTransition(Trigger.Start, () => { Console.WriteLine("直接处理了开始的命令，不需要转移状态"); });
        //将自动转跳到Checking
        machine.Fire(Trigger.Start);
        Console.WriteLine($"当前状态机状态为：{machine.State}");

    }

    public static void 查看状态信息()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .OnEntry(() => { Console.WriteLine($"进入等待状态，此时推理机状态为{machine.State}"); })
            .OnExit(() => { Console.WriteLine("退出等待状态"); })
            //初步感觉这是两个方法集合，不太懂
            .OnActivate(() => { Console.WriteLine($"激活函数"); })
            .OnDeactivate(() => { Console.WriteLine($"非激活函数"); })
            .Permit(Trigger.Start, State.Running)
            .Permit(Trigger.Hold, State.Holding);
        machine.Configure(State.Running)
            .OnEntry(() => { Console.WriteLine("进入运行状态"); })
            .OnExit(() => { Console.WriteLine("退出运行状态"); })
            .Permit(Trigger.Stop, State.Watting);
        machine.Configure(State.Holding);
        var info= machine.GetInfo();
        Console.WriteLine($"初始状态：{info.InitialState}");
        Console.WriteLine($"所有状态：{info.States.Select(s=>s.ToString()).Aggregate((a, b) => $"{a} {b}")}");
        var permittedTriggers= machine.GetPermittedTriggers();
        Console.WriteLine($"当前可用触发器：{permittedTriggers.Select(t=>t.ToString()).Aggregate((a,b)=>$"{a} {b}")}");
    }

    /// <summary>
    /// 不是很懂，只是能用
    /// </summary>
    public static void 带参数的状态转移()
    {
        MyState myState = new MyState();
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);
        machine.Configure(State.Watting)
            .Permit(Trigger.Start,State.Running);
            
        machine.Configure(State.Running)
            .OnEntryFrom(Trigger.Start, email => Console.WriteLine(email.Parameters[0]));

        var b= machine.SetTriggerParameters<string>(Trigger.Start);

        machine.Fire(b, "joe@example.com");
    }

    /// <summary>
    /// uml图
    /// </summary>
    public static void 输出图()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .Permit(Trigger.Start, State.Running);
        machine.Configure(State.Running)
           .Permit(Trigger.Stop, State.Watting);

        string graph = UmlDotGraph.Format(machine.GetInfo());
        Console.WriteLine(graph);
    }
    public static void 动态指定转移状态()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);
        machine.Configure(State.Watting)
            //动态指定
            .PermitDynamic(Trigger.Start, () => State.Watting);
        machine.Configure(State.Running)
           .Permit(Trigger.Stop, State.Watting);

        machine.Fire(Trigger.Start);
        Console.WriteLine(machine.State);
    }
}
