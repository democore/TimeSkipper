using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Storm.ExternalEvent;
using Storm.StardewValley.Event;
using Storm.StardewValley.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSkipper
{
    [Mod]
    public class TimeSkipper : DiskResource
    {
        Texture2D button;
        Vector2 buttonPosition;
        bool wasMouseDown = false;

        [Subscribe]
        public void Initalize(InitializeEvent ev)
        {
            String path = Path.Combine(PathOnDisk, "forwardButton.png");
            button = ev.Root.LoadResource(path);
        }

        [Subscribe]
        public void PostRender(PreUIRenderEvent ev)
        {
            if (ev.Root.DisplayHUD)
            {
                var batch = ev.Root.SpriteBatch;
                var viewport = ev.Root.Viewport;
                buttonPosition = new Vector2(viewport.Width - button.Width * 3.4f, button.Height * 6.3f);
                batch.Draw(button, new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, 33, 33), new Color(255, 255, 255, 255));
            }
        }

        [Subscribe]
        public void UpdateCallback(PreUpdateEvent preUpdateevent)
        {
            MouseState mouseState = preUpdateevent.Root.OldMouseState;
            StaticContext root = preUpdateevent.Root;

            if (!wasMouseDown && mouseState.LeftButton == ButtonState.Pressed)
            {
                float windowToGameWidth = (float)root.Viewport.Width / (float)root.Window.ClientBounds.Width;
                float windowToGameHeight = (float)root.Viewport.Height / (float)root.Window.ClientBounds.Height;
                Rectangle mouseRect = new Rectangle(Convert.ToInt32(mouseState.X * windowToGameWidth), Convert.ToInt32(mouseState.Y * windowToGameHeight), 1, 1);
                Rectangle buttonRect = new Rectangle(Convert.ToInt32(buttonPosition.X), Convert.ToInt32(buttonPosition.Y), 40, 40);

                if (mouseRect.Intersects(buttonRect))
                {
                    //button pressed
                    wasMouseDown = true;
                    root.TimeOfDay += 10;
                    String timeOfDayString = root.TimeOfDay.ToString();
                    if (timeOfDayString.Length > 2)
                    {
                        int minuteTenth;
                        if (int.TryParse(timeOfDayString[timeOfDayString.Length - 2].ToString(), out minuteTenth))
                        {
                            if (minuteTenth >= 6)
                            {
                                root.TimeOfDay += (10 - minuteTenth) * 10;
                            }
                        }
                    }
                }
                /*Console.WriteLine("Pressed At: " + mouseRect);
                Console.WriteLine("Button At:" + buttonRect);
                Console.WriteLine("Viewport Size: " + root.Viewport);
                Console.WriteLine("ingamezoom: " + windowToGameWidth);
                Console.WriteLine("Window Size: " + root.Window.ClientBounds);*/
            }
            if (wasMouseDown && mouseState.LeftButton == ButtonState.Released)
            {
                wasMouseDown = false;
            }
        }
    }
}
