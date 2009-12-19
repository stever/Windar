﻿/*
 * Windar: Playdar for Windows
 * Copyright (C) 2009 Steven Robertson <steve@playnode.org>
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

using System.Text;
using Windar.Common;

namespace Windar.PlaydarDaemon.Commands
{
    class InitAppData : ShortCmd<InitAppData>
    {
        public override string Run()
        {
            var cmd = new StringBuilder();

            // Make path to %AppData%\Windar\etc
            cmd.Append("IF NOT EXIST \"").Append(DaemonController.Instance.Paths.PlaydarDataPath);
            cmd.Append("MKDIR \"").Append(DaemonController.Instance.Paths.PlaydarDataPath);
            cmd.Append("IF NOT EXIST \"").Append(DaemonController.Instance.Paths.PlaydarDataPath).Append("\\etc\" ");
            cmd.Append("MKDIR \"").Append(DaemonController.Instance.Paths.PlaydarDataPath).Append("\\etc\"");

            // Copy template configuration files, if not already present.
            cmd.Append("IF NOT EXIST \"").Append(DaemonController.Instance.Paths.PlaydarDataPath).Append("\\etc\\playdar.conf\" ");
            cmd.Append("COPY \"").Append(DaemonController.Instance.Paths.PlaydarPath).Append("\\etc\\playdar.conf\"");
            cmd.Append("IF NOT EXIST \"").Append(DaemonController.Instance.Paths.PlaydarDataPath).Append("\\etc\\playdartcp.conf\" ");
            cmd.Append("COPY \"").Append(DaemonController.Instance.Paths.PlaydarPath).Append("\\etc\\playdartcp.conf\"");

            Runner.SkipLogInfoOutput = true;
            Runner.RunCommand(cmd.ToString());
            ContinueWhenDone();
            return Output;
        }
    }
}
