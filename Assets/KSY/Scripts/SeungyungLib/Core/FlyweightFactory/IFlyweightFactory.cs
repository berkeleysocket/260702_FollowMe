using System;

namespace SeungyungLib.Core.FlyweightService
{
    //하나의 인스턴스를 공유해서 사용하자는 취지의 서비스기 때문에 Value는 무조건 class다.
    public interface IFlyweightFactory<TKey, TValue> 
        where TValue : class, IFlyweight
    {
        TValue? GetOrAdd(
            TKey key, 
            Func<TValue> add);
        TValue? GetOrAdd<TArgs>(
            TKey key, 
            TArgs arg, 
            Func<TArgs, TValue> add);
        TValue? GetOrAdd<TArgs1, TArgs2>(
            TKey key, 
            TArgs1 arg1, 
            TArgs2 arg2, 
            Func<TArgs1, TArgs2, TValue> add);
        TValue? GetOrAdd<TArgs1, TArgs2, TArgs3>(
            TKey key, 
            TArgs1 arg1, 
            TArgs2 arg2, 
            TArgs3 args3, 
            Func<TArgs1, TArgs2, TArgs3, TValue> add);
    }
}