using StardewValley.Menus;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EugenesFarm.Player
{
    public class TrailTextMenu : IClickableMenu
    {
        private readonly PlayerMoveText parent;
        private readonly TextBox textBox;
        private readonly ClickableComponent toggleButton;

        public TrailTextMenu(PlayerMoveText parent)
            : base(Game1.viewport.Width / 2 - 150, Game1.viewport.Height / 2 - 50, 300, 100)
        {
            this.parent = parent;
            textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 20,
                Y = this.yPositionOnScreen + 20,
                Width = 260,
                Text = parent.customTrailText
            };
            Game1.keyboardDispatcher.Subscriber = textBox;

            toggleButton = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + 20, this.yPositionOnScreen + 160, 120, 40),
                "toggleTrail"
            );
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

            base.draw(b);
            textBox.Draw(b);

            SpriteText.drawStringWithScrollBackground(
                b,
                "Edit Trail Text:",
                this.xPositionOnScreen + 20,
                this.yPositionOnScreen - 100
            );

            string toggleLabel = parent.TrailEnabled ? "Trail: ON" : "Trail: OFF";
            IClickableMenu.drawTextureBox(
                b,
                toggleButton.bounds.X,
                toggleButton.bounds.Y,
                toggleButton.bounds.Width,
                toggleButton.bounds.Height,
                parent.TrailEnabled ? Color.LimeGreen : Color.Red
            );
            SpriteText.drawString(b, toggleLabel, toggleButton.bounds.X + 10, toggleButton.bounds.Y + 10);

            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            textBox.Update();

            if (toggleButton.containsPoint(x, y))
            {
                parent.ToggleTrail();
                Game1.playSound("drumkit6");
            }
        }

        public override void receiveKeyPress(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (Game1.keyboardDispatcher.Subscriber == textBox)
            {
                if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    parent.SetCustomTrailText(textBox.Text);
                    Game1.exitActiveMenu();
                }

                // Prevent menu from closing on E while typing
                // potentially other keys? not sure yet will have to test
                if (key == Microsoft.Xna.Framework.Input.Keys.E)
                    return;
            }
            else
            {
                base.receiveKeyPress(key);
            }
        }
    }
}