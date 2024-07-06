using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Explorer
{
    public class MyEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ThreadSender
    {
        public event EventHandler<MyEventArgs> MessageSent;

        public void SendMessage(string message)
        {
            // Thực hiện một số tác vụ
            System.Threading.Thread.Sleep(1000);

            // Gửi thông tin đến thread nhận
            OnMessageSent(new MyEventArgs { Message = message });
        }

        protected virtual void OnMessageSent(MyEventArgs e)
        {
            MessageSent?.Invoke(this, e);
        }
    }

    public class ThreadReceiver
    {
        private readonly ThreadSender _threadSender;

        public ThreadReceiver(ThreadSender threadSender)
        {
            _threadSender = threadSender;
            _threadSender.MessageSent += ThreadSender_MessageSent;
        }

        private void ThreadSender_MessageSent(object sender, MyEventArgs e)
        {
            // Xử lý thông tin nhận được từ thread gửi
            Console.WriteLine($"Received message: {e.Message}");
        }
    }
}
