using System.Windows.Media;

namespace OakBot
{
    public class DispatchUI
    {
        #region Fields

        private TwitchChatMessage _chatMessage;

        #endregion

        #region Consturctor

        public DispatchUI(MainWindow window, TwitchChatMessage chatMessage)
        {
            _chatMessage = chatMessage;

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

        public TwitchChatMessage chatMessage
        {
            get
            {
                return _chatMessage;
            }
        }

        #endregion
    }
}
