using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace LogicUtils
{

    /// <summary>
    /// Базовый класс для некоторого условия.
    /// </summary>
    public abstract class LogicCondition
    {

        /// <summary>
        /// признак соответствия условию
        /// </summary>
        /// <param name="Value">проверяемый на условие объект</param>
        /// <returns>true или false в зависимости от соответствия условию</returns>
        public abstract bool Match(object Value);

    }

    public abstract class SemanticCondition
    {

        public abstract Exception Match(object Value);

    }

    /// <summary>
    /// Условие, которое всегда выполняется
    /// </summary>
    public class TrueCondition : LogicCondition
    {

        public TrueCondition() { }

        public override bool Match(object Value) { return true; }

    }

    /// <summary>
    /// Условие, которое никогда не выполняется.
    /// </summary>
    public class FalseCondition : LogicCondition
    {

        public FalseCondition() { }

        public override bool Match(object Value) { return false; }

    }

    /// <summary>
    /// Базовый класс для некоторого условия, реализованный с помощью шаблона.
    /// </summary>
    /// <typeparam name="T">тип проверяемого значения</typeparam>
    public abstract class Condition<T> : LogicCondition
    {
        public override bool Match(object Value)
        {
            if (Value is T)
                return Match((T)Value);
            else
                return false;
        }
        /// <summary>
        /// признак соответствия условию
        /// </summary>
        /// <param name="Value">проверяемый на условие объект</param>
        /// <returns>true или false в зависимости от соответствия условию</returns>
        public abstract bool Match(T Value);

    }

    /// <summary>
    /// Базовый класс для сложного (составного) условия.
    /// </summary>
    public abstract class MultiCondition : LogicCondition
    {
    
        /// <summary>
        /// получить интерфейс для перечисления условий
        /// </summary>
        public abstract IEnumerable<LogicCondition> Conditions { get; }      

    }

    /// <summary>
    /// Базовый класс для сложного (составного) условия, реализованный с помощью шаблона.
    /// </summary>
    /// <typeparam name="T">тип проверяемого значения</typeparam>
    public abstract class MultiCondition<T> : Condition<T>
    {

        /// <summary>
        /// получить интерфейс для перечисления условий
        /// </summary>
        public abstract IEnumerable<Condition<T>> Conditions { get; }

    }

    /// <summary>
    /// Базовый класс для конъюнкции.
    /// </summary>
    public abstract class Conjunction : MultiCondition
    {

        public override bool Match(object Value)
        {
            foreach (LogicCondition c in this.Conditions)
                if (!c.Match(Value)) return false;
            return true;
        }

    }

    /// <summary>
    /// Базовый класс для дизъюнкции.
    /// </summary>
    public abstract class Disjunction : MultiCondition
    {

        public override bool Match(object Value)
        {
            foreach (LogicCondition c in this.Conditions)
                if (c.Match(Value)) return true;
            return false;
        }

    }

    /// <summary>
    /// Базовый класс для шаблонной конъюнкции.
    /// </summary>
    /// <typeparam name="T">тип проверяемого значения</typeparam>
    public abstract class Conjunction<T> : MultiCondition<T>
    {

        public override bool Match(T Value)
        {
            foreach (Condition<T> c in this.Conditions)
                if (!c.Match(Value)) return false;
            return true;
        }

    }

    /// <summary>
    /// Базовый класс для шаблонной дизъюнкции.
    /// </summary>
    /// <typeparam name="T">тип проверяемого значения</typeparam>
    public abstract class Disjunction<T> : MultiCondition<T>
    {

        public override bool Match(T Value)
        {
            foreach (Condition<T> c in this.Conditions)
                if (c.Match(Value)) return true;
            return false;
        }
        
    }

    /*   
    public abstract class ConjunctionCondition<T> : Condition<T>
    {
        protected ConjunctionCondition() { }
        protected ConjunctionCondition(IEnumerable<Condition<T>> InheritedConditions)
            : base(InheritedConditions)
        {
        }

        public override bool Match(T value)
        {
            bool result = true;
            foreach (var condition in InheritedConditions)
            {
                bool tempResult = condition.Match(value);
                // Если хоть одно не прокатило, то проверка провалена
                if (!tempResult)
                {
                    result = false;
                    break;
                }
            }
            // Если проверка ещё не провалена
            if (result)
                // Не забываем проверить с собой
                result = SelfCondition(value);
            return result;
        }
    }

    public abstract class DisjunctionCondition<T> : Condition<T>
    {
        protected DisjunctionCondition() { }
        protected DisjunctionCondition(IEnumerable<Condition<T>> InheritedConditions)
            : base(InheritedConditions)
        {
        }

        public override bool Match(T value)
        {
            bool result = true;
            foreach (var condition in InheritedConditions)
            {
                bool tempResult = condition.Match(value);
                // Если хоть одно прокатило, то проверка успешна
                if (tempResult)
                {
                    result = false;
                    break;
                }
            }
            // Если ни одно условие ещё не выполнилось
            if (!result)
                // Не забываем проверить с собой
                result = SelfCondition(value);
            return result;
        }
    }
     */
}
