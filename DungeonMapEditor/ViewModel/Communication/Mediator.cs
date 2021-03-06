using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel.Communication
{
    public sealed class Mediator
    {
        #region Data
        static readonly Mediator instance = new Mediator();
        private volatile object locker = new object();

        MultiDictionary<ViewModelMessage, Action<Object>> internalList
            = new MultiDictionary<ViewModelMessage, Action<Object>>();
        #endregion

        #region Ctor
        //CTORs
        static Mediator()
        {


        }

        private Mediator()
        {

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The singleton instance
        /// </summary>
        public static Mediator Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Registers a Colleague to a specific message
        /// </summary>
        /// <param name="callback">The callback to use
        /// when the message it seen</param>
        /// <param name="message">The message to
        /// register to</param>
        public void Register(Action<Object> callback,
            ViewModelMessage message)
        {
            internalList.AddValue(message, callback);
        }


        /// <summary>
        /// Notify all colleagues that are registered to the
        /// specific message
        /// </summary>
        /// <param name="message">The message for the notify by</param>
        /// <param name="args">The arguments for the message</param>
        public void NotifyColleagues(ViewModelMessage message,
            object args)
        {
            if (internalList.ContainsKey(message))
            {
                //forward the message to all listeners
                foreach (Action<object> callback in
                    internalList[message].ToList())
                    callback(args);
            }
        }
        #endregion

    }
}