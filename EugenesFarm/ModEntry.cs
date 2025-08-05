using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            moveText = new PlayerMoveText(helper, this.Monitor);

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }


        private void OnMenuChanged(object? sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is StardewValley.Menus.TitleMenu titleMenu)
            {
                this.Monitor.Log("Title screen detected - you could modify it here", LogLevel.Debug);
            }
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.DataType == typeof(Texture2D))
            {
                this.Monitor.Log($"Texture requested: {e.Name}", LogLevel.Info);
            }

            // Try multiple possible logo locations
            var logoAssets = new[]
            {
        //"LooseSprites/logo",
        //"TitleScreen",
         //"LooseSprites/titleLogo",
        "Minigames/TitleButtons",
        //"LooseSprites/Cursors"
    };

            foreach (var logoAsset in logoAssets)
            {
                if (e.NameWithoutLocale.IsEquivalentTo(logoAsset))
                {
                    this.Monitor.Log($"FOUND LOGO ASSET: {logoAsset}", LogLevel.Info);

                    e.Edit(asset =>
                    {
                        var texture = asset.AsImage();
                        this.Monitor.Log($"{logoAsset} dimensions: {texture.Data.Width}x{texture.Data.Height}", LogLevel.Info);

                        try
                        {
                            var customTitleImage = this.Helper.ModContent.Load<Texture2D>("assets/titlescreen2.png");
                            this.Monitor.Log($"Custom image loaded: {customTitleImage.Width}x{customTitleImage.Height}", LogLevel.Info);

                            if (customTitleImage.Width == 0 || customTitleImage.Height == 0)
                            {
                                this.Monitor.Log("Custom title image is empty, skipping patching.", LogLevel.Warn);
                                return;
                            }
                            // the location of the main title asset was annoying to figure out. why is it in minigames - title buttons???
                            // why is the titel a minigame? xd
                            if (logoAsset == "Minigames/TitleButtons")
                            {

                                int logoWidth = 500;
                                int logoHeight = 190;
                                int sourceWidth = Math.Min(customTitleImage.Width, logoWidth);
                                int sourceHeight = Math.Min(customTitleImage.Height, logoHeight);

                                texture.PatchImage(
                                    source: customTitleImage,
                                    sourceArea: new Rectangle(0, 0, sourceWidth, sourceHeight),
                                    targetArea: new Rectangle(0, 0, sourceWidth, sourceHeight)
                                );

                                this.Monitor.Log($"Successfully replaced Stardew Valley title at (0,0) with size {sourceWidth}x{sourceHeight}", LogLevel.Info);
                            }
                            else if (logoAsset == "LooseSprites/Cursors")
                            {
                                -this.Monitor.Log($"Skipping Cursors asset to avoid affecting other UI elements", LogLevel.Info);
                            }
                            else
                            {
                                -this.Monitor.Log($"Skipping {logoAsset} to avoid unintended modifications", LogLevel.Info);
                            }

                            this.Monitor.Log($"Successfully processed {logoAsset}!", LogLevel.Info);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"Failed to patch {logoAsset}: {ex.Message}", LogLevel.Warn);
                        }
                    });
                }
            }

            if (e.Name.Name.ToLower().Contains("title") || e.Name.Name.ToLower().Contains("logo"))
            {
                this.Monitor.Log($"POTENTIAL TITLE/LOGO ASSET: {e.Name}", LogLevel.Info);
            }
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
