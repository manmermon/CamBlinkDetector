/*
* Copyright 2018-2019 by Manuel Merino Monge <manmermon@dte.us.es>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lslAFFSDK
{
    public class ExcCommander
    {
        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN  = 0x0002;
        private const int MOUSEEVENTF_LEFTUP    = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP   = 0x0010;

        //This simulates a left mouse click
        private void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void DoubleLeftMouseClick(int xpos, int ypos)
        {
            this.LeftMouseClick( xpos, ypos);
            Thread.Sleep(50);
            this.LeftMouseClick(xpos, ypos);
        }

        private void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
        }

        private void DoubleRightMouseClick(int xpos, int ypos)
        {
            this.RightMouseClick(xpos, ypos);
            Thread.Sleep(50);
            this.RightMouseClick(xpos, ypos);
        }

        private List< string > commands;
        public ExcCommander( )
        {
            this.commands = new List< string >();
        }

        public void addCommand( string command )
        {
            this.commands.Add(command);
        }

        public void clearCommands()
        {
            this.commands.Clear();
        }

        public void execute()
        {
            System.Drawing.Point mouseLoc = System.Windows.Forms.Cursor.Position;

            foreach (string cmd in this.commands)
            {
                if( cmd.ToLower().Equals( "mouseright" ) )
                {
                    this.RightMouseClick(mouseLoc.X, mouseLoc.Y);
                }
                else if (cmd.ToLower().Equals("mouseleft"))
                {
                    this.LeftMouseClick(mouseLoc.X, mouseLoc.Y);
                }
                else if (cmd.ToLower().Equals("mousedoubleright"))
                {
                    this.DoubleRightMouseClick(mouseLoc.X, mouseLoc.Y);
                }
                else if (cmd.ToLower().Equals("mousedoubleleft"))
                {
                    this.DoubleLeftMouseClick(mouseLoc.X, mouseLoc.Y);
                }
                else
                {
                    SendKeys.SendWait( cmd );
                }                
            }
        }
    }
}
