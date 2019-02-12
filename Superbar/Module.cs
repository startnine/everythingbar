//using Start9.WCF;
using System;
using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace Superbar
{
    public static class Module
    {
        static string _moduleWritePipeHandle = string.Empty;
        static string _moduleReadPipeHandle = string.Empty;

        static AnonymousPipeClientStream _moduleReadPipe;
        static AnonymousPipeClientStream _moduleWritePipe;

        static StreamWriter _writer = null;

        public static void StartModule()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                //MessageBox.Show("ARGUMENTS RETRIEVED");
                _moduleWritePipeHandle = args[1];
                _moduleReadPipeHandle = args[2];
            }
            /*else
            {
                MessageBox.Show("INSUFFICIENT ARGUMENTS");
            }
            foreach (string s in args)
                MessageBox.Show(s, "ARGUMENT");*/

            _moduleReadPipe = new AnonymousPipeClientStream(PipeDirection.In, _moduleReadPipeHandle);
            _moduleWritePipe = new AnonymousPipeClientStream(PipeDirection.Out, _moduleWritePipeHandle);

            _writer = new StreamWriter(_moduleWritePipe)
            {
                AutoFlush = true
            };
        }

        public static void SendMessage(object message)
        {
            //MessageBox.Show("Message sent by Superbar");
            _writer.WriteLine(message);
            _moduleWritePipe.WaitForPipeDrain();
            //MessageBox.Show("PIPE DRAINED");
        }
    }

    /*public class ModuleCallback : ICallbackService
    {
        public void SendMessage(Message message)
        {
            Debug.WriteLine(message.Command.ToString());
        }
    }*/
}
