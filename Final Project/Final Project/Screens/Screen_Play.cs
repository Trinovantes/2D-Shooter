using System;
using System.Collections.Generic;
using Final_Project.Classes;
using Final_Project.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;

namespace Final_Project.Screens
{
    // store various timers for game
    public struct Timer
    {
        public TimeSpan GameLevel;
        public TimeSpan Enemy;
        public TimeSpan Projectile;
        public TimeSpan OnHitDelay;
        public TimeSpan Drop;

        public Timer(double gameLevel, double enemy, double projectile, double onHitDelay, double drop, double powerUp)
        {
            GameLevel = TimeSpan.FromSeconds(gameLevel);
            Enemy = TimeSpan.FromSeconds(enemy);
            Projectile = TimeSpan.FromSeconds(projectile);
            OnHitDelay = TimeSpan.FromSeconds(onHitDelay);
            Drop = TimeSpan.FromSeconds(drop);
        }
    }

    // keep track of current powerup and modifies game values
    public class PowerUpTracker
    {
        TimeSpan expireTime;
        TimeSpan remainingTime;
        POWERUP type;

        public bool Active;
        public float Projectile;
        public float Speed;
        public float Score;
        public float Damage;

        public PowerUpTracker()
        {
            Reset();
        }

        public void Reset()
        {
            expireTime = TimeSpan.Zero;

            Active = false;
            Projectile = 1;
            Speed = 1;
            Score = 1;
            Damage = 1;
        }

        public void NewPowerUp(GameTime gameTime, POWERUP powerUp)
        {
            Reset();

            Active = true;
            expireTime = gameTime.TotalGameTime + Settings.Intervals.Drop;      // lasts for min time for a drop to appear
            type = powerUp;

            switch (type)
            {
                case POWERUP.YELLOW:
                    Score = 2;
                    break;

                case POWERUP.GREEN:
                    Speed = 2;
                    break;

                case POWERUP.BLUE:
                    Projectile = 2;
                    break;

                case POWERUP.RED:
                    Damage = 2;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            remainingTime = expireTime - gameTime.TotalGameTime;

            if (remainingTime <= TimeSpan.Zero)
            {
                // power up expired
                Reset();
            }
        }

        public override string ToString()
        {
            string output = "";

            if (Active)
            {
                switch (type)
                {
                    case POWERUP.YELLOW:
                        output = "Double Points";
                        break;

                    case POWERUP.GREEN:
                        output = "Double Speed";
                        break;

                    case POWERUP.BLUE:
                        output = "Double Firing Rate";
                        break;

                    case POWERUP.RED:
                        output = "Double Damage";
                        break;
                }

                output += String.Format(" ({0:D2})", remainingTime.Seconds);
            }

            return output;
        }

        public Color InfoColour()
        {
            Color output = Color.Black;

            switch (type)
            {
                case POWERUP.YELLOW:
                    output = Color.Yellow;
                    break;

                case POWERUP.GREEN:
                    output = Color.DarkGreen;
                    break;

                case POWERUP.BLUE:
                    output = Color.DarkBlue;
                    break;

                case POWERUP.RED:
                    output = Color.DarkRed;
                    break;
            }

            return output;
        }
    }

    // main game
    class Screen_Play : Screen
    {
        Random random;
        Game game;

        int gameLevel;
        int projectileLevel;
        int score;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        ParallaxBackground[] backgrounds = new ParallaxBackground[2]; // BUG: third background flickers and position is glitched - otherwise there would be 3 backgrounds to be init'd
        Vector2 infoPosition;
        Vector2 infoDefaultPosition;

        Unit_Animated_Player player;
        PowerUpTracker currentPowerUp;

        List<Unit_Projectile> projectiles;
        List<Unit_Enemy> enemies;
        List<Unit_Animated_Explosion> explosions;
        List<Unit_Drop> drops;
        Timer interval;
        Timer previousInterval;
        
        //------------------------------------------------------------------------------------

