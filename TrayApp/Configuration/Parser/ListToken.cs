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

using System.Collections.Generic;
using System.Text;

namespace Windar.TrayApp.Configuration.Parser
{
    class ListToken : ParserToken, IValueToken
    {
        public List<ParserToken> Tokens { get; set; }

        public ListToken()
        {
            Tokens = new List<ParserToken>();
        }

        public ListToken(List<ParserToken> list)
        {
            Tokens = list;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            foreach (var token in Tokens)
                result.Append(token.ToString());
            result.Append(']');
            return result.ToString();
        }
    }
}
