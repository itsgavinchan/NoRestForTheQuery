using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace noRestForTheQuery
{
    public class FinalGame : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public const int WINDOW_WIDTH = 1200;
        public const int WINDOW_HEIGHT = 600;
        public const float GRAVITY = 0.9F;
        const int MAXHOMEWORK = 5;
        const int MAXLEVELS = 2;
        const int INVUL_TIME = 1500;
        const int BLINK_TIME = 100;
        const int SANITY_TIME = 600;
        const int PROFESSOR_TIME = 9000;
        int mouseX, mouseY, hwKilled;

        enum ScreenStatus { START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY };
        int currentStatus = (int) ScreenStatus.START;

        // STATSBAR
        Texture2D statSprite, idSprite, filler;
        Vector2 statsPos = Vector2.Zero;
        Vector2 levelPos = new Vector2(105, 90);
        Vector2 budgetPos = new Vector2(255, 92);
        Vector2 ammoPos = new Vector2(462, 92);
        Vector2 shieldPos = new Vector2(628, 92);
        Vector2 healthPos = new Vector2(161, 7);
        Vector2 sanityPos = new Vector2(148, 30);
        Vector2 expPos = new Vector2(135, 52);
        Color healthColor = new Color(255, 192, 0);
        Color sanityColor = new Color(0, 114, 255);
        Color expColor = new Color(60, 184, 120);

        // START SCREEN
        const string gameTitle = "No Rest For The Query";
        const string continueMessage = "Press Space To Continue";
        Vector2 gameTitlePos, continueMessagePos;

        // INTRODUCTION SCREEN
        string[] introMessage = new string[] { 
            "Congratulations. Starting today, you're a CS student at NYU-Poly.",
            "In the incoming years, become the masochistic programmer you are ",
            "paying tuition to be and get ready to sweat blood. Good luck, young",
            "programmer. May you survive this hurdle intact and graduate (on time).",
            ""
            };

        // GAME OVER SCREEN
        const string gameOverMessage = "Game Over";
        const string gameOverContMessage = "Press R to Restart The Game";
        Vector2 gameOverPos, gameOverContPos;
        
        // WEEKEND STANDBY SCREEN
        // You may only choose three choices of the total of SIX options - BEG, SLEEP, FOOD, SANITY, STUDY, STORE
        const int numOfWeekendOptions = 6, weekendMargin = 50, maxChoices = 3;
        enum WeekendChoices { BEG, SLEEP, FOOD, RELAX, STUDY, STORE };

        List<int> chosenChoices = new List<int>(3);
        //Texture2D[] weekendIcons = new Texture2D[numOfWeekendOptions];
        Vector2[] weekendPositions = new Vector2[numOfWeekendOptions];
        Texture2D weekEndBGSprite, filledBulletSprite, plansSprite, listSprite, submitSprite;

        // STORE Option Specific
        public static int SHIELDCOST = 100;
        public static int SLEEPINCREMENT = 50;
        public static int BULKBUY = 50;
        public static double PENCILCOST = 20.0/BULKBUY;
        int pencilPurchasing = 0, costOfShield, costOfPencils;
        bool shieldCheck = false, staticScreenTrigger = false;
        Vector2 pencilOption = new Vector2( 790, 240 );
        Vector2 shieldOption = new Vector2(790, 363);
        Vector2 minusPos = new Vector2(795, 275);
        Vector2 plusPos = new Vector2(1010, 275);
        Vector2 submitPos = new Vector2(900, 475);
        Vector2 quantityPos = new Vector2(850, 275);
        Vector2 remainingPos = new Vector2(910, 140); 
        Vector2 spendingPos = new Vector2(960, 180);
        Color b80000 = new Color(184, 0, 0);

        // BATTLE/WEEKDAY SCREEN
        // GAME TOOLS
        public static int studentWidth = 50;
        public static int studentHeight = 65;
        public static int defaultBlockWidth = studentWidth;     //To make level design easier
        public static int defaultBlockHeight = studentHeight;   //Student assured to fit in spaces
        public static int gameLevel = 1;
        public static Random rand = new Random();
        public Rectangle goal = new Rectangle();
        string currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";

        // CAMERA
        public static int screenOffset = 0, tempScreenOffset;
        public static Matrix translation = Matrix.Identity;

        // STUDENT
        Student1 student;
        AnimatedSprite studentAnimation;
        int hitRecoilTime = INVUL_TIME;
        int blinkDuration = BLINK_TIME;
        int sanityTime = SANITY_TIME;
        int sanityBlockade = 0;
        int professorTime = PROFESSOR_TIME * gameLevel;
        const double SANITYTRIGGER = 0.5;
        Vector2 blockadePos;
        bool lostHealth = false; //Needed in order to decrement health only once after being hit.

        // PROFESSORS
        List<Professor> professors = new List<Professor>();

        // PLATFORMS
        List<Platform> platforms = new List<Platform>();
        List<Platform> hiddenPlatforms = new List<Platform>();
        List<Platform> triggers = new List<Platform>();

        // HOMEWORKS
        List<Homework> homeworks = new List<Homework>();
        List<Exam> exams = new List<Exam>();

        // DISPLAY Variables
        SpriteFont mainFont, largeFont;
        public static Texture2D platformSprite, studentSprite, pencilSprite, markerSprite, homeworkSprite, examSprite, notebookSprite, professorSprite, blockadeSprite;
        
        // INPUT States
        KeyboardState lastKeyState = Keyboard.GetState();
        MouseState lastMouseState = Mouse.GetState();

        public FinalGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            IsMouseVisible = true;
        }
        protected override void Initialize() { base.Initialize(); }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>(@"Fonts/MainFont");
            largeFont = Content.Load<SpriteFont>(@"Fonts/LargeFont");

            studentSprite = Content.Load<Texture2D>(@"Sprites/player");
            platformSprite = Content.Load<Texture2D>(@"Sprites/platform");
            pencilSprite = Content.Load<Texture2D>(@"Sprites/pencil");
            markerSprite = Content.Load<Texture2D>(@"Sprites/marker");
            homeworkSprite = Content.Load<Texture2D>(@"Sprites/homework");
            examSprite = Content.Load<Texture2D>(@"Sprites/final");
            notebookSprite = Content.Load<Texture2D>(@"Sprites/notebook");
            professorSprite = Content.Load<Texture2D>(@"Sprites/professor");
            blockadeSprite = Content.Load<Texture2D>(@"Sprites/blockade");

            filler = Content.Load<Texture2D>(@"Sprites/filler");
            statSprite = Content.Load<Texture2D>(@"Sprites/statsbar");
            idSprite = Content.Load<Texture2D>(@"Sprites/identification");

            weekEndBGSprite = Content.Load<Texture2D>(@"Sprites/weekend");
            plansSprite = Content.Load<Texture2D>(@"Sprites/plans");
            listSprite = Content.Load<Texture2D>(@"Sprites/shoppingList");
            submitSprite = Content.Load<Texture2D>(@"Sprites/submit");
            filledBulletSprite = Content.Load<Texture2D>(@"Sprites/filled");

            blockadePos = new Vector2(WINDOW_WIDTH - sanityBlockade, 0);
            gameTitlePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameTitle).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameTitle).Y / 2 - 25);
            continueMessagePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(continueMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(continueMessage).Y / 2 + 25);
            gameOverPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverMessage).Y / 2 - 25);
            gameOverContPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverContMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverContMessage).Y / 2 + 25);
            
            // Temporary Signal for Selection of Options
            //int sum = 0;
            //for (int i = 0; i < weekendIcons.Count(); i++) { sum += weekendIcons[i].Width; }
            //float itemMargin = ( (WINDOW_WIDTH - weekendMargin * 2) - sum) / weekendIcons.Count();
            for( int i = 0; i < numOfWeekendOptions; i++ ) {
                weekendPositions[i] = new Vector2(160, 198 + 57*i);
            }

            // Animation sheet
            studentAnimation = new AnimatedSprite( Content.Load<Texture2D>(@"Sprites/playerSheet"), 
                                                   5, studentWidth, studentHeight );

            // Student starts in the top center of the screen for testing. Otherwise, it follows the level plan
            student = new Student1( ref studentAnimation,
                                    new Vector2(WINDOW_WIDTH / 2 - studentWidth / 2, 200 ), // Position
                                    new Vector2(studentWidth / 2, studentHeight / 2),       // Origin
                                    Vector2.Zero, 3.5F); 
            student.colorArr = new Color[ studentAnimation.Texture.Width * studentAnimation.Texture.Height ];
            studentAnimation.Texture.GetData<Color>( 0, studentAnimation.SourceRect, student.colorArr,
                                                     student.sprite.currentFrame*studentWidth, 
                                                     studentWidth * studentHeight );
            

            // INITIATE - Professors of All Levels; Temporary Implentation For Testing
            for (int i = 0; i < MAXLEVELS; i++) {
                professors.Add(new Professor(   (i+1) * 5,
                                                new Vector2(WINDOW_WIDTH + screenOffset + professorSprite.Width, (WINDOW_HEIGHT - professorSprite.Height) / 2),
                                                new Vector2(professorSprite.Width / 2, professorSprite.Height / 2),
                                                Vector2.Zero,
                                                1));
                professors[i].colorArr = new Color[ professorSprite.Width * professorSprite.Height ];
                professorSprite.GetData<Color>(professors[i].colorArr);
            }

            //Build the first level
            buildLevel( ref platforms, ref student, ref homeworks );

        }
        protected override void UnloadContent() { }
        public void updatePosition() {
            if (currentStatus == (int)ScreenStatus.WEEKDAY) {
                // Update Position to Keep Still - StatsBar
                levelPos.X = 105 + screenOffset;
                budgetPos.X = 255 + screenOffset;
                ammoPos.X = 462 + screenOffset;
                shieldPos.X = 628 + screenOffset;
                healthPos.X = 161 + screenOffset;
                sanityPos.X = 148 + screenOffset;
                expPos.X = 135 + screenOffset;
                statsPos.X = screenOffset;
                blockadePos.X = WINDOW_WIDTH - sanityBlockade + screenOffset;
            }
            else if (currentStatus == (int)ScreenStatus.WEEKEND && staticScreenTrigger) {
                for (int i = 0; i < weekendPositions.Count(); i++) {
                    weekendPositions[i].X = 160 + screenOffset;
                }
                quantityPos.X = 850 + screenOffset;
                pencilOption.X = 790 + screenOffset;
                shieldOption.X = 790 + screenOffset;
                minusPos.X = 795 + screenOffset;
                plusPos.X = 1010 + screenOffset;
                submitPos.X = 780 + screenOffset;
                remainingPos.X = 910 + screenOffset;
                spendingPos.X = 960 + screenOffset;
            }
            else if (currentStatus == (int)ScreenStatus.GAMEOVER && staticScreenTrigger) {
                gameOverPos.X += screenOffset;
                gameOverContPos.X += screenOffset;
            }
        }
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            mouseX = Mouse.GetState().X + screenOffset; 
            mouseY = Mouse.GetState().Y;

            // Implements the different screen stages -  START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY

            // START SCREEN - Saves Not Implemented Yet
            // Pressing Space will proceed to Introduction Screen
            if (IsActive && currentStatus == (int)ScreenStatus.START){
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.INTRODUCTION; }
            }
            // INTRODUCTION SCREEN - Explains the Story and Rules of Game
            // Pressing Space will proceed to the Start of Game - Weekday Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.INTRODUCTION) {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.WEEKDAY; }
            }
            // PAUSE SCREEN - Provides option to pause the screen; may implement SAVE, QUIT GAME, RESTART LEVEL in future iterations
            // Press Backspace to resume the game in Weekday Stage (Pause Stage can only be accessed from Weekday Stage)
            else if (IsActive && currentStatus == (int)ScreenStatus.PAUSE) {
                if (staticScreenTrigger) staticScreenTrigger = false;
                if (Keyboard.GetState().IsKeyDown(Keys.Back) && lastKeyState.IsKeyUp(Keys.Back)) { currentStatus = (int)ScreenStatus.WEEKDAY; }
            }
            // STATUS SCREEN - Displays the status of the player; not yet implemented; a testing screen to be removed at final implementation
            // Pressing Space will proceed back to the Weekend Stage (Status Stage can only be accessed from Weekend Stage)
            else if (IsActive && currentStatus == (int)ScreenStatus.STATUS) {
                if (staticScreenTrigger) staticScreenTrigger = false;
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.WEEKEND; }
            }
            // GAME OVER SCREEN
            // Pressing R Key will restart the game from the beginning - Start Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.GAMEOVER) {
                if (staticScreenTrigger) staticScreenTrigger = false;
                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); currentStatus = (int)ScreenStatus.START; }
            }
            // WEEKEND SCREEN
            // Left-Clicking will allow you to choose three of the five options - SLEEP, STUDY, FOOD, STORE, RELAX=
            // Pressing Enter will execute your choices and provide you the boosts, but only if you select the maximum amount of choices
                // If you attempt to choose one more than the maximum amount of allowed choices, the first choice will be removed and 
                // replaced with the latest choice. This will not give the player an option to choose less or more than the maximum amount 
                // of allowed choices.
            // Pressing Space will lead you to the Status Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKEND) {
                if (staticScreenTrigger) staticScreenTrigger = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.STATUS; }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) {

                    for (int i = 0; i < numOfWeekendOptions; i++) {
                        if (checkMouseOverlap(weekendPositions[i], filledBulletSprite.Width, filledBulletSprite.Height)) {
                            if (chosenChoices.Count() >= maxChoices && !chosenChoices.Contains(i)) { chosenChoices.RemoveAt(0); chosenChoices.Add(i); }
                            else if (chosenChoices.Contains(i) ) { chosenChoices.RemoveAt(chosenChoices.IndexOf(i)); }
                            else if ( !chosenChoices.Contains(i) ) { chosenChoices.Add(i); }
                        }
                    }

                    if (chosenChoices.Contains((int)WeekendChoices.STORE)) {
                        if (checkMouseOverlap(shieldOption, filledBulletSprite.Width, filledBulletSprite.Height)) {
                            if (!shieldCheck) costOfShield = 0;
                            shieldCheck = !shieldCheck;
                        }


                            if (checkMouseOverlap(minusPos, 25, 25) && pencilPurchasing > 0 ) { pencilPurchasing -= BULKBUY; }
                            else if(checkMouseOverlap(plusPos, 25, 25)) { 
                                if( !shieldCheck && costOfPencils < student.budget ) pencilPurchasing += BULKBUY; 
                                else if( shieldCheck && costOfPencils + costOfShield < student.budget ){ pencilPurchasing += BULKBUY;  }
                            }
                        
                        if (shieldCheck && costOfPencils + costOfShield > student.budget) {
                                pencilPurchasing = (int)((student.budget - costOfShield) / PENCILCOST);
                        }
                        
                    }

                    if( chosenChoices.Count() == 3 && checkMouseOverlap( submitPos, 250, 35 ) ) {
                        for (int i = 0; i < chosenChoices.Count(); i++) {
                            if (chosenChoices[i] == (int)WeekendChoices.BEG) { student.budget+= 500; }
                            else if (chosenChoices[i] == (int)WeekendChoices.SLEEP) { student.sleepEffect(); }
                            else if (chosenChoices[i] == (int)WeekendChoices.STUDY) { student.studyEffect(); }
                            else if (chosenChoices[i] == (int)WeekendChoices.FOOD) { student.foodEffect(); }
                            else if (chosenChoices[i] == (int)WeekendChoices.RELAX) { student.socialEffect(); }
                            else if (chosenChoices[i] == (int)WeekendChoices.STORE) { 
                                student.amtPencil += pencilPurchasing;
                                student.budget -= (int)(pencilPurchasing * PENCILCOST);
                                if( shieldCheck ){
                                    student.budget -= SHIELDCOST* (student.notebook.maxBooks - student.notebook.numOfNotebook);
                                    student.notebook.reset();
                                }
                            }
                        }
                        shieldCheck = false;
                        chosenChoices.Clear();
                        pencilPurchasing = 0;
                        costOfPencils = 0;
                        costOfShield = 0;
                        currentStatus = (int)ScreenStatus.WEEKDAY; 
                    }

                    if( shieldCheck ){ costOfShield = SHIELDCOST * (student.notebook.maxBooks - student.notebook.numOfNotebook); }
                    costOfPencils = (int)(pencilPurchasing * PENCILCOST);
                }
            }
            // WEEKDAY SCREEN - Where the Action Happens and You Die (A Lot); Currently In Testing
            // While the game is active and in the Weekday Stage, homework will continually be spawned
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKDAY) {

                sanityTime -= gameTime.ElapsedGameTime.Milliseconds;
                // if (sanityTime % 6 == 0 && student.sanity <= SANITYTRIGGER && sanityBlockade < blockadeSprite.Width ) sanityBlockade+=3;
                if (sanityTime < 0 ) {
                    sanityTime = SANITY_TIME;
                    student.sanity -= 0.005;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); }

                // TEST - Initiate GAMEOVER Stage; CHECK DEATH - Student dies if goes off-screen to the left or jumps off a platform
                if ( student.currentHealth <= 0 || Keyboard.GetState().IsKeyDown(Keys.Q) || (student.isAlive && (student.position.X < screenOffset - studentSprite.Width * 2 || student.position.Y > WINDOW_HEIGHT + studentSprite.Height))) {
                    staticScreenTrigger = true;
                    student.isAlive = false; 
                    currentStatus = (int)ScreenStatus.GAMEOVER; 
                }

                // TEST - Initiate WEEKEND Stage
                if (Keyboard.GetState().IsKeyDown(Keys.U)) {
                    staticScreenTrigger = true;
                    // Transitions to WEEKEND Stage
                    currentStatus = (int)ScreenStatus.WEEKEND;
                }

                // TEST - Initiate PAUSE Stage
                if (Keyboard.GetState().IsKeyDown(Keys.Back) && lastKeyState.IsKeyUp(Keys.Back)) {
                    currentStatus = (int)ScreenStatus.PAUSE;
                }

                // PLAYER CONTROL - Jump
                if (Keyboard.GetState().IsKeyDown(Keys.W) && lastKeyState.IsKeyUp(Keys.W) && !student.jumping && student.onGround) { student.jump(); }

                // PLAYER CONTROL - Move Left
                if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                    if (!student.checkBoundaries()) { student.velocity.X = -student.speed; }
                    student.sprite.animateLeft(Keyboard.GetState(), lastKeyState, gameTime);
                }

                // PLAYER CONTROL - Move Right
                if (Keyboard.GetState().IsKeyDown(Keys.D) && student.position.X <= (WINDOW_WIDTH + screenOffset) - sanityBlockade * 0.5) {
                    if (!student.checkBoundaries()) { student.velocity.X = student.speed; }
                    student.sprite.animateRight(Keyboard.GetState(), lastKeyState, gameTime);
                }

                // PLAYER CONTROL - Attack with Ammo (Pencils)
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) {
                    double x = (mouseX - (student.position.X + student.sprite.width / 2));
                    double y = (mouseY - (student.position.Y + student.sprite.height / 2));
                    student.shoot((float)(Math.Atan2(y, x)));
                    if( x <= 0 ){ student.sprite.currentFrame = 4; }
                    else{ student.sprite.currentFrame = 0; }
                }

                // TEST - Professor Appearance
                if (Keyboard.GetState().IsKeyDown(Keys.P) && lastKeyState.IsKeyUp(Keys.P)) {
                    if (!professors[gameLevel - 1].isAlive) { professors[gameLevel - 1].isAlive = !professors[gameLevel - 1].isAlive; }
                    else { professors[gameLevel - 1].reset(); }
                }

                if (!professors[gameLevel - 1].isAlive) {
                    for (int i = 0; i < triggers.Count(); i++) {
                        if (checkOverlap(student.position, studentSprite.Width, studentSprite.Height, triggers[i].position, platformSprite.Width, platformSprite.Height)) {
                            professors[gameLevel - 1].isAlive = true;
                        }
                    }
                }
                else {
                    professorTime -= gameTime.ElapsedGameTime.Milliseconds;
                    // if (sanityTime % 6 == 0 && student.sanity <= SANITYTRIGGER && sanityBlockade < blockadeSprite.Width ) sanityBlockade+=3;
                    if (professorTime % 600 == 0) {
                        double x = ((student.position.X + studentSprite.Width / 2) - (professors[gameLevel - 1].position.X + professorSprite.Width / 2));
                        double y = ((student.position.Y + studentSprite.Height / 2) - (professors[gameLevel - 1].position.Y + professorSprite.Height / 2));
                        professors[gameLevel - 1].shoot(x, y);
                    }

                    if (professorTime < 0) {
                        professors[gameLevel - 1].isAlive = false;
                        professorTime = PROFESSOR_TIME * gameLevel;
                        student.sanity -= 0.005;
                    }
                    
                
                }
                // TEST - Professor Attack
                if (Keyboard.GetState().IsKeyDown(Keys.L) && lastKeyState.IsKeyUp(Keys.L)) {
                    if (professors[gameLevel - 1].isAlive) {
                        double x = ((student.position.X + studentSprite.Width / 2) - (professors[gameLevel - 1].position.X + professorSprite.Width / 2));
                        double y = ((student.position.Y + studentSprite.Height / 2) - (professors[gameLevel - 1].position.Y + professorSprite.Height / 2));
                        professors[gameLevel - 1].shoot(x, y);
                    }
                }

                // TEST - Progression to Next Level
                if (Keyboard.GetState().IsKeyDown(Keys.N) && lastKeyState.IsKeyUp(Keys.N)) {
                    reset();
                    if (gameLevel < MAXLEVELS) gameLevel++;
                    currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";
                    buildLevel(ref platforms, ref student, ref homeworks);
                }

                // TEST - Controls for Camera Movement
                if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                    translation *= Matrix.CreateTranslation(new Vector3(1, 0, 0));
                    screenOffset -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                    translation *= Matrix.CreateTranslation(new Vector3(-1, 0, 0));
                    screenOffset += 1;
                }

                // POSITION UPDATE - Student
                student.update();

                int index = 0;
                // POSITION UPDATE - Student's Ammo (Pencils)
                index = 0;
                while (index < student.pencils.Count()) {
                    student.pencils[index].update();
                    if (student.pencils[index].checkBoundaries(pencilSprite.Width, pencilSprite.Height)) { student.pencils.RemoveAt(index); }
                    else { index++; }
                }

                // POSITION UPDATE - Professor and Their Ammo (Markers)
                //if (professors[gameLevel - 1].isAlive) { 
                index = 0;
                professors[gameLevel - 1].update();
                while (index < professors[gameLevel - 1].markers.Count()) {
                    professors[gameLevel - 1].markers[index].update(student.position.X + studentSprite.Width / 2, student.position.Y + studentSprite.Height / 2);
                    if (professors[gameLevel - 1].markers[index].checkBoundaries(markerSprite.Width, markerSprite.Height)) { professors[gameLevel - 1].markers.RemoveAt(index); }
                    else { index++; }
                }
                //}

                // POSITION AND COLLISION UPDATE - Homeworks
                index = 0;
                while (index < homeworks.Count()) {
                    homeworks[index].update( student.position.X+student.origin.X, student.position.Y+student.origin.Y );

                    int i = 0;
                    while (i < student.pencils.Count() ) {
                        homeworks[index].handleCollision(student.pencils[i], pencilSprite.Width, pencilSprite.Height,
                                                homeworkSprite.Width, homeworkSprite.Height);
                        if (homeworks[index].hit) { break; }
                        i++;
                    }

                    if (homeworks[index].hit) { 
                        homeworks[index].decrementHealth( student.attackPower );
                        student.pencils.RemoveAt(i);
                        homeworks[index].hit = false;
                    }

                    student.handleCollision(homeworks[index], homeworkSprite.Width, homeworkSprite.Height,
                                                studentSprite.Width, studentSprite.Height);

                    if (student.hit) {
                        if (!lostHealth) {
                            if (student.notebook.numOfNotebook > 0) { student.notebook.isDamaged(); }
                            else { student.decrementHealth( homeworks[index].attackPower );  }
                            homeworks[index].isAlive = false;
                            lostHealth = true; //lostHealth is set to false in handleInvulTime function
                        }
                    }

                    //if (homeworks[index].checkBoundaries(homeworkSprite.Width, homeworkSprite.Height) ) { homeworks.RemoveAt(index); }
                    if (!homeworks[index].isAlive) { homeworks.RemoveAt(index); student.gainExperience(); hwKilled++;  }
                    else { index++; }
                }

                // POSITION AND COLLISION UPDATE - Exams
                index = 0;
                while (index < exams.Count()) {
                    exams[index].update(student.position.X + student.origin.X, student.position.Y + student.origin.Y);

                    int i = 0;
                    while (i < student.pencils.Count()) {
                        exams[index].handleCollision(student.pencils[i], pencilSprite.Width, pencilSprite.Height,
                                                examSprite.Width, examSprite.Height);
                        if (exams[index].hit) { break; }
                        i++;
                    }

                    if (exams[index].hit) {
                        exams[index].decrementHealth(student.attackPower);
                        student.pencils.RemoveAt(i);
                        exams[index].hit = false;
                    }

                    student.handleCollision(exams[index], examSprite.Width, examSprite.Height,
                                                studentSprite.Width, studentSprite.Height);

                    if (student.hit) {
                        if (!lostHealth) {
                            if (student.notebook.numOfNotebook > 0) { student.notebook.isDamaged(); }
                            else { student.decrementHealth( exams[index].attackPower ); }
                            exams[index].durability--;
                            lostHealth = true; //lostHealth is set to false in handleInvulTime function
                        }
                    }

                    if (exams[index].currentHealth <= 0 || exams[index].durability == 0) { exams[index].isAlive = false; }
                    //if (exams[index].checkBoundaries(examsprite.Width, examsprite.Height) ) { exams.RemoveAt(index); }
                    if (!exams[index].isAlive) {
                        
                            exams.RemoveAt(index); 
                            student.gainExperience();
                        
                    }
                    else { index++; }
                }

                // POSITION UPDATE - Camera
                translation *= Matrix.CreateTranslation(new Vector3(-1, 0, 0));
                screenOffset += 1;
                
                // COLLISION UPDATE - Check if student hit by marker, but check first if professor is present and if they even have markers
                if (professors[gameLevel - 1].isAlive /*&& professors[gameLevel - 1].markers.Count() > 0 */) { 
                    index = 0;

                    while ( index < professors[gameLevel - 1].markers.Count()) {
                        student.handleCollision( professors[gameLevel-1].markers[index], markerSprite.Width, markerSprite.Height, 
                                                 studentSprite.Width, studentSprite.Height );
                        if( student.hit ){ break; }
                        index++;
                    }

                    if( student.hit ){
                        if (!lostHealth) {
                            if (student.notebook.numOfNotebook > 0) { student.notebook.isDamaged(); }
                            else { student.decrementHealth( professors[gameLevel-1].attackPower );  }
                            lostHealth = true; //lostHealth is set to false in handleInvulTime function
                        }
                    }
                }


                    // BOOK-KEEPING
                    updatePosition();
                handleSpriteMovement(ref student.sprite);
                handleStudentPlatformCollision( platforms );                               //Handle student/platform collision
                handleStudentPlatformCollision( hiddenPlatforms);

                if (student.velocity.Y != 0) { student.onGround = false; }      //Check if on ground
                if (student.onGround) { student.jumping = false; }              //Reset jump state
                if (student.hit){ handleInvulTime( gameTime ); }                //If hit, countdown invul time
                checkForVictory();

                if (lastKeyState.IsKeyUp(Keys.Left) || lastKeyState.IsKeyUp(Keys.Right)) { student.velocity.X = 0; }

            }

            lastKeyState = Keyboard.GetState();
            lastMouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, translation);

            // Displays the different screen stages - START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY

            // START SCREEN
            // Displays the Game Title and Continue Message
            if (IsActive && currentStatus == (int)ScreenStatus.START) {
                spriteBatch.DrawString(mainFont, gameTitle, gameTitlePos, Color.White);
                spriteBatch.DrawString(mainFont, continueMessage, continueMessagePos, Color.White);
            }

            // INTRODUCTION SCREEN
            // Displays the Welcoming Message to Greet the Player as well as brief them on the controls
            else if (IsActive && currentStatus == (int)ScreenStatus.INTRODUCTION) {
                for (int i = 0; i < introMessage.Count(); i++) {
                    spriteBatch.DrawString(mainFont, introMessage[i], new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(introMessage[i]).X / 2, i * 25 + 100), Color.White);
                }
            }

            // STATUS SCREEN - In development; May Not Implement (For Testing Purposes)
            else if (IsActive && (currentStatus == (int)ScreenStatus.STATUS || currentStatus == (int)ScreenStatus.PAUSE ) ) { 
                spriteBatch.Draw(weekEndBGSprite, statsPos, Color.White);
                int ypos = 0, yAmt = 30;
                spriteBatch.DrawString(largeFont, "Health: " + student.currentHealth + "/" + student.fullHealth, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Shield: " + student.notebook.numOfNotebook, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Ammo Held: " + student.amtPencil, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Ammo On Screen: " + student.pencils.Count(), new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Homework Killed: " + hwKilled, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Experience: " + (student.experience * 100) + "/100", new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Attack Power: " + student.attackPower, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Budget: $" + student.budget, new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                spriteBatch.DrawString(largeFont, "Sanity: " + (student.sanity * 100) + "/100", new Vector2(screenOffset + 120, 100 + (ypos++) * yAmt), b80000);
                
            }

            // GAMEOVER SCREEN
            else if (IsActive && currentStatus == (int)ScreenStatus.GAMEOVER) {
                spriteBatch.DrawString(mainFont, gameOverMessage, gameOverPos, Color.White);
                spriteBatch.DrawString(mainFont, gameOverContMessage, gameOverContPos, Color.White);
            }

            // WEEKEND SCREEN
            // Temporary implementation; actual UI will be different. Selected choices will be brown and unselected choices will be red. 
            // Hovering over a choice will show the choice as black. 
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKEND) {
                spriteBatch.Draw(weekEndBGSprite, statsPos, Color.White);
                spriteBatch.Draw(plansSprite, statsPos, Color.White);
                if (chosenChoices.Contains((int)WeekendChoices.STORE)) {
                    spriteBatch.Draw(listSprite, statsPos, Color.White);
                    spriteBatch.DrawString(mainFont, pencilPurchasing + " for $" + PENCILCOST * pencilPurchasing, quantityPos, Color.White);
                    spriteBatch.DrawString(largeFont, "$" + (student.budget - costOfShield - costOfPencils), remainingPos, b80000);
                    spriteBatch.DrawString(largeFont, "$" + (costOfShield + costOfPencils), spendingPos, b80000);
                }
                for (int i = 0; i < numOfWeekendOptions; i++) {
                    if (chosenChoices.Contains(i)) { spriteBatch.Draw(filledBulletSprite, weekendPositions[i], Color.White); }
                }
                if (checkMouseOverlap(shieldOption, filledBulletSprite.Width, filledBulletSprite.Height ) || shieldCheck) { spriteBatch.Draw(filledBulletSprite, shieldOption, Color.White); }
                if (chosenChoices.Count() == maxChoices) { spriteBatch.Draw(submitSprite, statsPos, Color.White); }
            }
            // WEEKDAY SCREEN
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKDAY) {

                // Platform Display
                for (int i = 0; i < platforms.Count; ++i) { spriteBatch.Draw(platformSprite, platforms[i].position, platforms[i].rectangle, Color.Black); }
                
                // HIDDEN PLATFORMS Display
                for (int i = 0; i < hiddenPlatforms.Count; ++i) { spriteBatch.Draw(platformSprite, hiddenPlatforms[i].position, hiddenPlatforms[i].rectangle, Color.LightGray); }
                
                // TRIGGERS
                for (int i = 0; i < triggers.Count; ++i) { spriteBatch.Draw(platformSprite, triggers[i].position, triggers[i].rectangle, Color.White); }
                
                //Goal Display
                spriteBatch.Draw( platformSprite, goal, Color.White );

                // Homework Display
                for (int i = 0; i < homeworks.Count(); i++) { 
                    spriteBatch.Draw(homeworkSprite, homeworks[i].position, Color.Orange);
                    spriteBatch.DrawString(mainFont, "" + homeworks[i].currentHealth, homeworks[i].position, Color.Black);
                }

                for (int i = 0; i < exams.Count(); i++) {
                    spriteBatch.Draw(examSprite, exams[i].position, Color.Beige);
                    spriteBatch.DrawString(mainFont, "" + exams[i].currentHealth, exams[i].position, Color.Black);
                }

                // Ammo (Pencil) Display
                for (int i = 0; i < student.pencils.Count(); i++) { spriteBatch.Draw(pencilSprite, student.pencils[i].position, null, Color.White, student.pencils[i].rotation, student.pencils[i].origin, 1.0F, SpriteEffects.None, 0.0F); }
                
                // Professor Display and their Markers
                if (professors[gameLevel - 1].isAlive) {
                    spriteBatch.Draw(homeworkSprite, professors[gameLevel - 1].position, Color.Blue);
                    for (int i = 0; i < professors[gameLevel - 1].markers.Count(); i++) {
                        spriteBatch.Draw(markerSprite, professors[gameLevel - 1].markers[i].position, null, Color.White, professors[gameLevel - 1].markers[i].rotation, professors[gameLevel - 1].markers[i].origin, 1.0F, SpriteEffects.None, 0.0F);
                    }
                }
                
                // If the student is not hit, draw the student and the notebook shield as usual
                if (!student.hit) {
                    spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.White);
                    if (student.notebook.isAlive) { spriteBatch.Draw(notebookSprite, student.notebook.position, null, Color.Red, student.notebook.rotation, student.notebook.origin, 1.0F, SpriteEffects.None, 0.0F); }
                }
                else {

                    // If the notebook shield still holds, blink the notebook shield
                    if (student.notebook.isAlive) {
                        blinkDuration -= gameTime.ElapsedGameTime.Milliseconds;
                        if (blinkDuration < 0) {
                            spriteBatch.Draw(notebookSprite, student.notebook.position, null, Color.Red, student.notebook.rotation, student.notebook.origin, 1.0F, SpriteEffects.None, 0.0F);
                            blinkDuration = BLINK_TIME;
                        }
                        spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.White);
                    }

                    // Otherwise, if the shield is destroyed, blink the student instead
                    else {
                        blinkDuration -= gameTime.ElapsedGameTime.Milliseconds;
                        if (blinkDuration < 0) {
                            spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.White); 
                            blinkDuration = BLINK_TIME;
                        }
                    }
                }

                // SANITY CONTROL
                //if (student.sanity <= SANITYTRIGGER) {
                    spriteBatch.Draw(blockadeSprite, blockadePos, Color.White);
                //}
                
                // Display stats
                spriteBatch.Draw(statSprite, statsPos, Color.White);
                spriteBatch.Draw(filler, new Rectangle((int)healthPos.X, (int)healthPos.Y, (int)(((float)student.currentHealth / student.fullHealth) * 450), 17), healthColor);
                spriteBatch.Draw(filler, new Rectangle((int)sanityPos.X, (int)sanityPos.Y, (int)(student.sanity * 450), 17), sanityColor);
                spriteBatch.Draw(filler, new Rectangle((int)expPos.X, (int)expPos.Y, (int)(student.experience * 450), 17), expColor);
                spriteBatch.Draw(idSprite, statsPos, Color.White);
                spriteBatch.DrawString(mainFont, "$" + student.budget, budgetPos, Color.Black);
                spriteBatch.DrawString(mainFont, "" + student.amtPencil, ammoPos, Color.Black);
                spriteBatch.DrawString(mainFont, "" + student.notebook.numOfNotebook, shieldPos, Color.Black);
                if (gameLevel == 1) spriteBatch.DrawString(mainFont, "Fr", levelPos, Color.Black);
                else if (gameLevel == 2) spriteBatch.DrawString(mainFont, "So", levelPos, Color.Black);
                else if (gameLevel == 3) spriteBatch.DrawString(mainFont, "Jr", levelPos, Color.Black);
                else if (gameLevel == 4) spriteBatch.DrawString(mainFont, "Sr", levelPos, Color.Black);
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool checkMouseOverlap(Vector2 position, int width, int height) {
            if (mouseX > position.X && mouseX < position.X + width && mouseY > position.Y && mouseY < position.Y + height) { return true; }
            else { return false; }
        }
        public bool checkOverlap(Vector2 o1Pos, int o1W, int o1H, Vector2 o2Pos, int o2W, int o2H) {
            Rectangle o1 = new Rectangle((int)o1Pos.X, (int)o1Pos.Y, o1W, o1H);
            Rectangle o2 = new Rectangle((int)o2Pos.X, (int)o2Pos.Y, o2W, o2H);

            if (o1.Intersects(o2)) { return true; }
            else { return false; }
        }
        public bool purchasePencils(int quantity) {
            if (quantity * PENCILCOST > student.budget) { return false; }
            else { student.amtPencil += quantity; student.budget -= (int)(quantity * PENCILCOST); return true; }
        }
        protected void reset() {
            // Reset The Objects (Or Clear the Lists)
            homeworks.Clear();
            exams.Clear();
            professors[gameLevel - 1].reset();
            student.reset();

            // Reset The Game Mechaics
            gameLevel = 1;
            currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";
            buildLevel(ref platforms, ref student, ref homeworks); 
            screenOffset = 0;
            translation = Matrix.Identity;

            hitRecoilTime = INVUL_TIME;
            blinkDuration = BLINK_TIME;
            sanityTime = SANITY_TIME;
            sanityBlockade = 0;

            // Reset the Positions
            gameOverPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverMessage).Y / 2 - 25);
            gameOverContPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverContMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverContMessage).Y / 2 + 25);
            
        }
        protected void softReset() {
            //Reset all objects on the stage
            homeworks.Clear();
            exams.Clear();
            goal = new Rectangle();

            //Load stage
            buildLevel(ref platforms, ref student, ref homeworks); 

            //Reset game tool variables
            screenOffset = 0;
            translation = Matrix.Identity;
            updatePosition();
            hitRecoilTime = INVUL_TIME;
            blinkDuration = BLINK_TIME;
            gameOverPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverMessage).Y / 2 - 25);
            gameOverContPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverContMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverContMessage).Y / 2 + 25);
            
        }

        private void handleStudentPlatformCollision( List<Platform> platform ) {
            int interLeft, interRight, interTop, interBot, interWidth, interHeight;
            for (int i = 0; i < platform.Count; ++i) {
                interLeft = Math.Max((int)student.position.X, platform[i].rectangle.Left);
                interTop = Math.Max((int)student.position.Y, platform[i].rectangle.Top);
                interRight = Math.Min((int)student.position.X + studentWidth, platform[i].rectangle.Right);
                interBot = Math.Min((int)student.position.Y + studentHeight, platform[i].rectangle.Bottom);
                interWidth = interRight - interLeft;
                interHeight = (interBot - interTop);

                if (interWidth >= 0 && interHeight >= 0) {  //If the intersecting rect is valid, they hit!
               
                    student.colliding = true;

                    //Movement collision on ground
                    if (interHeight > interWidth) {
                        //If student going to the right, stop them at the left edge of the platform
                        if (student.velocity.X > 0) { student.position.X -= interWidth; }

                        //If going to the left
                        if (student.velocity.X < 0) { student.position.X += interWidth; }

                        //They collided, so stop moving
                        student.velocity.X = 0;
                    }

                    //Vertical movement collision (Adjustments added compensate for odd corner cases)
                    if (interWidth > interHeight-Math.Abs(student.velocity.Y) && interWidth > student.speed+1) {
                        //If student is falling, stop them at the top edge of the platform (Make sure it's above the platform)
                        if (student.velocity.Y > 0 && student.position.Y < platform[i].position.Y) {
                            student.position.Y -= interHeight; 
                            student.onGround = true;
                            student.velocity.Y = 0;
                        }

                        //If student is going upward from a jump, put them down (w/ a little force)
                        if (student.velocity.Y < 0) { 
                            student.position.Y += interHeight; 
                            student.velocity.Y *= -.1F; 
                        }
                    }
                }
                else { student.colliding = false; }
            }
        }
        protected void handleSpriteMovement( ref AnimatedSprite sprite ) {
            //Update the current sprite being taken from the sprite sheet
            sprite.SourceRect = new Rectangle(sprite.currentFrame * sprite.width, 0, sprite.width, sprite.height);

            //Keep the animation loop
            if (Keyboard.GetState().GetPressedKeys().Length == 0) {
                if (sprite.currentFrame > 0 && sprite.currentFrame < 4) {
                    sprite.currentFrame = 0;
                }
                if (sprite.currentFrame > 4 && sprite.currentFrame < 8) {
                    sprite.currentFrame = 4;
                }
            }
        }
        protected void handleInvulTime( GameTime gameTime ){
            hitRecoilTime -= gameTime.ElapsedGameTime.Milliseconds;
            if( hitRecoilTime < 0 ){
                hitRecoilTime = INVUL_TIME;
                student.hit = false;
                lostHealth = false;
            }
        }
        protected void checkForVictory(){

            //If the student falls into the 
            if( goal.Left < student.position.X+student.origin.X && student.position.X+student.origin.X <= goal.Right &&
                goal.Top  < student.position.Y+student.origin.Y && student.position.Y+student.origin.Y <= goal.Bottom ){
                
                //Increment game level
                if( gameLevel < MAXLEVELS ){ gameLevel++; }
                currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";
                softReset();
                staticScreenTrigger = true;
                currentStatus = (int)ScreenStatus.WEEKEND;

            }
        }
        private void buildLevel( ref List<Platform> platforms, ref Student1 student, ref List<Homework> homeworks ) {
            platforms.Clear();
            int x = 0; int y = 0;
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader( currentLevelFile );
            
            while( (line = file.ReadLine()) != null ){
                char[] levelRow = line.ToCharArray();
                foreach( char symbol in levelRow ){
                    //if( symbol == '.' || symbol == ' ') {  }
                    if( symbol == 's' ){ student.position = new Vector2( x, y ); }
                    if( symbol == 'x' ){ platforms.Add( new Platform( new Vector2( x, y ), Vector2.Zero, 0 ) ); }
                    if( symbol == 'h' ){ 
                        homeworks.Add( new Homework( new Vector2( x, y ), new Vector2(homeworkSprite.Width / 2, homeworkSprite.Height / 2), Vector2.Zero ) ); 
                        homeworks.Last().colorArr = new Color[ homeworkSprite.Width * homeworkSprite.Height ];
                        homeworkSprite.GetData<Color>( homeworks.Last().colorArr );
                    }
                    if( symbol == 'g' ){
                        goal = new Rectangle( x, y, defaultBlockWidth, defaultBlockHeight );
                    }
                    if (symbol == 'e') {
                        exams.Add(new Exam(new Vector2(x, y), new Vector2(examSprite.Width / 2, examSprite.Height / 2), Vector2.Zero));
                        exams.Last().colorArr = new Color[examSprite.Width * examSprite.Height];
                        examSprite.GetData<Color>(exams.Last().colorArr);
                    }
                    if (symbol == '-') {
                        hiddenPlatforms.Add(new Platform(new Vector2(x, y), Vector2.Zero, 0));
                    } 
                    if (symbol == 't') {
                        triggers.Add(new Platform(new Vector2(x, y), Vector2.Zero, 0));
                    }
                    x += defaultBlockWidth;
                }
                x = 0;
                y += defaultBlockHeight;
            }
        }
    }
}
