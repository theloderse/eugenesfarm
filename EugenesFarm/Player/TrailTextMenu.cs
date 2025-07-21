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
        private readonly List<string> trailTypes = new List<string> { "Text Trail", "Rainbow Trail", "Star Trail" };
        private int selectedTrailType = 0;

        public TrailTextMenu(PlayerMoveText parent)
            : base(Game1.uiViewport.Width / 2 - 250, Game1.uiViewport.Height / 2 - 225, 500, 450)
        {
            this.parent = parent;

            textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 50,
                Y = this.yPositionOnScreen + 270,
                Width = 400,
                Text = parent.customTrailText
            };
            textBox.Selected = true;
            Game1.keyboardDispatcher.Subscriber = textBox;

            trailTypeDropdown = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + 50, this.yPositionOnScreen + 140, 250, 60),
                "trailTypeDropdown"
            );

            toggleButton = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + 190, this.yPositionOnScreen + 370, 160, 60),
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
                    this.yPositionOnScreen + 40
                ),
                Game1.textColor
            );

            string typeLabel = "Trail Type:";
            Game1.spriteBatch.DrawString(
                Game1.smallFont,
                typeLabel,
                new Vector2(this.xPositionOnScreen + 50, this.yPositionOnScreen + 90),
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
                trailTypeDropdown.bounds.X + 20,
                trailTypeDropdown.bounds.Y + 18
            );
            Game1.spriteBatch.DrawString(Game1.smallFont, currentType, dropdownTextPos, Game1.textColor);

            string arrow = dropdownOpen ? "▲" : "▼";
            var arrowPos = new Vector2(
                trailTypeDropdown.bounds.X + trailTypeDropdown.bounds.Width - 35,
                trailTypeDropdown.bounds.Y + 18
            );
            Game1.spriteBatch.DrawString(Game1.smallFont, arrow, arrowPos, Game1.textColor);

            if (dropdownOpen)
            {
                int optionHeight = 60;
                int optionWidth = trailTypeDropdown.bounds.Width + 60;
                int startY = trailTypeDropdown.bounds.Y - (optionHeight * trailTypes.Count);

                for (int i = 0; i < trailTypes.Count; i++)
                {
                    var optionBounds = new Rectangle(
                        trailTypeDropdown.bounds.X,
                        startY + (i * optionHeight),
                        optionWidth,
                        optionHeight
                    );

                    Color optionColor = i == selectedTrailType ? Color.LightGreen : Color.LightGray;
                    IClickableMenu.drawTextureBox(b, optionBounds.X, optionBounds.Y, optionBounds.Width, optionBounds.Height, optionColor);

                    var optionTextPos = new Vector2(optionBounds.X + 20, optionBounds.Y + 18);
                    Game1.spriteBatch.DrawString(Game1.smallFont, trailTypes[i], optionTextPos, Game1.textColor);
                }
            }

            if (selectedTrailType == 0)
            {
                textBox.Draw(b);
            }

            Color buttonColor = parent.TrailEnabled ? Color.LightGreen : Color.LightCoral;
            IClickableMenu.drawTextureBox(
                b,
                toggleButton.bounds.X - 30,
                toggleButton.bounds.Y - 40,
                toggleButton.bounds.Width,
                toggleButton.bounds.Height,
                buttonColor
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
                int optionHeight = 60;
                int optionWidth = trailTypeDropdown.bounds.Width + 60;
                int startY = trailTypeDropdown.bounds.Y - (optionHeight * trailTypes.Count);

                for (int i = 0; i < trailTypes.Count; i++)
                {
                    var optionBounds = new Rectangle(
                        trailTypeDropdown.bounds.X,
                        startY + (i * optionHeight),
                        optionWidth,
                        optionHeight
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
