using Stateless;
using Stateless.Graph;

namespace StatelessLearning;
//定义 停 -开始-> 运行中 -停止->停
public enum State
{
    Watting,
    Running,
}
public enum Trigger
{
    Start,
    Stop,
}

internal class 基本
{
    /// <summary>
    /// 所有需要用的状态都必须通过Configure配置
    /// </summary>
    public static void 使用方法()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .Permit(Trigger.Start, State.Running);
        machine.Configure(State.Running)
           .Permit(Trigger.Stop, State.Watting);
        //操作部分
        machine.Fire(Trigger.Start);
        Console.WriteLine(machine.State);
        machine.Fire(Trigger.Stop);
        Console.WriteLine(machine.State);
    }

    public static void 状态转移事件()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .OnEntry(() => { Console.WriteLine($"进入等待状态，此时推理机状态为{machine.State}"); })
            .OnExit(() => { Console.WriteLine("退出等待状态"); })
            //初步感觉这是两个方法集合，不太懂
            .OnActivate(() => {  Console.WriteLine($"激活函数"); })
            .OnDeactivate(() => { Console.WriteLine($"非激活函数"); })
            .Permit(Trigger.Start, State.Running);
        machine.Configure(State.Running)
            .OnEntry(() => { Console.WriteLine("进入运行状态"); })
            .OnExit(() => { Console.WriteLine("退出运行状态"); })
            .Permit(Trigger.Stop, State.Watting);

        //操作部分
        machine.Fire(Trigger.Start);
       
        machine.Fire(Trigger.Stop);
        //手动激活
        machine.Activate();
        machine.Deactivate();
    }

    public static void 卫子句()
    {
        bool graud = false;
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            .PermitIf(Trigger.Start, State.Running,()=> graud);
        //操作部分
        Console.WriteLine($"能否进行状态转移：{machine.CanFire(Trigger.Start)}，若直接执行将会报错");
    }

    public static void 自转移与忽略()
    {
        //定义部分
        var machine = new StateMachine<State, Trigger>(State.Watting);

        machine.Configure(State.Watting)
            //略
            .Ignore(Trigger.Stop)
            .Permit(Trigger.Start, State.Running);
        machine.Configure(State.Running)
            //重指向
            .PermitReentry(Trigger.Start)
            .Permit(Trigger.Stop, State.Watting);
        //操作部分
        machine.Fire(Trigger.Start);
        machine.Fire(Trigger.Start);
        Console.WriteLine(machine.State);
        machine.Fire(Trigger.Stop);
        machine.Fire(Trigger.Stop);
        Console.WriteLine(machine.State);
    }
}
