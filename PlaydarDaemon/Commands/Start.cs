﻿/*
 * Windar: Playdar for Windows
 * Copyright (C) 2009 Steven Robertson <http://stever.org.uk/>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text;
using Windar.Common;

namespace Windar.PlaydarController.Commands
{
    class Start : AsyncCmd<Start>
    {
        public delegate void PlaydarStartedHandler(object sender, EventArgs e);
        public delegate void PlaydarStartFailedHandler(object sender, EventArgs e);

        public event PlaydarStartedHandler PlaydarStarted;
        public event PlaydarStartFailedHandler PlaydarStartFailed;

        #region State

        private enum State
        {
            Initial,
            Started
        }

        private State _state;

        #endregion

        public Start()
        {
            _state = State.Initial;
            Runner.CommandOutput += StartCmd_CommandOutput;
            Runner.CommandError += StartCmd_CommandError;
        }

        public override void RunAsync()
        {
            Cmd<CopyAppFilesToAppData>.Create().Run();

            Runner.RunCommand(@"cd " + DaemonController.Instance.PlaydarDataPath);

            //NOTE: Following is not currently required as etc is found in current dir.
            //Runner.RunCommand("set PLAYDAR_ETC=" + Paths.PlaydarDataPath + @"\etc");

            var cmd = new StringBuilder();
            cmd.Append('"').Append(DaemonController.Instance.ErlCmd).Append('"');
            cmd.Append(" -sname playdar@localhost");
            cmd.Append(" -noinput");
            cmd.Append(" -pa \"").Append(DaemonController.Instance.PlaydarPath).Append("\\ebin\"");
            cmd.Append(" -boot start_sasl");
            cmd.Append(" -s reloader");
            cmd.Append(" -s playdar");
            Runner.RunCommand(cmd.ToString());
        }

        protected void StartCmd_CommandOutput(object sender, CmdRunner.CommandEventArgs e)
        {
            if (e.Text.Trim().Equals("started_at: playdar@localhost"))
            {
                _state = State.Started;
                PlaydarStarted(this, new EventArgs());
            }
            
            //TODO: Also check for errors.
        }

        protected void StartCmd_CommandError(object sender, CmdRunner.CommandEventArgs e)
        {
            //TODO: Check string for error.
        }
    }
}
