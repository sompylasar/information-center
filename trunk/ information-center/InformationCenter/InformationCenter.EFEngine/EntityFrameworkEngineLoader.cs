using System;
using LogicUtils;
using System.Data.Objects;
using InformationCenter.Data;

namespace InformationCenter.EFEngine
{

    /// <summary>
    /// Класс-загрузчик движков, работающих с помощью технологии Entity Framework.
    /// </summary>
    public abstract class EF_Engine_Loader
    {

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public EF_Engine_Loader() { }

        #endregion

        #region Свойства

        /// <summary>
        /// тип контейнера для БД (наследуется от ObjectContext)
        /// </summary>
        public abstract Type EntityContainerType { get; }

        /// <summary>
        /// тип движка для контейнера (наследуется от EntityFrameworkModelWrapper)
        /// </summary>
        public abstract Type EntityEngineType { get; }

        #endregion

        #region Методы

        /// <summary>
        /// проверка типов на соответствие необходимым условиям
        /// </summary>
        protected virtual void ValidateTypes()
        {
            if (!EntityContainerType.IsSubclassOf(typeof(ObjectContextEx)))
                throw new TypeInheritanceException(EntityContainerType, typeof(ObjectContextEx));
            if (EntityContainerType.GetConstructor(new Type[] { typeof(string) }) == null)
                throw new ConstructorNotFoundException(EntityContainerType, new Type[] { typeof(string) });
            if (!EntityEngineType.IsSubclassOf(typeof(EF_Engine)))
                throw new TypeInheritanceException(EntityEngineType, typeof(EF_Engine));
            if (EntityEngineType.GetConstructor(new Type[] { EntityContainerType }) == null)
                throw new ConstructorNotFoundException(EntityEngineType, new Type[] { EntityContainerType });
        }

        protected virtual bool TestConnection { get { return true; } }

        protected virtual void DoPostConnectionWork(string ConnectionString) { }

        /// <summary>
        /// загрузить Entity Framework движок
        /// </summary>
        /// <param name="ConnectionString">строка соединения</param>
        /// <param name="Exc">выходной объект ошибки (если таковая произошла)</param>
        /// <returns>объект движка</returns>
        protected EF_Engine Load(string Metadata, string ConnectionString, out Exception Exc)
        {
            using (var t = new DebugTimer("EF_Engine_Loader.Load : {0}"))
            {
                bool loading = true;
                ObjectContext entities = null;
                EF_Engine engine = null;
                Exc = null;
                try
                {
                    ValidateTypes(); // проверяет наличие необходимых конструкторов через рефлексию            
                    string prefix = "", postfix = "";
                    if (Metadata[Metadata.Length - 1] != '\'') prefix = "\'";
                    if (ConnectionString[ConnectionString.Length - 1] != '\'') postfix = "\'";
                    entities = (ObjectContextEx)EntityContainerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { Metadata + prefix + ConnectionString + postfix });
                    // обязательно нужно протестировать соединение, потому что entities создаётся почти всегда,
                    // даже когда connectionString кривая
                    if (TestConnection)
                    {
                        entities.Connection.Open();
                        entities.Connection.Close();
                    }
                    DoPostConnectionWork(ConnectionString);
                }
                catch (Exception Ex)
                {
                    loading = false;
                    Exc = new Exception("Не удалось подключиться к источнику данных, смотри InnerException.", Ex);
                    if (entities != null)
                    {
                        entities.Dispose();
                        entities = null;
                    }
                }
                if (loading)
                {
                    try
                    {
                        engine = (EF_Engine)EntityEngineType.GetConstructor(new Type[] { EntityContainerType }).Invoke(new object[] { entities });
                    }
                    catch (Exception Ex)
                    {
                        loading = false;
                        Exc = new Exception("Не удалось создать объект движка над источником данных, смотри InnerException.", Ex);
                        if (engine != null)
                        {
                            engine.Dispose();
                            engine = null;
                        }
                    }
                }
                return engine;
            }
        }

        #endregion

    }

}