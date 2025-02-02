using System;
using System.Reflection;
using log4net;
using Windar.Common;
using Windar.PlaydarDaemon.Commands;

namespace Windar.PlaydarDaemon
{
    public class DaemonController
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().ReflectedType);

        #region Delegates and events.

        public delegate void PlaydarStartedHandler(object sender, EventArgs e);
        public delegate void PlaydarStartFailedHandler(object sender, EventArgs e);
        public delegate void PlaydarStoppedHandler(object sender, EventArgs e);
        public delegate void ScanCompletedHandler(object sender, EventArgs e);

        public event PlaydarStartedHandler PlaydarStarted;
        public event PlaydarStartFailedHandler PlaydarStartFailed;
        public event PlaydarStoppedHandler PlaydarStopped;
        public event ScanCompletedHandler ScanCompleted;

        #endregion

        #region Properties

        static DaemonController _instance;
        WindarPaths _paths;
        bool _started;

        internal static DaemonController Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        internal WindarPaths Paths
        {
            get { return _paths; }
            set { _paths = value; }
        }

        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public int NumFiles
        {
            get
            {
                string result = Cmd<NumFiles>.Create().Run();
                if (Log.IsDebugEnabled) Log.Debug("NumFiles result = " + result.Trim());
                try
                {
                    return Int32.Parse(result);
                }
                catch (FormatException)
                {
                    //TODO: Try to create a useful error message.
                    throw new Exception(result);
                }
            }
        }

        #endregion

        public DaemonController(WindarPaths paths)
        {
            Paths = paths;
            Instance = this;
            Started = false;

            // Create user AppData files if necessary.
            Cmd<InitAppData>.Create().Run();
        }

        #region Commands

        public void Start()
        {
            Start cmd = Cmd<Start>.Create();
            cmd.PlaydarStarted += StartCmd_PlaydarStarted;
            cmd.PlaydarStartFailed += StartCmd_PlaydarStartFailed;
            cmd.RunAsync();
            Started = true;
            System.Threading.Thread.Sleep(500);
        }

        public void Stop()
        {
            Cmd<Stop>.Create().Run();
            Started = false;
            PlaydarStopped(this, new EventArgs());
            System.Threading.Thread.Sleep(1000);
        }

        public void Restart()
        {
            if (Started) Stop();
            Start();
        }

        public string Ping()
        {
            return Cmd<Ping>.Create().Run();
        }

        public string Status()
        {
            return Cmd<Status>.Create().Run();
        }

        public string DumpLibrary()
        {
            return Cmd<DumpLibrary>.Create().Run();
        }

        public void Scan(string path)
        {
            Scan cmd = Cmd<Scan>.Create();
            cmd.ScanCompleted += ScanCmd_ScanCompleted;
            cmd.ScanPath = path;
            cmd.RunAsync();
        }

        #endregion

        #region Command event handlers.

        void StartCmd_PlaydarStarted(object sender, EventArgs e)
        {
            Started = true;
            PlaydarStarted(this, e);
        }

        void StartCmd_PlaydarStartFailed(object sender, EventArgs e)
        {
            Started = false;
            PlaydarStartFailed(this, e);
        }

        void ScanCmd_ScanCompleted(object sender, EventArgs e)
        {
            ScanCompleted(this, e);
        }

        #endregion
    }
}
