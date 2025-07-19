using StardewValley.Menus;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EugenesFarm.Player
{
    public class TrailTextMenu : IClickableMenu
    {
        private readonly PlayerMoveText parent;
        private readonly TextBox textBox;
        private readonly ClickableComponent toggleButton;
        private readonly ClickableComponent trailTypeDropdown;
        private bool dropdownOpen = false;
        private readonly List<string> trailTypes = new List<string> { "Text Trail", "Rainbow Trail" };
        private int selectedTrailType = 0;

        public TrailTextMenu(PlayerMoveText parent)
            : base(Game1.viewport.Width / 2 - 200, Game1.viewport.Height / 2 - 150, 400, 350)
        {
            this.parent = parent;

            textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 30,
                Y = this.yPositionOnScreen + 200,
                Width = 300,
                Text = parent.customTrailText
            };
            textBox.Selected = true;
            Game1.keyboardDispatcher.Subscriber = textBox;

            trailTypeDropdown = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + 30, this.yPositionOnScreen + 100, 200, 40),
                "trailTypeDropdown"
            );

            toggleButton = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + 140, this.yPositionOnScreen + 250, 120, 50),
                "toggleTrail"
            );

            selectedTrailType = (int)parent.CurrentTrailType;
            
            if (selectedTrailType != 0) 
            {
                textBox.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
            }
        }

        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(
                b,
                this.xPositionOnScreen,
                this.yPositionOnScreen,
                this.width,
                this.height,
                Color.White
            );

            string titleText = "Trail Settings";
            var titleSize = Game1.smallFont.MeasureString(titleText);
            Game1.spriteBatch.DrawString(
                Game1.smallFont,
                titleText,
                new Vector2(
                    this.xPositionOnScreen + this.width / 2 - titleSize.X / 2,
                    this.yPositionOnScreen + 20
                ),
                Game1.textColor
            );

            string typeLabel = "Trail Type:";
            Game1.spriteBatch.DrawString(
                Game1.smallFont,
                typeLabel,
                new Vector2(this.xPositionOnScreen + 30, this.yPositionOnScreen + 50),
                Game1.textColor
            );

            Color dropdownColor = dropdownOpen ? Color.LightBlue : Color.White;
            IClickableMenu.drawTextureBox(
                b,
                trailTypeDropdown.bounds.X,
                trailTypeDropdown.bounds.Y,
                trailTypeDropdown.bounds.Width,
                trailTypeDropdown.bounds.Height,
                dropdownColor
            );

            string currentType = trailTypes[selectedTrailType];
            var dropdownTextPos = new Vector2(
                trailTypeDropdown.bounds.X + 10,
                trailTypeDropdown.bounds.Y + 10
            );
            Game1.spriteBatch.DrawString(Game1.smallFont, currentType, dropdownTextPos, Game1.textColor);

            string arrow = dropdownOpen ? "▲" : "▼";
            var arrowPos = new Vector2(
                trailTypeDropdown.bounds.X + trailTypeDropdown.bounds.Width - 25,
                trailTypeDropdown.bounds.Y + 10
            );
            Game1.spriteBatch.DrawString(Game1.smallFont, arrow, arrowPos, Game1.textColor);

            if (dropdownOpen)
            {
                for (int i = 0; i < trailTypes.Count; i++)
                {
                    var optionBounds = new Rectangle(
                        trailTypeDropdown.bounds.X,
                        trailTypeDropdown.bounds.Y + trailTypeDropdown.bounds.Height + (i * 30),
                        trailTypeDropdown.bounds.Width,
                        30
                    );

                    Color optionColor = i == selectedTrailType ? Color.LightGreen : Color.LightGray;
                    IClickableMenu.drawTextureBox(b, optionBounds.X, optionBounds.Y, optionBounds.Width, optionBounds.Height, optionColor);

                    var optionTextPos = new Vector2(optionBounds.X + 10, optionBounds.Y + 5);
                    Game1.spriteBatch.DrawString(Game1.smallFont, trailTypes[i], optionTextPos, Game1.textColor);
                }
            }

            if (selectedTrailType == 0)
            {
                string textLabel = "Trail Text:";
                Game1.spriteBatch.DrawString(
                    Game1.smallFont,
                    textLabel,
                    new Vector2(this.xPositionOnScreen + 30, this.yPositionOnScreen + 150),
                    Game1.textColor
                );
                textBox.Draw(b);
            }

            Color buttonColor = parent.TrailEnabled ? Color.LightGreen : Color.LightCoral;
            IClickableMenu.drawTextureBox(
                b,
                toggleButton.bounds.X,
                toggleButton.bounds.Y,
                toggleButton.bounds.Width,
                toggleButton.bounds.Height,
                buttonColor
            );

            string toggleLabel = parent.TrailEnabled ? "Trail: ON" : "Trail: OFF";
            SpriteText.drawStringHorizontallyCenteredAt(
                b,
                toggleLabel,
                toggleButton.bounds.X + toggleButton.bounds.Width / 2,
                toggleButton.bounds.Y + 50
            );

            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            
            if (selectedTrailType == 0)
            {
                textBox.Update();
            }

            if (trailTypeDropdown.containsPoint(x, y))
            {
                dropdownOpen = !dropdownOpen;
                Game1.playSound("drumkit6");
                return;
            }

            if (dropdownOpen)
            {
                for (int i = 0; i < trailTypes.Count; i++)
                {
                    var optionBounds = new Rectangle(
                        trailTypeDropdown.bounds.X,
                        trailTypeDropdown.bounds.Y + trailTypeDropdown.bounds.Height + (i * 30),
                        trailTypeDropdown.bounds.Width,
                        30
                    );

                    if (optionBounds.Contains(x, y))
                    {
                        selectedTrailType = i;
                        dropdownOpen = false;
                        Game1.playSound("coin");
                        
                        parent.CurrentTrailType = (PlayerMoveText.TrailType)selectedTrailType;
                        
                        if (selectedTrailType == 0)
                        {
                            textBox.Selected = true;
                            Game1.keyboardDispatcher.Subscriber = textBox;
                        }
                        else
                        {
                            textBox.Selected = false;
                            Game1.keyboardDispatcher.Subscriber = null;
                        }
                        return;
                    }
                }
            }

            if (dropdownOpen && !trailTypeDropdown.containsPoint(x, y))
            {
                dropdownOpen = false;
            }

            if (toggleButton.containsPoint(x, y))
            {
                parent.ToggleTrail();
                Game1.playSound("drumkit6");
            }
        }

        public override void receiveKeyPress(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (selectedTrailType == 0 && Game1.keyboardDispatcher.Subscriber == textBox)
            {
                if (key == Microsoft.Xna.Framework.Input.Keys.Enter ||
                    key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    parent.SetCustomTrailText(textBox.Text);
                    Game1.exitActiveMenu();
                    return;
                }

                if (key == Microsoft.Xna.Framework.Input.Keys.E)
                    return;
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Escape && dropdownOpen)
            {
                dropdownOpen = false;
                return;
            }

            base.receiveKeyPress(key);
        }

    }
}
