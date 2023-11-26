using BattleShipEngine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;

namespace BattleShipStrategies.MartinF.Unethical;

public class MartinParasiticStrategy : IGameStrategy
{
    public static GameSetting ExposedSettings { get; private set; } = GameSetting.Default;

    private readonly IBoardCreationStrategy _parasiticBoardStrategy;
    private Int2[] moves = null!;
    private int currentMoveIndex;
    public MartinParasiticStrategy()
    {
        //Intercept all board creation strategies and replace them with my own
        _parasiticBoardStrategy = new BoardStrategyToInject();
        NukeThem();
    }

    private void NukeThem()
    {
        //Intercept board creation strategies
        var parasiticBoardCreationMethod = _parasiticBoardStrategy.GetType()
            .GetMethod(nameof(IBoardCreationStrategy.GetBoatPositions), BindingFlags.Instance | BindingFlags.Public)!;

        var assembly = typeof(MartinParasiticStrategy).Assembly;
        var boardCreationTypes = assembly
            .GetTypes()
            .Where(t => 
                t.GetInterfaces().Contains(typeof(IBoardCreationStrategy)) 
                && !t.Name.StartsWith("Martin"));
        foreach (var boardCreationType in boardCreationTypes)
        {
            //Intercept GetBoatPositions
            var originalMethod = boardCreationType.GetMethod(nameof(IBoardCreationStrategy.GetBoatPositions),
                BindingFlags.Instance | BindingFlags.Public)!;

            originalMethod.RedirectTo(parasiticBoardCreationMethod);
        }

        //Intercept move strategies
        var gameStrategyToInject = new GameStrategyToInject();
        var parasiticMoveMethod = gameStrategyToInject.GetType()
            .GetMethod(nameof(IGameStrategy.GetMove), BindingFlags.Instance | BindingFlags.Public)!;


        var gameStrategyTypes = assembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces().Contains(typeof(IGameStrategy))
                && !t.Name.StartsWith("Martin"));

        foreach (var gameStrategyType in gameStrategyTypes)
        {
            //Intercept GetMove
            var originalMoveMethod = gameStrategyType.GetMethod(nameof(IGameStrategy.GetMove),
                BindingFlags.Instance | BindingFlags.Public)!;
            
            originalMoveMethod.RedirectTo(parasiticMoveMethod);
        }
    }

    public Int2 GetMove()
    {
        var move = moves[currentMoveIndex];
        currentMoveIndex++;

        return move;
    }

    public void RespondHit()
    {
    }

    public void RespondSunk()
    {
    }

    public void RespondMiss()
    {
    }

    public void Start(GameSetting setting)
    {
        NukeThem();
        currentMoveIndex = 0;
        moves = _parasiticBoardStrategy.GetBoatPositions(setting);
        ExposedSettings = setting;
    }
}

file static class InterceptorClass
{
    public static void RedirectTo(this MethodInfo origin, MethodInfo target)
    {
        IntPtr ori = GetMethodAddress(origin);
        IntPtr tar = GetMethodAddress(target);

        Marshal.Copy(new IntPtr[] { Marshal.ReadIntPtr(tar) }, 0, ori, 1);
    }

    private static IntPtr GetMethodAddress(MethodInfo mi)
    {
        const ushort SLOT_NUMBER_MASK = 0xffff; // 2 bytes mask
        const int MT_OFFSET_32BIT = 0x28;       // 40 bytes offset
        const int MT_OFFSET_64BIT = 0x40;       // 64 bytes offset

        IntPtr address;

        // JIT compilation of the method
        RuntimeHelpers.PrepareMethod(mi.MethodHandle);

        IntPtr md = mi.MethodHandle.Value;             // MethodDescriptor address
        IntPtr mt = mi.DeclaringType.TypeHandle.Value; // MethodTable address

        if (mi.IsVirtual)
        {
            // The fixed-size portion of the MethodTable structure depends on the process type
            int offset = IntPtr.Size == 4 ? MT_OFFSET_32BIT : MT_OFFSET_64BIT;

            // First method slot = MethodTable address + fixed-size offset
            // This is the address of the first method of any type (i.e. ToString)
            IntPtr ms = Marshal.ReadIntPtr(mt + offset);

            // Get the slot number of the virtual method entry from the MethodDesc data structure
            long shift = Marshal.ReadInt64(md) >> 32;
            int slot = (int)(shift & SLOT_NUMBER_MASK);

            // Get the virtual method address relative to the first method slot
            address = ms + (slot * IntPtr.Size);
        }
        else
        {
            // Bypass default MethodDescriptor padding (8 bytes) 
            // Reach the CodeOrIL field which contains the address of the JIT-compiled code
            address = md + 8;
        }

        return address;
    }
}

file class BoardStrategyToInject : IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        List<Int2> boats = new List<Int2>();
        int line = 0;
        int positionInLine = 0;
        for (int boatType = 0; boatType < setting.BoatCount.Length; boatType++)
            for (int boat = 0; boat < setting.BoatCount[boatType]; boat++)
            {
                if (positionInLine + boatType + 1 > setting.Width)
                {
                    line += 2;
                    positionInLine = 0;
                }
                for (int boatPart = 0; boatPart < boatType + 1; boatPart++)
                    boats.Add(new Int2(line, positionInLine + boatPart));
                positionInLine += boatType + 2;
            }
        return boats.ToArray();
    }
}

file class GameStrategyToInject : IGameStrategy
{
    public Int2 GetMove()
    {
        return new Int2(
            Random.Shared.Next(0, MartinParasiticStrategy.ExposedSettings.Width),
            Random.Shared.Next(0, MartinParasiticStrategy.ExposedSettings.Height));
    }

    public void RespondHit()
    {
    }

    public void RespondSunk()
    {
    }

    public void RespondMiss()
    {
    }

    public void Start(GameSetting setting)
    {
    }
}