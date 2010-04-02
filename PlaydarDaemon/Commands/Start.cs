﻿/*************************************************************************
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * Copyright (C) 2009, 2010 Steven Robertson <steve@playnode.org>
 *
 * Windar: Playdar for Windows
 *
 * Windar is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * Windar is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ************************************************************************/

using System;
using System.Reflection;
using System.Text;
using log4net;
using Windar.Common;

namespace Windar.PlaydarDaemon.Commands
{
    class Start : AsyncCmd<Start>
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().ReflectedType);

        public delegate void PlaydarStartedHandler(object sender, EventArgs e);
        public delegate void PlaydarStartFailedHandler(object sender, EventArgs e);

        public event PlaydarStartedHandler PlaydarStarted;
        public event PlaydarStartFailedHandler PlaydarStartFailed;

        #region State

        enum State
        {
            Initial,
            ApplicationInfo
        }

        State _state;

        #endregion

        public Start()
        {
            _state = State.Initial;
            Runner.CommandOutput += StartCmd_CommandOutput;
            Runner.CommandError += StartCmd_CommandError;
        }

        public override void RunAsync()
        {
            Runner.RunCommand("CD " + DaemonController.Instance.Paths.PlaydarProgramPath);
            Runner.RunCommand("SET PLAYDAR_ETC=" + DaemonController.Instance.Paths.WindarAppData + @"\etc");

            var cmd = new StringBuilder();
            cmd.Append('"').Append(DaemonController.Instance.Paths.ErlCmd).Append('"');
            cmd.Append(" -sname playdar@localhost");
            cmd.Append(" -noinput");
            cmd.Append(" -pa \"").Append(DaemonController.Instance.Paths.PlaydarProgramPath).Append("\\ebin\"");
            cmd.Append(" -boot start_sasl");
            cmd.Append(" -s reloader");
            cmd.Append(" -s playdar");

            Runner.RunCommand(cmd.ToString());
        }

        protected void StartCmd_CommandOutput(object sender, CmdRunner.CommandEventArgs e)
        {
            var currentLine = e.Text;
            if (!currentLine.StartsWith("IGNORE: "))
            {
                if (currentLine.Length > 0)
                {
                    if (Log.IsDebugEnabled) Log.Debug(string.Format("CMD.INF: [erl.exe] {0}", currentLine));
                }
            }
            switch (_state)
            {
                case State.Initial:
                    if (e.Text.Trim().Equals("application: playdar"))
                    {
                        _state = State.ApplicationInfo;
                    }
                    break;

                case State.ApplicationInfo:
                    _state = State.Initial;
                    if (e.Text.Trim().Equals("started_at: playdar@localhost"))
                    {
                        PlaydarStarted(this, new EventArgs());
                    }
                    else if (e.Text.Trim().Equals("exited: {shutdown,{playdar_app,start,[normal,[]]}}"))
                    {
                        PlaydarStartFailed(this, new EventArgs());
                    }
                    break;
            }
        }

        protected void StartCmd_CommandError(object sender, CmdRunner.CommandEventArgs e)
        {
            //TODO: Check string for error.
        }
    }
}
