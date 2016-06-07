using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCP_Browser
{
    public class NativeMessagingOutputQueue
    {
        public delegate void MessageForwarder(String Message);


        private byte[] outstandingBytes;
        private MessageForwarder MessageForwarderDelegate;
        private Stream OutputStream;
        private Thread ProcessThread;
        private object LockObject;

        public int PollingTimeout { get; set; }
        private bool Active;
        private ThreadState ThreadState;
        private ThreadState RequestedThreadState;
        private int currentIndex;
        

        public NativeMessagingOutputQueue(Stream OutputStream, MessageForwarder MessageForwarderDelegate)
        {
            this.OutputStream = OutputStream;
            this.MessageForwarderDelegate = MessageForwarderDelegate;
            this.PollingTimeout = 100;
            this.RequestedThreadState = System.Threading.ThreadState.Unstarted;
            this.LockObject = new object();
        }

        public void Start()
        {
            if (ProcessThread == null)
            {
                ProcessThread = new Thread(this.StartThread);
                this.Active = true;
                this.ThreadState = ThreadState.Running;
                this.outstandingBytes = new byte[1000004];
                this.currentIndex = 0;
                ProcessThread.IsBackground = true;
                ProcessThread.Start();
            }
            else
            {
                throw new Exception("Already Started");
            }
        }

        public void Pause()
        {
            if (ProcessThread != null)
            {
                lock (this.LockObject)
                {
                    this.RequestedThreadState = System.Threading.ThreadState.SuspendRequested;
                    this.Active = false;
                }
            }
        }

        public void Resume()
        {
            lock(this.LockObject)
            {
                if(this.RequestedThreadState == System.Threading.ThreadState.Suspended)
                {
                    this.Active = true;
                    this.ProcessThread.Interrupt();
                }
            }
        }

        public void Stop()
        {
            if (ProcessThread != null)
            {
                lock (this.LockObject)
                {
                    this.RequestedThreadState = System.Threading.ThreadState.StopRequested;
                    this.Active = false;
                }
            }
        }

        private void StartThread()
        {
            Run();
        }

        private void Run()
        {
            while (1 == 1)
            {
                lock (this.LockObject)
                {
                    if (!this.Active)
                    {
                        if(this.RequestedThreadState == System.Threading.ThreadState.StopRequested)
                        {
                            this.ThreadState = ThreadState.Stopped;
                        }
                        else if(this.RequestedThreadState == System.Threading.ThreadState.SuspendRequested)
                        {
                            this.ThreadState = ThreadState.SuspendRequested;
                        }                        
                    }
                    this.RequestedThreadState = this.ThreadState;
                }
                if(this.ThreadState == System.Threading.ThreadState.SuspendRequested)
                {
                    try
                    {
                        this.ThreadState = System.Threading.ThreadState.Suspended;
                        lock(this.LockObject)
                        {
                            this.RequestedThreadState = this.ThreadState;
                        }
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException)
                    {
                        this.ThreadState = System.Threading.ThreadState.Running;
                    }
                    catch (ThreadAbortException)
                    {
                        this.ThreadState = System.Threading.ThreadState.AbortRequested;
                    }
                    finally
                    {

                    }
                    continue;
                }
                else if (this.ThreadState == System.Threading.ThreadState.Running)
                {
                    // TODO : process stream
                    
                    // Read All output available
                    
                    uint length = 0;


                    int b = -1;

                    if((b = ReadByte()) != -1)
                    {
                        outstandingBytes[currentIndex] = BitConverter.GetBytes(b)[0];
                        currentIndex++;
                    }

                    // Process

                    // Read First Four Bytes
                    if(currentIndex > 4) // Since we increment this field in the previous block it much be > 4 (0-3 is the integer, 4 is the first byte of the content)
                    {
                        byte[] firstFour = new byte[4];
                        for(int i = 0; i < 4; i++)
                        {
                            firstFour[i] = outstandingBytes[i];
                        }
                        length = BitConverter.ToUInt32(firstFour, 0);

                        // Check if that to an integer can be found in the bytes read
                        // -- Extract bytes to UTF-8 String
                        if (currentIndex >= length + 4)
                        {
                            string message = Encoding.UTF8.GetString(outstandingBytes, 4, (int)length);
                            if (message[0] == '{' && (message[(int)length-2] == '}' || message[(int)length-1] == '}'))
                            {
                                this.MessageForwarderDelegate.Invoke(message);
                                byte[] remainingBytes = new byte[outstandingBytes.Length];
                                currentIndex = 0;
                            }
                        }                        
                    }                    
                }
                else if (this.ThreadState == System.Threading.ThreadState.AbortRequested)
                {
                    OutputStream.Dispose();
                    this.ThreadState = System.Threading.ThreadState.Aborted;
                    break;
                }
                else if (this.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    OutputStream.Dispose();
                    break;
                }
            }
            lock(this.LockObject)
            {
                this.RequestedThreadState = this.ThreadState;
            }
        }

        private int ReadByte()
        {
            int b = -1;
            try
            {
                b = this.OutputStream.ReadByte();
            }
            catch
            {
                b = -1;
            }
            return b;
        }
        
        
    }
}
