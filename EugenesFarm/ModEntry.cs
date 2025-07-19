using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using EugenesFarm.Player;

namespace EugenesFarm
{
    /// <summary>The mod entry point.</summary>
    public sealed class ModEntry : Mod
    {
        private PlayerMoveText? moveText;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            moveText = new PlayerMoveText(helper);
        }



        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            this.Monitor.Log("OnDayStarted triggered.", LogLevel.Info);

            var home = Game1.getLocationFromName("FarmHouse");
            if (home != null)
            {
                this.Monitor.Log("FarmHouse found.", LogLevel.Info);


            }
            else
            {
                this.Monitor.Log("FarmHouse not found.", LogLevel.Warn);
            }
        }

        // Helper to get the furniture ID from Json Assets

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Log player position when F3 is pressed
            if (e.Button == SButton.F3)
            {

                int x = (int)Game1.player.Tile.X;
                int y = (int)Game1.player.Tile.Y;
                this.Monitor.Log($"Player tile position: X={x}, Y={y}", LogLevel.Info);
            }

            // Existing debug log
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
    }

}
