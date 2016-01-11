using System.Windows.Media;

namespace OakBot
{
    public class DispatchUI
    {
        #region Fields

        private TwitchChatEvent _botEvent;

        #endregion

        #region Consturctor

        public DispatchUI(MainWindow window, TwitchChatEvent chatEvent)
        {
            _botEvent = chatEvent;

            MainWindow.DelUI del = new MainWindow.DelUI(window.ResolveDispatchToUI);
            window.Dispatcher.BeginInvoke(del, this);
        }

        #endregion

        #region Methods

        //internal void dispatchToUI()
        //{
        //    MainWindow.MyDel del = delegate ()
        //    {
        //        _window.ResolveDispatchToUI(sender);
        //    };
        //
        //    _window.Dispatcher.BeginInvoke(del);
        //}

        //internal void dispatchToUI(DispatchUI obj)
        //{
        //    MainWindow.MyDel del = new MainWindow.MyDel(_window.ResolveDispatchToUI);
        //    _window.Dispatcher.BeginInvoke(del, obj);
        //}

        #endregion

        #region Properties

        public TwitchChatEvent botEvent
        {
            get
            {
                return _botEvent;
            }
        }

        #endregion
    }
}
