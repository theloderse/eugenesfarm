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

                var furnitureId = GetFurnitureId("Eugene Sprite");
                this.Monitor.Log($"Furniture ID for 'Eugene Sprite': {furnitureId}", LogLevel.Info);

                if (furnitureId != -1)
                {
                    var furniture = new StardewValley.Objects.Furniture("Eugene Sprite", new Vector2(2, 2));
                    home.furniture.Add(furniture);
                    this.Monitor.Log("Furniture placed in farmhouse.", LogLevel.Info);
                }
                else
                {
                    this.Monitor.Log("Furniture ID not found. Is Json Assets loaded and your content pack installed?", LogLevel.Warn);
                }
            }
            else
            {
                this.Monitor.Log("FarmHouse not found.", LogLevel.Warn);
            }
        }

        // Helper to get the furniture ID from Json Assets
        public int GetFurnitureId(string name)
        {
            var ja = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            return ja?.GetFurnitureId(name) ?? -1;
        }

        // Interface for Json Assets API
        public interface IJsonAssetsApi
        {
            int GetFurnitureId(string name);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
    }
}