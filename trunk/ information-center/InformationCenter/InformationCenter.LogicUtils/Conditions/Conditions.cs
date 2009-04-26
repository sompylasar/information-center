using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace LogicUtils
{
    //public class intervalparameterscondition : condition<parametercollection>
    //{
    //    public string startdateparametername { get; private set; }
    //    public string enddateparametername { get; private set; }

    //    public intervalparameterscondition(string startdateparamname, string enddateparamname)
    //    {
    //        startdateparametername = startdateparamname;
    //        enddateparametername = enddateparamname;
    //    }

    //    public override bool match(parametercollection value)
    //    {
    //        bool result = false;
    //        var parstart = value.params.first(s => s.name == startdateparametername);
    //        var parend = value.params.first(s => s.name == enddateparametername);

    //        if ((parstart != null) && (parend != null))
    //            if ((parstart.value is datetime) && (parend.value is datetime))
    //                result = ((datetime)parstart.value) <= ((datetime)parend.value);

    //        return result;
    //    }
    //}

    /// <summary>
    /// >= ComparableValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class GreatEqualParameterCondition<T> : Condition<Parameter> where T:IComparable
    //{
    //    public T ComparableValue { get; private set; }

    //    public GreatEqualParameterCondition(T ComparableValue)
    //    {
    //        this.ComparableValue = ComparableValue;
    //    }

    //    /// <summary>
    //    /// Проверить
    //    /// </summary>
    //    /// <param name="Value">Объект проверки</param>
    //    /// <returns></returns>
    //    public override bool Match(Parameter Value)
    //    {
    //        if (!(Value.Value is T))
    //            return false;

    //        return ((T)(Value.Value)).CompareTo(ComparableValue) >= 0;
    //    }
    //}

    /// <summary>
    /// <= ComparableValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class LessEqualParameterCondition<T> : Condition<Parameter> where T : IComparable
    //{
    //    public T ComparableValue { get; private set; }

    //    public LessEqualParameterCondition(T ComparableValue)
    //    {
    //        this.ComparableValue = ComparableValue;
    //    }

    //    /// <summary>
    //    /// Проверить
    //    /// </summary>
    //    /// <param name="Value">Объект проверки</param>
    //    /// <returns></returns>
    //    public override bool Match(Parameter Value)
    //    {
    //        if (!(Value.Value is T))
    //            return false;

    //        return ((T)(Value.Value)).CompareTo(ComparableValue) <= 0;
    //    }
    //}
}