        public bool GameOver
        {
            get { return player.Health < 0; }
        }

        public int Score
        {
            get { return score; }
        }

        //------------------------------------------------------------------------------------

        public Screen_Play(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            this.random = new Random();
            this.game = game;

            gameLevel = 0;
            projectileLevel = 0;
            score = 0;

            // Create backgrounds
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i] = new ParallaxBackground(game, spriteBatch, i);
                backgrounds[i].SetSpeed(Settings.CloudSpeeds[i]);
            }

            // Create player
            player = new Unit_Animated_Player(game, spriteBatch);
            currentPowerUp = new PowerUpTracker();

            // Prepare lists
            projectiles = new List<Unit_Projectile>();
            enemies = new List<Unit_Enemy>();
            explosions = new List<Unit_Animated_Explosion>();
            drops = new List<Unit_Drop>();
            
            // Misc
            infoDefaultPosition = new Vector2(Settings.InfoTitle_Padding, Settings.Viewport.Height - (spriteFont.LineSpacing + Settings.InfoTitle_Padding));
            infoPosition = infoDefaultPosition;

            interval = Settings.Intervals;
            previousInterval = new Timer();
        }

        public override void Update(GameTime gameTime)
        {
            /*
             * powerup modifiers must be updated before other updates
             * 
             * other update order does not matter much as long as checks are done after
             * position updates
             * 
             * even if positioning of something should've been done before/after something else,
             * game draws so fast that the eye can't even tell
             * 
             */

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            currentPowerUp.Update(gameTime);

            UpdateLevel(gameTime);
            UpdateBackgrounds(gameTime);
            UpdatePlayer(gameTime);
            UpdateProjectiles(gameTime);
            UpdateEnemies(gameTime);
            UpdateExplosions(gameTime);
            UpdateDrops(gameTime);

            CheckPlayerEnemyCollision(gameTime);
            CheckProjectileEnemyCollision(gameTime);
            CheckPlayerDropCollision(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            /* 
             * Draw Stack:
             * - Background
             * - Game Info
             * 
             * - Explosions     (due to its obstructive view)
             * - Projectiles
             * - Enemies
             * - Drops
             * - Player
             */

            DrawBackgrounds(gameTime);
            DrawInfo();

            DrawUnitsList<Unit_Animated_Explosion>(gameTime, explosions);
            DrawUnitsList<Unit_Projectile>(gameTime, projectiles);
            DrawUnitsList<Unit_Enemy>(gameTime, enemies);
            DrawUnitsList<Unit_Drop>(gameTime, drops);
            player.Draw(gameTime);
        }

        //------------------------------------------------------------------------------------

        // only update units list
        private void UpdateUnits<unitType>(GameTime gameTime, IList<unitType> units) where unitType : Unit
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Update(gameTime);
            }
        }

        // only delete inactive units
        private void DeleteInactiveUnits<unitType>(GameTime gameTime, IList<unitType> units) where unitType : Unit
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (!units[i].Active)
                {
                    units.RemoveAt(i);
                }
            }
        }

        // combined udate/delete if no process is needed in the middle
        private void UpdateUnitsList<unitType>(GameTime gameTime, IList<unitType> units) where unitType : Unit
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Update(gameTime);

                if (!units[i].Active)
                {
                    units.RemoveAt(i);
                }
            }
        }

        //------------------------------------------------------------------------------------

        private void UpdateLevel(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousInterval.GameLevel >= interval.GameLevel)
            {
                // time for new level
                previousInterval.GameLevel = gameTime.TotalGameTime;
                gameLevel++;

                // speed up backgrounds
                for (int i = 0; i < backgrounds.Length; i++)
                {
                    backgrounds[i].IncreaseSpeed();
                }
            }
        }

        private void UpdateBackgrounds(GameTime gameTime)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].Update(gameTime);
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.AdjustSpeed(currentPowerUp.Speed);

            if (KeyPress(Keys.Left)) { player.MoveLeft(); }
            if (KeyPress(Keys.Right)) { player.MoveRight(); }
            if (KeyPress(Keys.Up)) { player.SpeedUp(); }
            if (KeyPress(Keys.Down)) { player.SpeedDown(); }

            player.Position = new Vector2(MathHelper.Clamp(player.Position.X, 0, Width - player.Width), MathHelper.Clamp(player.Position.Y, 0, Height - player.Height));
            player.Update(gameTime);
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            if (KeyPress(Keys.Space))
            {
                if (gameTime.TotalGameTime - previousInterval.Projectile >= TimeSpan.FromMilliseconds(interval.Projectile.Milliseconds / currentPowerUp.Projectile))
                {
                    // ready for new shot
                    previousInterval.Projectile = gameTime.TotalGameTime;

                    // start angle for first projectile
                    // relative to standard position
                    int start = (180 - projectileLevel * Settings.Projectile_MidAngle) / 2;

                    // create projectiles until angle loops to opposite side
                    for (int theta = start; theta <= 180 - start; theta += 10)
                    {
                        projectiles.Add(new Unit_Projectile(game, spriteBatch, player, projectileLevel, theta));
                    }
                }
            }

            UpdateUnitsList(gameTime, projectiles);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // check if it's time to spawn new enemy
            if (gameTime.TotalGameTime - previousInterval.Enemy >= interval.Enemy)
            {
                // use max enemy spawn time as range for generating next enemy's spawn time
                // range decreases as game level increases
                int maxSpawnTime = (int)Settings.Intervals.Enemy.TotalMilliseconds;
                int newSpawnTime = (int)MathHelper.Clamp(maxSpawnTime / (gameLevel + 1), 1, maxSpawnTime);

                // update current spawn time to prepare for next spawn
                previousInterval.Enemy = gameTime.TotalGameTime;

                // modify spawn time accordingly with game level with a bit of randomness
                interval.Enemy = TimeSpan.FromMilliseconds(random.Next(1, newSpawnTime));

                enemies.Add(new Unit_Enemy(game, spriteBatch, gameLevel));
            }

            UpdateUnits(gameTime, enemies);

            // loop through every enemy and add score if it's destroyed
            foreach (Unit_Enemy enemy in enemies)
            {
                if (enemy.Destoryed)
                {
                    // increase score if enemy is destroyed by projectile
                    score += (int)(enemy.PointValue * currentPowerUp.Score);

                    // create new explosion
                    explosions.Add(new Unit_Animated_Explosion(game, spriteBatch, enemy));

                    // create a new drop if it's time
                    CreateDrops(gameTime, enemy);
                }
            }

            DeleteInactiveUnits(gameTime, enemies);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            UpdateUnitsList<Unit_Animated_Explosion>(gameTime, explosions);
        }

        private void CreateDrops(GameTime gameTime, Unit_Enemy sourceEnemy)
        {
            if (gameTime.TotalGameTime - previousInterval.Drop >= interval.Drop)
            {
                // use drop spawn time as range for generating next drop's spawn time
                int minSpawnTime = (int)Settings.Intervals.Drop.TotalMilliseconds;
                int maxSpawnTime = minSpawnTime * 2;

                // update current spawn time to prepare for next spawn
                previousInterval.Drop = gameTime.TotalGameTime;

                // modify spawn time accordingly with game level with a bit of randomness
                interval.Drop = TimeSpan.FromMilliseconds(random.Next(minSpawnTime, maxSpawnTime));

                // 5 types of drops, randomizes the drop type; generated here because C# will generate 
                // a better random value when using the same Random object
                // 20% chance of projectile level up
                // 80% chance of random powerup
                if (random.Next(5) > 0)
                {
                    drops.Add(new Unit_Drop_PowerUp(game, spriteBatch, sourceEnemy, random.Next(0, 4)));
                }
                else
                {
                    drops.Add(new Unit_Drop_ChocolateBall(game, spriteBatch, sourceEnemy));
                }
            }
        }

        private void UpdateDrops(GameTime gameTime)
        {
            UpdateUnitsList<Unit_Drop>(gameTime, drops);
        }

        //------------------------------------------------------------------------------------

        private void CheckProjectileEnemyCollision(GameTime gameTime)
        {
            for (int p = 0; p < projectiles.Count; p++)
            {
                for (int e = 0; e < enemies.Count; e++)
                {
                    if (projectiles[p].Active)
                    {
                        if (enemies[e].Area.Intersects(projectiles[p].Area))
                        {
                            projectiles[p].Active = false;
                            enemies[e].Health -= (int)(projectiles[p].Damage * currentPowerUp.Damage);
                        }
                    }
                }
            }
        }

        private void CheckPlayerEnemyCollision(GameTime gameTime)
        {
            // not immune to hits once delay is over
            if (gameTime.TotalGameTime - previousInterval.OnHitDelay >= interval.OnHitDelay)
            {
                player.YActive = false;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (player.Area.Intersects(enemies[i].Area))
                {
                    if (gameTime.TotalGameTime - previousInterval.OnHitDelay >= interval.OnHitDelay)
                    {
                        // on hit, new immunity delay
                        previousInterval.OnHitDelay = gameTime.TotalGameTime;
                        player.YActive = true;

                        // enemy crashed, no points
                        enemies[i].Crashed();
                        player.Health--;
                    }
                }
            }
        }

        private void CheckPlayerDropCollision(GameTime gameTime)
        {
            for (int i = 0; i < drops.Count; i++)
            {
                if (player.Area.Intersects(drops[i].Area))
                {
                    if (drops[i].Active)
                    {
                        drops[i].Active = false;

                        if (drops[i] is Unit_Drop_ChocolateBall)
                        {
                            projectileLevel++;
                            projectileLevel = (int)MathHelper.Clamp(projectileLevel, 0, Settings.Projectile_MaxLevel);
                        }
                        else if (drops[i] is Unit_Drop_PowerUp)
                        {
                            currentPowerUp.NewPowerUp(gameTime, ((Unit_Drop_PowerUp)drops[i]).PowerUp);
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------

        private void DrawBackgrounds(GameTime gameTime)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].Draw(gameTime);
            }
        }

        private void DrawUnitsList<unitType>(GameTime gameTime, IList<unitType> units) where unitType : Unit
        {
            foreach (Unit unit in units)
            {
                unit.Draw(gameTime);
            }
        }

        private void DrawInfo()
        {
            string infoTitle = "";
            int infoValue = 0;
            string info;

            // loop through each info to display
            for (int i = 0; i <= 3; i++)
            {
                switch (i)
                {
                    case 0:
                        infoTitle = Settings.InfoTitle_Score;
                        infoValue = score;
                        break;

                    case 1:
                        infoTitle = Settings.InfoTitle_Health;
                        infoValue = player.Health;
                        break;

                    case 2:
                        infoTitle = Settings.InfoTitle_Level;
                        infoValue = gameLevel;
                        break;

                    case 3:
                        infoTitle = Settings.InfoTitle_Projectile;
                        infoValue = projectileLevel;
                        break;
                }

                info = infoTitle + ": " + infoValue;
                spriteBatch.DrawString(spriteFont, info, infoPosition, Settings.InfoTitle_Colour);

                // shift next position right of the current info
                infoPosition.X += spriteFont.MeasureString(info).X + Settings.InfoTitle_Padding;
            }

            // display current powerup if any
            if (currentPowerUp.Active)
            {
                spriteBatch.DrawString(spriteFont, currentPowerUp.ToString(), infoPosition, currentPowerUp.InfoColour());
            }

            infoPosition = infoDefaultPosition;
        }

        //------------------------------------------------------------------------------------

        private bool KeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        private bool UniqueKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }
    }
}
