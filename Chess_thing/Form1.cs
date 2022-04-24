using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Chess_thing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(bool IsTimeEnabled,int[] Timer, bool IsAiEnabled = false)
        {
            InitializeComponent();
            WithTime = IsTimeEnabled;

            PredefinedTime[0] = Timer[0];
            PredefinedTime[1] = Timer[1];

            WhiteTime[0] = Timer[0];
            WhiteTime[1] = Timer[1];
            BlackTime[0] = Timer[0];
            BlackTime[1] = Timer[1];
        }


        //TODO:
        /* Bug Hunting
         * >Pawn Promotion
         * Main Menu(in another form)
         * Cancelar o movimento se este não for possivel, assim o jogador pode escolher outra peça (DONE!!!)
         * Cheque (DONE!!!)
         * >>Cheque-Mate (DONE!!!)
         * >Pontuação
         * Ai
         * Multiplayer Mode: 
         *      SAME COMPUTER 
         *      LAN NETWORK 
         *      THROUGH INTERNET
         * O rei não pode se mover para uma posição que o deixa em cheque (DONE!!!)
         * Peças aliadas não podem se mover se este movimento pode deixar o rei em cheque (DONE!!!)
        */

        #region GlobalVariables
        string[] Begining_Wpos = {"A7","B7","C7","D7","E7","F7","G7","H7",
                                  "A8","B8","C8","D8","E8","F8","G8","H8"},
                 Begining_Bpos =  {"A2","B2","C2","D2","E2","F2","G2","H2",
                                   "A1","B1","C1","D1","E1","F1","G1","H1"};

        string[] Wpos = {"E3","B7","C7","D7","E7","F7","G7","H7",
                         "A8","B8","C3","D8","E8","F8","G8","H8"};
        string[] Bpos = {"A2","B2","C2","D2","E6","F2","G2","H2",
                         "A1","B1","B5","A4","E1","F1","G1","H1"};
        string[] PieceChar = {"P","P","P","P","P","P","P","P",
                              "T","H","B","Q","K","B","H","T"};

        double[] PieceValue = {1, 1, 1, 1, 1, 1, 1, 1,
                               5.1, 3.2, 3.33, 8.8, 200, 3.33, 3.2, 5.5};


        string[,] Board;
        Bitmap[] White, Black;
        PictureBox[] WPieces, BPieces;
        bool IsWhiteTurn = true; //otherwise is the turn of the black pieces
        string[] ToMove = { "false", "", "", "" };//{"false","A1","P","0"};
        int[] mousePos = { 0, 0 };

        int y0 = 49, x0 = 44;
        int[] AtackingPiecePos = new int[2];
        int[] DefendingPiecePos = new int[2];
        int[] MovingPiecePos = new int[2];
        string MousePos_str;


        bool WhiteIsCap = false, BlackIsCap = false;
        int WhiteKingIndex = 12, BlackKingIndex = 12;

        bool WithTime = false;
        int[] PredefinedTime = new int[2];
        int[] WhiteTime = {0, 20}, BlackTime = {0, 20};
        bool HasRestarded = false;
        bool Lose = false;
        public bool GoBack = false;
        #endregion

        /*
         * HAND-MADE FUNCTIONS
         */

        #region MainFunctions
        public bool Check_Mate(bool IsWhiteTheAtacker)
        {
            //if the place is blocked by an ally piece
            //if there's an enemy piece, if it is safe to cap it
            //if the place is safe
            bool KingLost = false;
            bool[] IsBlocked = { false, false, false, false, false, false, false, false, false };
            bool[] IsBlockedByAlly = { false, false, false, false, false, false, false, false, true };

            int IsBlockedCounter = 0;
            int[] KingPos = new int[2];
            if (IsWhiteTheAtacker)
            {
                KingPos[0] = Str2int(Wpos[WhiteKingIndex])[1];
                KingPos[1] = Str2int(Wpos[WhiteKingIndex])[0];
            }
            else
            {
                KingPos[0] = Str2int(Bpos[BlackKingIndex])[1];
                KingPos[1] = Str2int(Bpos[BlackKingIndex])[0];
            }

            string[] KingPossibleMoves = {int2Str(KingPos[1], KingPos[0] - 1),
                                          int2Str(KingPos[1] - 1, KingPos[0] - 1),
                                          int2Str(KingPos[1] - 1, KingPos[0]),
                                          int2Str(KingPos[1] - 1, KingPos[0] + 1),
                                          int2Str(KingPos[1], KingPos[0] + 1),
                                          int2Str(KingPos[1] + 1, KingPos[0] + 1),
                                          int2Str(KingPos[1] + 1, KingPos[0]),
                                          int2Str(KingPos[1] + 1, KingPos[0] - 1),
                                          int2Str(KingPos[1], KingPos[0])};

            string[] HelperPieces = { "Free Space", "Free Space", "Free Space", "Free Space", "Free Space", "Free Space", "Free Space", "Free Space", "Free Space" };
            //if the path is blocked or atacked
            for (int i = 0; i < Wpos.Length; ++i)
            {
                if (IsWhiteTheAtacker)
                {
                    for (int j = 0; j < IsBlocked.Length; ++j)
                    {
                        if (Wpos[i] == KingPossibleMoves[j] && WPieces[i].Visible)
                            IsBlockedByAlly[j] = true;
                        if ((KingPossibleMoves[j] == "NA" || CheckTest(IsWhiteTheAtacker, KingPossibleMoves[j], WhiteKingIndex) || (Wpos[i] == KingPossibleMoves[j] && WPieces[i].Visible)) && !IsBlocked[j])
                        {
                            IsBlocked[j] = true;
                            HelperPieces[j] = "NA";
                            ++IsBlockedCounter;
                            break;
                        }

                    }
                }
                else
                {
                    for (int j = 0; j < IsBlocked.Length; ++j)
                    {
                        if (Bpos[i] == KingPossibleMoves[j] && BPieces[i].Visible)
                            IsBlockedByAlly[j] = true;
                        if ((KingPossibleMoves[j] == "NA" || CheckTest(IsWhiteTheAtacker, KingPossibleMoves[j], BlackKingIndex) || (Bpos[i] == KingPossibleMoves[j] && BPieces[i].Visible)) && !IsBlocked[j])
                        {
                            //if (KingPossibleMoves[j] != "NA")
                            //MessageBox.Show(KingPossibleMoves[j] + "\n" + CheckTest(IsWhiteTheAtacker, KingPossibleMoves[j], BlackKingIndex).ToString());
                            IsBlocked[j] = true;
                            HelperPieces[j] = "NA";
                            ++IsBlockedCounter;
                            break;
                        }
                        if (KingPossibleMoves[j] != "NA" && !CheckTest(IsWhiteTheAtacker, KingPossibleMoves[j], WhiteKingIndex))
                        {
                            for (int k = 0; k < Wpos.GetLength(0); ++k)
                            {
                                if (Wpos[k] == KingPossibleMoves[j] && WPieces[k].Visible)
                                {
                                    HelperPieces[j] = "The king itself!";
                                    break;
                                }
                            }
                        }
                    }
                }
            }//end for

            string text = "";
            for (int j = 0; j < IsBlocked.Length; ++j)
            {
                text += "Ally[" + j.ToString() + "] = " + IsBlockedByAlly[j].ToString() + "\n";
            }
            if (IsBlockedCounter > 1)
            {
                //if any ally piece can save the king by blocking the atack
                for (int i = 0; i < Wpos.Length; ++i)
                {
                    if (IsWhiteTheAtacker)
                    {
                        if (i != WhiteKingIndex)
                        {
                            for (int j = 0; j < IsBlocked.Length; ++j)
                            {
                                if (KingPossibleMoves[j] != "NA" && TestMovement(IsWhiteTheAtacker, Wpos[i], KingPossibleMoves[j]) && !CheckTest(!IsWhiteTheAtacker, KingPossibleMoves[j], i) && WPieces[i].Visible && IsBlocked[j])
                                {
                                    IsBlocked[j] = false;
                                    --IsBlockedCounter;
                                    HelperPieces[j] = PieceChar[i] + " at " + Wpos[i];
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (i != BlackKingIndex)
                        {
                            for (int j = 0; j < IsBlocked.Length; ++j)
                            {
                                if (KingPossibleMoves[j] != "NA" && TestMovement(IsWhiteTheAtacker, Bpos[i], KingPossibleMoves[j]) && !CheckTest(!IsWhiteTheAtacker, KingPossibleMoves[j], i) && BPieces[i].Visible && IsBlocked[j])
                                {
                                    IsBlocked[j] = false;
                                    --IsBlockedCounter;
                                    HelperPieces[j] = PieceChar[i] + " at " + Bpos[i];
                                    break;
                                }
                            }
                        }
                    }
                }//end for

                //if any ally piece can save the king by capturing an enemy piece
                for (int i = 0; i < Wpos.Length; ++i)
                {
                    if (IsWhiteTheAtacker)
                    {
                        for (int j = 0; j < Bpos.Length; ++j)
                        {
                            if (j != BlackKingIndex)
                            {
                                if (WPieces[i].Visible && BPieces[j].Visible && TestCapture(i, j, IsWhiteTheAtacker, PieceChar[i]) && !CheckTest(IsWhiteTheAtacker, Bpos[j], i))
                                {
                                    string temp = Wpos[i];
                                    BPieces[j].Visible = false;
                                    Wpos[i] = Bpos[j];

                                    for (int b = 0; b < IsBlockedByAlly.GetLength(0); ++b)
                                    {
                                        IsBlockedByAlly[b] = false;
                                        for (int a = 0; a < Wpos.GetLength(0); ++a)
                                        {
                                            if (Wpos[a] == KingPossibleMoves[b] && WPieces[a].Visible)
                                            {
                                                IsBlockedByAlly[b] = true;
                                                break;
                                            }
                                                
                                        }
                                    }

                                    for (int k = 0; k < IsBlocked.Length; ++k)
                                    {
                                        //(KingPossibleMoves[k] != "NA" && CheckTest(IsWhiteTheAtacker, KingPossibleMoves[k], WhiteKingIndex) && IsBlocked[k])
                                        if (KingPossibleMoves[k] != "NA" && !CheckTest(IsWhiteTheAtacker, KingPossibleMoves[k], WhiteKingIndex) && IsBlocked[k] && !IsBlockedByAlly[k])
                                        {

                                            IsBlocked[k] = false;
                                            --IsBlockedCounter;
                                            HelperPieces[k] = PieceChar[i] + " at " + temp + "; by Capturing a " + PieceChar[j] + " at " + Bpos[j];
                                            break;
                                        }
                                    }
                                    Wpos[i] = temp;
                                    BPieces[j].Visible = true;

                                    for (int b = 0; b < IsBlockedByAlly.GetLength(0); ++b)
                                    {
                                        IsBlockedByAlly[b] = false;
                                        for (int a = 0; a < Wpos.GetLength(0); ++a)
                                        {
                                            if (Wpos[a] == KingPossibleMoves[b] && WPieces[a].Visible)
                                            {
                                                IsBlockedByAlly[b] = true;
                                                break;
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        //if (i != WhiteKingIndex)
                        //{
                        //}
                    }
                    else
                    {
                        for (int j = 0; j < Wpos.Length; ++j)
                        {
                            if (j != WhiteKingIndex)
                            {
                                if (BPieces[i].Visible && WPieces[j].Visible && TestCapture(i, j, IsWhiteTheAtacker, PieceChar[i]) && !CheckTest(IsWhiteTheAtacker, Wpos[j], i))
                                {
                                    string temp = Bpos[i];
                                    WPieces[j].Visible = false;
                                    Bpos[i] = Wpos[j];

                                    for (int b = 0; b < IsBlockedByAlly.GetLength(0); ++b)
                                    {
                                        IsBlockedByAlly[b] = false;
                                        for (int a = 0; a < Bpos.GetLength(0); ++a)
                                        {
                                            if (Bpos[a] == KingPossibleMoves[b] && BPieces[a].Visible)
                                            {
                                                IsBlockedByAlly[b] = true;
                                                break;
                                            }

                                        }
                                    }

                                    for (int k = 0; k < IsBlocked.Length; ++k)
                                    {
                                        if (KingPossibleMoves[k] != "NA" && !CheckTest(IsWhiteTheAtacker, KingPossibleMoves[k], BlackKingIndex) && IsBlocked[k] && !IsBlockedByAlly[k])//(Bpos[i] != KingPossibleMoves[j] && BPieces[i].Visible)
                                        {
                                            IsBlocked[k] = false;
                                            --IsBlockedCounter;
                                            HelperPieces[k] = PieceChar[i] + " at " + temp + "; by Capturing a " + PieceChar[j] + " at " + Wpos[j];
                                            //break;
                                        }
                                    }
                                    Bpos[i] = temp;
                                    WPieces[j].Visible = true;

                                    for (int b = 0; b < IsBlockedByAlly.GetLength(0); ++b)
                                    {
                                        IsBlockedByAlly[b] = false;
                                        for (int a = 0; a < Bpos.GetLength(0); ++a)
                                        {
                                            if (Bpos[a] == KingPossibleMoves[b] && BPieces[a].Visible)
                                            {
                                                IsBlockedByAlly[b] = true;
                                                break;
                                            }

                                        }
                                    }

                                }
                            }
                        }

                        //if (i != BlackKingIndex)
                        //{
                        //}
                    }
                }//end for
            }

            if (IsBlockedCounter == 9)
                KingLost = true;
            string deb = "IsWhiteTheAtacker = " + IsWhiteTheAtacker.ToString() +
                            "\nKingPos = " + int2Str(KingPos[1], KingPos[0]) +
                            "\nKingLost = " + KingLost.ToString() +
                            "\nIsBlockedCounter = " + IsBlockedCounter.ToString() + "\n\n";
            for (int i = 0; i < IsBlocked.Length; ++i)
            {
                if (HelperPieces[i] == "NA")
                    deb += "IsBlocked[" + i.ToString() + "](" + KingPossibleMoves[i] + "){ally = " + IsBlockedByAlly[i].ToString() + " } = " + IsBlocked[i].ToString() + "\n";
                else
                    deb += "IsBlocked[" + i.ToString() + "](" + KingPossibleMoves[i] + ")(Saved by " + HelperPieces[i] + "){ally = " + IsBlockedByAlly[i].ToString() + " } = " + IsBlocked[i].ToString() + "\n";
            }

            label19.Text = deb;
            //MessageBox.Show(text);
            //MessageBox.Show(deb);
            if (KingLost)
            {
                MessageBox.Show("The King Fell into the ground!!!\nThe game will be reseted", "The King Lost!!!");
                ResetGame();
            }

            return KingLost;
        }

        public void ResetGame()
        {
            HasRestarded = true;

            WhiteTime[0] = PredefinedTime[0];
            WhiteTime[1] = PredefinedTime[1];
            BlackTime[0] = PredefinedTime[0];
            BlackTime[1] = PredefinedTime[1];

            WHITE_TIME.Text = WhiteTime[1].ToString("00") + ":" + WhiteTime[0].ToString("00");
            BLACK_TIME.Text = BlackTime[1].ToString("00") + ":" + BlackTime[0].ToString("00");

            CLOCK.Visible = WithTime;
            if(WithTime)
                PlayersMovement.Start();

            for (int i = 0; i < Wpos.Length; ++i)
            {
                Wpos[i] = Begining_Wpos[i];
                Bpos[i] = Begining_Bpos[i];

                MovePiece(WPieces[i], Wpos[i]);
                MovePiece(BPieces[i], Bpos[i]);

                WPieces[i].Visible = true;
                BPieces[i].Visible = true;
            }
            IsWhiteTurn = true;
            WhiteIsCap = false;
            BlackIsCap = false;
            ToMove[0] = "false";
            turn.Text = "WHITE";
            label20.Text = "Last Clicked Position = NONE";
            label21.Text = "White is in check = " + WhiteIsCap.ToString() + "\nBlack is in check = " + BlackIsCap.ToString();
        }

        public int GetPieceIndex(string PiecePos)
        {
            int index = -1;
            for (int i = 0; i < Wpos.Length; ++i)
            {
                if (Wpos[i] == PiecePos && WPieces[i].Visible)
                {
                    index = i;
                    break;
                }
                else if (Bpos[i] == PiecePos && BPieces[i].Visible)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public int GetPieceIndex(string PiecePos, bool IsWhite)
        {
            int index = -1;
            for (int i = 0; i < Wpos.Length; ++i)
            {
                if (IsWhite && Wpos[i] == PiecePos && WPieces[i].Visible)
                {
                    index = i;
                    break;
                }
                else if (!IsWhite && Bpos[i] == PiecePos && BPieces[i].Visible)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public bool CheckTest(bool IsWhiteTheAtacker, string FuturePosition, int ActualPositionIndex)//CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
        {
            bool IsInCheck = false;
            int BlackAtackerIndex = 0, WhiteAtackerIndex = 0;
            int FuturePositionPieceIndex = GetPieceIndex(FuturePosition);
            bool isVisible = false;
            string temp = "";

            for (int i = 0; i < Wpos.GetLength(0); ++i)
            {
                if (IsWhiteTheAtacker && WPieces[i].Visible && Wpos[i] == FuturePosition || !IsWhiteTheAtacker && BPieces[i].Visible && Bpos[i] == FuturePosition)
                {
                    //if (IsWhiteTheAtacker)
                    //    MessageBox.Show("IsWhite = " + IsWhiteTheAtacker.ToString() + "\n" + ActualPositionIndex.ToString() + "\nFrom: " + Bpos[ActualPositionIndex] + "\nTo: " + FuturePosition, "Inside Check function");
                    //else
                    //    MessageBox.Show("IsWhite = " + IsWhiteTheAtacker.ToString() + "\n" + ActualPositionIndex.ToString() + "\nFrom: " + Wpos[ActualPositionIndex] + "\nTo: " + FuturePosition, "Inside Check function");
                    return false;
                }
            }


            if (IsWhiteTheAtacker)
            {

                temp = Wpos[ActualPositionIndex];
                if (FuturePositionPieceIndex != -1 && BPieces[FuturePositionPieceIndex].Visible)
                {
                    isVisible = true;
                    BPieces[FuturePositionPieceIndex].Visible = false;
                }
                Wpos[ActualPositionIndex] = FuturePosition;
            }
            else
            {
                temp = Bpos[ActualPositionIndex];
                if (FuturePositionPieceIndex != -1 && WPieces[FuturePositionPieceIndex].Visible)
                {
                    isVisible = true;
                    WPieces[FuturePositionPieceIndex].Visible = false;
                }
                Bpos[ActualPositionIndex] = FuturePosition;
            }

            for (int i = 0; i < Wpos.Length; ++i)
            {
                if (IsWhiteTheAtacker)//if is the white turn check for the black pieces
                {
                    if (BPieces[i].Visible)
                        IsInCheck = TestCapture(i, WhiteKingIndex, !IsWhiteTheAtacker, PieceChar[i]);//the black is the atacker
                    //MessageBox.Show(IsWhiteTheAtacker.ToString() + "\n" + IsInCheck.ToString() + "\n" + i.ToString());
                    if (IsInCheck)
                        BlackAtackerIndex = i;
                }
                else //if is the black turn
                {
                    if (WPieces[i].Visible)
                        IsInCheck = TestCapture(i, BlackKingIndex, !IsWhiteTheAtacker, PieceChar[i]);//the white is the atacker
                    //MessageBox.Show(IsWhiteTheAtacker.ToString() + "\n" + IsInCheck.ToString() + "\n" + i.ToString() + "\n" + Wpos[i]);
                    if (IsInCheck)
                        WhiteAtackerIndex = i;
                }
                if (IsInCheck)
                    break;
            }

            //if (IsInCheck)
            //MessageBox.Show("Check");

            if (IsWhiteTheAtacker)
            {
                //MessageBox.Show("WhitePiece(" + PieceChar[ActualPositionIndex] + ")\n" + ActualPositionIndex.ToString() + "\nFrom: " + temp + "\nTo: " + Wpos[ActualPositionIndex] + "\n" + IsInCheck.ToString(), "Inside Check function");
                Wpos[ActualPositionIndex] = temp;
                if (FuturePositionPieceIndex != -1 && isVisible == true)
                    BPieces[FuturePositionPieceIndex].Visible = true;
            }
            else
            {
                //MessageBox.Show("BlackPiece(" + PieceChar[ActualPositionIndex] + ")\n" + ActualPositionIndex.ToString() + "\nFrom: " + temp + "\nTo: " + Bpos[ActualPositionIndex] + "\n" + IsInCheck.ToString(), "Inside Check function");
                Bpos[ActualPositionIndex] = temp;
                if (FuturePositionPieceIndex != -1 && isVisible == true)
                    WPieces[FuturePositionPieceIndex].Visible = true;
            }

            return IsInCheck;
        }

        public void CheckTest()
        {
            WhiteIsCap = false;
            BlackIsCap = false;
            int BlackAtackerIndex = 0, WhiteAtackerIndex = 0;
            string output = "";
            for (int i = 0; i < Wpos.Length; ++i)
            {
                if (!WhiteIsCap)
                {
                    if (BPieces[i].Visible)
                        WhiteIsCap = TestCapture(i, WhiteKingIndex, false, PieceChar[i]);//the black is the atacker
                    if (WhiteIsCap)
                        BlackAtackerIndex = i;
                }
                if (!BlackIsCap)
                {
                    if (WPieces[i].Visible)
                        BlackIsCap = TestCapture(i, BlackKingIndex, true, PieceChar[i]);//the white is the atacker
                    if (BlackIsCap)
                        WhiteAtackerIndex = i;
                }
                if (BlackIsCap && WhiteIsCap)
                    break;
            }

            if (WhiteIsCap)
                output = "White is in check by a " + PieceChar[BlackAtackerIndex] + " at " + Bpos[BlackAtackerIndex] + "\n";
            if (BlackIsCap)
                output += "Black is in check by a " + PieceChar[WhiteAtackerIndex] + " at " + Wpos[WhiteAtackerIndex];
            //if (WhiteIsCap || BlackIsCap)
            //    MessageBox.Show("Check");
            label21.Text = "White is in check = " + WhiteIsCap.ToString() + "\nBlack is in check = " + BlackIsCap.ToString() + "\n\n" + output;
        }

        public void CheckTest(int AtackerIndex, string AtackerPiece)
        {
            bool WhiteIsCap = TestCapture(AtackerIndex, WhiteKingIndex, false, AtackerPiece);//the black is the atacker
            bool BlackIsCap = TestCapture(AtackerIndex, BlackKingIndex, true, AtackerPiece);//the white is the atacker
            label21.Text = "White is in check = " + WhiteIsCap.ToString() + "\nBlack is in check = " + BlackIsCap.ToString() + "\n";
        }

        public int[] MousePos()
        {
            MousePos_str = Mouse2Pos();
            int[] ClickedPos = { Str2int(MousePos_str)[1], Str2int(MousePos_str)[0] };
            return ClickedPos;
        }

        public string Mouse2Pos()
        {
            string[] output = new string[5];

            for (int i = 0; i < 64; ++i)
            {
                if (mousePos[1] >= Convert.ToInt16(Board[i, 1]) && mousePos[1] <= Convert.ToInt16(Board[i, 1]) + 60 && mousePos[0] >= Convert.ToInt16(Board[i, 2]) - 10 && mousePos[0] <= Convert.ToInt16(Board[i, 2]) + 50)
                {
                    output[0] = Board[i, 0];
                    output[1] = (Convert.ToInt16(Board[i, 2])).ToString();
                    output[2] = (Convert.ToInt16(Board[i, 2]) + 60).ToString();
                    output[3] = (Convert.ToInt16(Board[i, 1]) - 10).ToString();
                    output[4] = (Convert.ToInt16(Board[i, 1]) + 50).ToString();
                    break;
                }
            }
            return output[0];
        }

        public void TestCapture(int piece_index)
        {
            //piece_indexer is the index of the defender piece!
            bool ThereWasCapturing = false;
            if (ToMove[0] == "true")
            {
                //CheckTest();
                AtackingPiecePos[0] = Str2int(ToMove[1])[1];
                AtackingPiecePos[1] = Str2int(ToMove[1])[0];

                if (IsWhiteTurn)//A white piece is goint to cap a black piece
                {
                    DefendingPiecePos[0] = Str2int(Bpos[piece_index])[1];
                    DefendingPiecePos[1] = Str2int(Bpos[piece_index])[0];
                    //label19.Text = "Def(" + DefendingPiecePos[0].ToString() + ";" + DefendingPiecePos[1].ToString() + ")\n" +
                    //                   "Atk(" + AtackingPiecePos[0].ToString() + ";" + AtackingPiecePos[1].ToString() + ")\n";

                    if (ToMove[2] == "P")//if the white piece is a pawn   && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3]))
                    {
                        if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 || DefendingPiecePos[1] == AtackingPiecePos[1] - 1) && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                            BPieces[piece_index].Hide();
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "H")//white tower
                    {
                        bool cap = false;
                        if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                            cap = true;
                        if (cap && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                            BPieces[piece_index].Hide();
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                    }
                    else if (ToMove[2] == "T")//white tower
                    {
                        bool IsPathClear = true;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(x, y) + "\n";

                        if (DefendingPiecePos[0] == AtackingPiecePos[0])
                        {
                            while (true)
                            {
                                if (x == DefendingPiecePos[1])
                                    break;
                                if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                                BPieces[piece_index].Hide();
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1])
                        {
                            while (true)
                            {
                                if (y == DefendingPiecePos[0])
                                    break;
                                if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                                BPieces[piece_index].Hide();
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                    }
                    else if (ToMove[2] == "B")//white bishop
                    {
                        bool IsPathClear = true;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(x, y) + "\n";

                        if (Math.Abs(AtackingPiecePos[0] - DefendingPiecePos[0]) == Math.Abs(AtackingPiecePos[1] - DefendingPiecePos[1]))
                        {
                            while (true)
                            {
                                if (x == DefendingPiecePos[1] || y == DefendingPiecePos[0] || x == 0 || x == 8 || y == 0 || y == 8)
                                    break;
                                if (AtackingPiecePos[1] < DefendingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                                if (AtackingPiecePos[0] < DefendingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                                BPieces[piece_index].Hide();
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "K")//white king
                    {
                        bool cap = false;
                        if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;


                        if (cap && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                            BPieces[piece_index].Hide();
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "Q")//white queen
                    {
                        bool IsPathClear = true;
                        bool cap = false;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(AtackingPiecePos[1], AtackingPiecePos[0]);

                        while (true)
                        {
                            if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[0] == AtackingPiecePos[0])
                            {
                                if (x == MousePos()[1])
                                {
                                    if (IsPathClear)
                                        cap = true;
                                    break;
                                }
                                if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[1] == AtackingPiecePos[1])
                            {
                                if (y == DefendingPiecePos[0])
                                {
                                    if (IsPathClear)
                                        cap = true;
                                    break;
                                }
                                if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != piece_index)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (cap && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3])))
                        {
                            if (IsPathClear)
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = Bpos[piece_index];
                                BPieces[piece_index].Hide();
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                    }

                    WPieces[Convert.ToInt16(ToMove[3])].BorderStyle = BorderStyle.None;
                    /********************************************************************
                     *              END OF WHITE PIECES CAPTURE                         *
                     ********************************************************************/
                }
                else if (!IsWhiteTurn)//A black piece is going to capture a white piece
                {
                    DefendingPiecePos[0] = Str2int(Wpos[piece_index])[1];
                    DefendingPiecePos[1] = Str2int(Wpos[piece_index])[0];
                    //label19.Text = "Def(" + DefendingPiecePos[0].ToString() + ";" + DefendingPiecePos[1].ToString() + ")\n" +
                    //                   "Atk(" + AtackingPiecePos[0].ToString() + ";" + AtackingPiecePos[1].ToString() + ")\n";

                    if (ToMove[2] == "P")//if the black piece is a pawn   && !CheckTest(IsWhiteTurn, Bpos[piece_index], int.Parse(ToMove[3]))
                    {
                        if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 || DefendingPiecePos[1] == AtackingPiecePos[1] - 1) && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                            WPieces[piece_index].Hide();//the piece has been captured!
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "H")//black horse
                    {
                        bool cap = false;
                        if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                            cap = true;
                        else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                            cap = true;
                        if (cap && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                            WPieces[piece_index].Hide();//the piece has been captured!
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                    }
                    else if (ToMove[2] == "T")//black tower
                    {
                        bool IsPathClear = true;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(x, y) + "\n";

                        if (DefendingPiecePos[0] == AtackingPiecePos[0])
                        {
                            while (true)
                            {
                                if (x == DefendingPiecePos[1])
                                    break;
                                if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                                WPieces[piece_index].Hide();//the piece has been captured!
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1])
                        {
                            while (true)
                            {
                                if (y == DefendingPiecePos[0])
                                    break;
                                if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                                WPieces[piece_index].Hide();//the piece has been captured!
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                    }
                    else if (ToMove[2] == "B")//black bishop
                    {
                        bool IsPathClear = true;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(x, y) + "\n";

                        if (Math.Abs(AtackingPiecePos[0] - DefendingPiecePos[0]) == Math.Abs(AtackingPiecePos[1] - DefendingPiecePos[1]))
                        {
                            while (true)
                            {
                                if (x == AtackingPiecePos[1])
                                    break;
                                if (AtackingPiecePos[1] > DefendingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                                if (AtackingPiecePos[0] > DefendingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != piece_index)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                                WPieces[piece_index].Hide();//the piece has been captured!
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                    }
                    else if (ToMove[2] == "K")//black king
                    {
                        bool cap = false;
                        if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                            cap = true;
                        else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                            cap = true;


                        if (cap && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                            WPieces[piece_index].Visible = false;
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasCapturing = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "Q")//black queen
                    {
                        bool IsPathClear = true;
                        bool cap = false;

                        int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                        //richTextBox1.Text += int2Str(AtackingPiecePos[1], AtackingPiecePos[0]);

                        while (true)
                        {
                            if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[0] == AtackingPiecePos[0])
                            {
                                if (x == MousePos()[1])
                                {
                                    if (IsPathClear)
                                        cap = true;
                                    break;
                                }
                                if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[1] == AtackingPiecePos[1])
                            {
                                if (y == DefendingPiecePos[0])
                                {
                                    if (IsPathClear)
                                        cap = true;
                                    break;
                                }
                                if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != piece_index)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (cap && !CheckTest(IsWhiteTurn, Wpos[piece_index], int.Parse(ToMove[3])))
                        {
                            if (IsPathClear)
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = Wpos[piece_index];
                                WPieces[piece_index].Hide();//the piece has been captured!
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasCapturing = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                    }
                    BPieces[Convert.ToInt16(ToMove[3])].BorderStyle = BorderStyle.None;
                }

                //richTextBox1.Text = "WPieces:\n";
                //for (int i = 0; i < Wpos.Length; ++i)
                //{
                //    richTextBox1.Text += "Visible[" + PieceChar[i] + "; " + Wpos[i] + "] = " + WPieces[i].Visible.ToString() + "\n";
                //}
                //richTextBox1.Text += "\nBPieces:\n";
                //for (int i = 0; i < Wpos.Length; ++i)
                //{
                //    richTextBox1.Text += "Visible[" + PieceChar[i] + "; " + Bpos[i] + "] = " + BPieces[i].Visible.ToString() + "\n";
                //}

                if (IsWhiteTurn)
                    turn.Text = "WHITE";
                else
                    turn.Text = "BLACK";

                CheckTest();
                if (WhiteIsCap || BlackIsCap)
                {
                    //MessageBox.Show(ThereWasCapturing.ToString() + "\n" + IsWhiteTurn.ToString());
                    Check_Mate(IsWhiteTurn);
                    //if(ThereWasCapturing)
                    //    Check_Mate(IsWhiteTurn);
                    //else
                    //    Check_Mate(!IsWhiteTurn);
                }
                else
                    label19.Text = "Nothing";
            }
        }

        public bool TestCapture(int AtackingPieceIndex, int DefendingPieceIndex, bool WhiteIsTheAtacker, string AtackerPiece)
        {
            bool IsToCapture = false;
            ///////////////////////////////////////////////////////////////////////////////////////
            if (WhiteIsTheAtacker)//A white piece is goint to cap a black piece
            {
                #region WhitePieces
                AtackingPiecePos[0] = Str2int(Wpos[AtackingPieceIndex])[1];
                AtackingPiecePos[1] = Str2int(Wpos[AtackingPieceIndex])[0];

                DefendingPiecePos[0] = Str2int(Bpos[DefendingPieceIndex])[1];
                DefendingPiecePos[1] = Str2int(Bpos[DefendingPieceIndex])[0];

                ////richTextBox1.Text = "Def(" + DefendingPiecePos[0].ToString() + ";" + DefendingPiecePos[1].ToString() + ")\n" +
                //                   "Atk(" + AtackingPiecePos[0].ToString() + ";" + AtackingPiecePos[1].ToString() + ")\n";

                if (AtackerPiece == "P")//if the white piece is a pawn
                {
                    if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                    {
                        if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 || DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                            IsToCapture = true;
                    }
                }
                else if (AtackerPiece == "H")//white horse
                {
                    bool cap = false;
                    if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                        cap = true;
                    if (cap)
                        IsToCapture = true;
                }
                else if (AtackerPiece == "T")//white tower
                {
                    bool IsPathClear = true;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    ////richTextBox1.Text += int2Str(x, y) + "\n";

                    if (DefendingPiecePos[0] == AtackingPiecePos[0])
                    {
                        while (true)
                        {
                            if (x == DefendingPiecePos[1])
                                break;
                            if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                ++x;
                            else
                                --x;

                            for (int i = 0; i < 16; ++i)
                            {
                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear && !CheckTest(IsWhiteTurn, Bpos[DefendingPieceIndex], AtackingPieceIndex))
                            IsToCapture = true;
                    }
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1])
                    {
                        while (true)
                        {
                            if (y == DefendingPiecePos[0])
                                break;
                            if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                ++y;
                            else
                                --y;

                            for (int i = 0; i < 16; ++i)
                            {
                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear)
                            IsToCapture = true;
                    }
                }
                else if (AtackerPiece == "B")//white bishop
                {
                    bool IsPathClear = true;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    //richTextBox1.Text = int2Str(x, y) + "\n";
                    //MessageBox.Show(Wpos[AtackingPieceIndex] + "\nAtkP[1] = " + AtackingPiecePos[1].ToString() +
                    //                "\nAtkP[0] = " + AtackingPiecePos[0].ToString() +
                    //                "\nDefP[1] = " + DefendingPiecePos[1].ToString() +
                    //                "\nDefP[0] = " + DefendingPiecePos[0].ToString());

                    if (Math.Abs(AtackingPiecePos[0] - DefendingPiecePos[0]) == Math.Abs(AtackingPiecePos[1] - DefendingPiecePos[1]))
                    {
                        while (true)
                        {
                            if (x == DefendingPiecePos[1] || y == DefendingPiecePos[0])
                            {
                                //MessageBox.Show("x = " + x.ToString() + "\ny = " + y.ToString() + "\nIsPathClear = " + IsPathClear.ToString());
                                break;
                            }

                            if (AtackingPiecePos[1] < DefendingPiecePos[1])
                                ++x;
                            else
                                --x;

                            if (AtackingPiecePos[0] < DefendingPiecePos[0])
                                ++y;
                            else
                                --y;

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                //MessageBox.Show(int2Str(x, y) + "\n" + Bpos[i] + "\n" + IsPathClear.ToString());
                                if (!IsPathClear)
                                    break;
                            }

                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear)
                            IsToCapture = true;
                        //MessageBox.Show(IsPathClear.ToString() + "\n" + IsToCapture.ToString());
                    }
                }
                else if (AtackerPiece == "K")//white king
                {
                    bool cap = false;
                    if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;


                    if (cap)
                        IsToCapture = true;
                }
                else if (AtackerPiece == "Q")//white queen
                {
                    bool IsPathClear = true;
                    bool cap = false;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    //richTextBox1.Text += int2Str(AtackingPiecePos[1], AtackingPiecePos[0]);

                    while (true)
                    {
                        if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[0] == AtackingPiecePos[0])
                        {
                            if (x == DefendingPiecePos[1])
                            {
                                if (IsPathClear)
                                    cap = true;
                                break;
                            }
                            if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                ++x;
                            else
                                --x;
                        }
                        if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[1] == AtackingPiecePos[1])
                        {
                            if (y == DefendingPiecePos[0])
                            {
                                if (IsPathClear)
                                    cap = true;
                                break;
                            }
                            if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                ++y;
                            else
                                --y;
                        }

                        for (int i = 0; i < 16; ++i)
                        {

                            if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                IsPathClear = false;
                            else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible && i != DefendingPieceIndex)
                                IsPathClear = false;
                            if (!IsPathClear)
                                break;
                        }
                        if (!IsPathClear)
                            break;
                    }

                    if (cap)
                    {
                        if (IsPathClear)
                            IsToCapture = true;
                    }
                }
                #endregion
                /********************************************************************
                 *              END OF WHITE PIECES CAPTURE                         *
                 ********************************************************************/
            }
            else //A black piece is going to capture a white piece
            {
                #region BlackPieces
                AtackingPiecePos[0] = Str2int(Bpos[AtackingPieceIndex])[1];
                AtackingPiecePos[1] = Str2int(Bpos[AtackingPieceIndex])[0];

                DefendingPiecePos[0] = Str2int(Wpos[DefendingPieceIndex])[1];
                DefendingPiecePos[1] = Str2int(Wpos[DefendingPieceIndex])[0];
                //label19.Text = "Def(" + DefendingPiecePos[0].ToString() + ";" + DefendingPiecePos[1].ToString() + ")\n" +
                //                   "Atk(" + AtackingPiecePos[0].ToString() + ";" + AtackingPiecePos[1].ToString() + ")\n";

                if (AtackerPiece == "P")//if the black piece is a pawn
                {
                    if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                    {
                        if ((DefendingPiecePos[1] == AtackingPiecePos[1] + 1 || DefendingPiecePos[1] == AtackingPiecePos[1] - 1))// && !CheckTest(IsWhiteTurn, Wpos[DefendingPieceIndex], AtackingPieceIndex
                            IsToCapture = true;
                    }
                }
                else if (AtackerPiece == "H")//black horse
                {
                    bool cap = false;
                    if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] - 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] - 2)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] - 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 2 && DefendingPiecePos[1] == AtackingPiecePos[1] + 1)
                        cap = true;
                    else if (DefendingPiecePos[0] == AtackingPiecePos[0] + 1 && DefendingPiecePos[1] == AtackingPiecePos[1] + 2)
                        cap = true;
                    if (cap)
                        IsToCapture = true;
                }
                else if (AtackerPiece == "T")//black tower
                {
                    bool IsPathClear = true;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    //richTextBox1.Text += int2Str(x, y) + "\n";

                    if (DefendingPiecePos[0] == AtackingPiecePos[0])
                    {
                        while (true)
                        {
                            if (x == DefendingPiecePos[1])
                                break;
                            if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                ++x;
                            else
                                --x;

                            for (int i = 0; i < 16; ++i)
                            {
                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear)
                            IsToCapture = true;
                    }
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1])
                    {
                        while (true)
                        {
                            if (y == DefendingPiecePos[0])
                                break;
                            if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                ++y;
                            else
                                --y;

                            for (int i = 0; i < 16; ++i)
                            {
                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear)
                            IsToCapture = true;
                    }
                }
                else if (AtackerPiece == "B")//black bishop
                {
                    bool IsPathClear = true;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    //richTextBox1.Text += int2Str(x, y) + "\n";

                    if (Math.Abs(AtackingPiecePos[0] - DefendingPiecePos[0]) == Math.Abs(AtackingPiecePos[1] - DefendingPiecePos[1]))
                    {
                        while (true)
                        {
                            if (x == DefendingPiecePos[1] || y == DefendingPiecePos[0])
                                break;
                            if (AtackingPiecePos[1] < DefendingPiecePos[1])
                                ++x;
                            else
                                --x;
                            if (AtackingPiecePos[0] < DefendingPiecePos[0])
                                ++y;
                            else
                                --y;

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != DefendingPieceIndex)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (IsPathClear)
                            IsToCapture = true;
                    }
                }
                else if (AtackerPiece == "K")//black king
                {
                    bool cap = false;
                    if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] - 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] + 1)
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0])
                        cap = true;
                    else if (DefendingPiecePos[1] == AtackingPiecePos[1] + 1 && DefendingPiecePos[0] == AtackingPiecePos[0] - 1)
                        cap = true;


                    if (cap)
                        IsToCapture = true;
                }
                else if (AtackerPiece == "Q")//black queen
                {
                    bool IsPathClear = true;
                    bool cap = false;

                    int x = AtackingPiecePos[1], y = AtackingPiecePos[0];
                    //richTextBox1.Text += int2Str(AtackingPiecePos[1], AtackingPiecePos[0]);
                    //MessageBox.Show(AtackingPiecePos[0].ToString() + "; " + AtackingPiecePos[1].ToString() + "\n" +
                    //                DefendingPiecePos[0].ToString() + "; " + DefendingPiecePos[1].ToString());
                    while (true)
                    {
                        //MessageBox.Show(int2Str(x, y) + "\n");
                        if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[0] == AtackingPiecePos[0])
                        {
                            if (x == DefendingPiecePos[1])
                            {
                                if (IsPathClear)
                                    cap = true;
                                break;
                            }
                            if (DefendingPiecePos[1] > AtackingPiecePos[1])
                                ++x;
                            else
                                --x;
                        }
                        if (Math.Abs(DefendingPiecePos[0] - AtackingPiecePos[0]) == Math.Abs(DefendingPiecePos[1] - AtackingPiecePos[1]) || DefendingPiecePos[1] == AtackingPiecePos[1])
                        {
                            if (y == DefendingPiecePos[0])
                            {
                                if (IsPathClear)
                                    cap = true;
                                break;
                            }
                            if (DefendingPiecePos[0] > AtackingPiecePos[0])
                                ++y;
                            else
                                --y;
                        }

                        //MessageBox.Show(int2Str(x, y) + "\n");
                        
                        for (int i = 0; i < 16; ++i)
                        {
                            
                            if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible && i != DefendingPieceIndex)
                                IsPathClear = false;
                            else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                IsPathClear = false;
                            if (!IsPathClear)
                                break;
                        }
                        if (!IsPathClear)
                            break;
                    }
                    //MessageBox.Show(IsPathClear.ToString());
                    if (cap)
                    {
                        if (IsPathClear)
                            IsToCapture = true;
                    }
                }
                #endregion
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            return IsToCapture;
        }

        public bool TestMovement(bool IsWhite, string ActualPostion, string FuturePosition)
        {
            bool CanItMove = false;
            int ActualPositionIndex = GetPieceIndex(ActualPostion);

            bool ActualTurn = IsWhiteTurn;
            int[] ActualPositionPiece = { Str2int(ActualPostion)[1], Str2int(ActualPostion)[0] },
                  FuturePositionPiece = { Str2int(FuturePosition)[1], Str2int(FuturePosition)[0] };
            //CheckTest(IsWhite, FuturePosition, ActualPositionIndex)
            bool IsThereAnAlly = false;
            for (int i = 0; i < Wpos.GetLength(0); ++i)
            {
                if (IsWhite && FuturePosition == Wpos[i])
                {
                    IsThereAnAlly = true;
                    break;
                }
                else if (!IsWhite && FuturePosition == Bpos[i])
                {
                    IsThereAnAlly = true;
                    break;
                }
            }

            if (ActualPositionIndex != -1 && !IsThereAnAlly)
            {
                if (IsWhite)//turn of the white pieces
                {

                    if (PieceChar[ActualPositionIndex] == "P")
                    {
                        if (ActualPositionPiece[0] == 7 && FuturePositionPiece[0] == ActualPositionPiece[0] - 2 && ActualPositionPiece[1] == FuturePositionPiece[1] && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;

                        if (FuturePositionPiece[0] == ActualPositionPiece[0] - 1 && ActualPositionPiece[1] == FuturePositionPiece[1] && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;

                    }
                    else if (PieceChar[ActualPositionIndex] == "H")
                    {
                        bool move = false;
                        if (FuturePositionPiece[0] == ActualPositionPiece[0] - 1 && FuturePositionPiece[1] == ActualPositionPiece[1] + 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 2 && FuturePositionPiece[1] == ActualPositionPiece[1] + 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 2 && FuturePositionPiece[1] == ActualPositionPiece[1] - 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 1 && FuturePositionPiece[1] == ActualPositionPiece[1] - 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 1 && FuturePositionPiece[1] == ActualPositionPiece[1] - 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 2 && FuturePositionPiece[1] == ActualPositionPiece[1] - 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 2 && FuturePositionPiece[1] == ActualPositionPiece[1] + 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 1 && FuturePositionPiece[1] == ActualPositionPiece[1] + 2)
                            move = true;

                        if (move && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "T")//white tower
                    {
                        bool IsPathClear = true;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        if (FuturePositionPiece[0] == ActualPositionPiece[0])
                        {
                            while (true)
                            {
                                if (x == FuturePositionPiece[1])
                                    break;
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1])
                        {
                            while (true)
                            {
                                if (y == FuturePositionPiece[0])
                                    break;
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else
                            CanItMove = false;


                    }
                    else if (PieceChar[ActualPositionIndex] == "B")//white bishop
                    {
                        bool IsPathClear = true;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]))
                        {
                            while (true)
                            {
                                if (x == FuturePositionPiece[1] || y == FuturePositionPiece[0])
                                    break;
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else
                            CanItMove = false;

                    }
                    else if (PieceChar[ActualPositionIndex] == "K")//white king
                    {
                        bool move = false;
                        if (FuturePositionPiece[1] == ActualPositionPiece[1] && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0])
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0])
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else
                            CanItMove = false;


                        if (move && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "Q")//white Queen
                    {
                        bool IsPathClear = true;
                        bool move = false;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        while (true)
                        {
                            if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]) || FuturePositionPiece[0] == ActualPositionPiece[0])
                            {
                                if (x == FuturePositionPiece[1])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]) || FuturePositionPiece[1] == ActualPositionPiece[1])
                            {
                                if (y == FuturePositionPiece[0])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (move)
                        {
                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else
                            CanItMove = false;
                    }//end of white queen motion

                }
                /************************************************************************
                 *                 END OF WHITE PIECES MOVEMENT                         *
                 ************************************************************************/
                else//Black Turn
                {
                    if (PieceChar[ActualPositionIndex] == "P")
                    {
                        if (ActualPositionPiece[0] == 2 && FuturePositionPiece[0] == ActualPositionPiece[0] + 2 && ActualPositionPiece[1] == FuturePositionPiece[1] && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;

                        if (FuturePositionPiece[0] == ActualPositionPiece[0] + 1 && ActualPositionPiece[1] == FuturePositionPiece[1] && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "H")//balck horse
                    {
                        bool move = false;

                        if (FuturePositionPiece[0] == ActualPositionPiece[0] - 1 && FuturePositionPiece[1] == ActualPositionPiece[1] + 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 2 && FuturePositionPiece[1] == ActualPositionPiece[1] + 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 2 && FuturePositionPiece[1] == ActualPositionPiece[1] - 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] - 1 && FuturePositionPiece[1] == ActualPositionPiece[1] - 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 1 && FuturePositionPiece[1] == ActualPositionPiece[1] - 2)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 2 && FuturePositionPiece[1] == ActualPositionPiece[1] - 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 2 && FuturePositionPiece[1] == ActualPositionPiece[1] + 1)
                            move = true;
                        else if (FuturePositionPiece[0] == ActualPositionPiece[0] + 1 && FuturePositionPiece[1] == ActualPositionPiece[1] + 2)
                            move = true;
                        else
                            CanItMove = false;

                        if (move && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "T")//black tower
                    {
                        bool IsPathClear = true;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        if (FuturePositionPiece[0] == ActualPositionPiece[0])
                        {
                            while (true)
                            {
                                if (x == FuturePositionPiece[1])
                                    break;
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1])
                        {
                            while (true)
                            {
                                if (y == FuturePositionPiece[0])
                                    break;
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "B")//black bishop
                    {
                        bool IsPathClear = true;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]))
                        {
                            while (true)
                            {
                                if (x == FuturePositionPiece[1])
                                    break;
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                                CanItMove = true;
                            else
                                CanItMove = false;
                        }
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "K")//black king
                    {
                        bool move = false;
                        if (FuturePositionPiece[1] == ActualPositionPiece[1] && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0])
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] - 1 && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0] + 1)
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0])
                            move = true;
                        else if (FuturePositionPiece[1] == ActualPositionPiece[1] + 1 && FuturePositionPiece[0] == ActualPositionPiece[0] - 1)
                            move = true;
                        else
                            CanItMove = false;


                        if (move && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                            CanItMove = true;
                        else
                            CanItMove = false;
                    }
                    else if (PieceChar[ActualPositionIndex] == "Q")//black queen
                    {
                        bool IsPathClear = true;
                        bool move = false;
                        int x = ActualPositionPiece[1], y = ActualPositionPiece[0];

                        while (true)
                        {
                            if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]) || FuturePositionPiece[0] == ActualPositionPiece[0])
                            {
                                if (x == FuturePositionPiece[1])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (FuturePositionPiece[1] > ActualPositionPiece[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(FuturePositionPiece[0] - ActualPositionPiece[0]) == Math.Abs(FuturePositionPiece[1] - ActualPositionPiece[1]) || FuturePositionPiece[1] == ActualPositionPiece[1])
                            {
                                if (y == FuturePositionPiece[0])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (FuturePositionPiece[0] > ActualPositionPiece[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (move && !CheckTest(IsWhite, FuturePosition, ActualPositionIndex))
                        {
                            if (IsPathClear)
                                CanItMove = true;
                        }
                        else
                            CanItMove = false;
                    }
                }
            }

            return CanItMove;
        }

        public int[] BoardPosition(string pos)
        {
            int[] mypos = new int[2];
            for (int i = 0; i < Board.GetLength(0); ++i)
            {
                if (Board[i, 0] == pos)
                {
                    mypos[0] = Convert.ToInt16(Board[i, 1]);
                    mypos[1] = Convert.ToInt16(Board[i, 2]);
                }
            }
            return mypos;
        }

        public int[] Str2int(string input)
        {
            int[] output = { 0, Convert.ToInt16(input[1].ToString()) };
            if (input[0] == 'A')
                output[0] = 0;
            else if (input[0] == 'B')
                output[0] = 1;
            else if (input[0] == 'C')
                output[0] = 2;
            else if (input[0] == 'D')
                output[0] = 3;
            else if (input[0] == 'E')
                output[0] = 4;
            else if (input[0] == 'F')
                output[0] = 5;
            else if (input[0] == 'G')
                output[0] = 6;
            else if (input[0] == 'H')
                output[0] = 7;

            return output;
        }

        public string int2Str(int[] input)
        {
            string temp = "";
            switch (input[0])
            {
                case 0:
                    temp = "A";
                    break;
                case 1:
                    temp = "B";
                    break;
                case 2:
                    temp = "C";
                    break;
                case 3:
                    temp = "D";
                    break;
                case 4:
                    temp = "E";
                    break;
                case 5:
                    temp = "F";
                    break;
                case 6:
                    temp = "G";
                    break;
                case 7:
                    temp = "H";
                    break;
            }

            if (temp != "" && input[1] > 0 && input[1] <= 8)
                return (temp + input[1].ToString());
            else
                return "NA";
        }

        public string int2Str(int i, int j)
        {
            string temp = "";
            switch (i)
            {
                case 0:
                    temp = "A";
                    break;
                case 1:
                    temp = "B";
                    break;
                case 2:
                    temp = "C";
                    break;
                case 3:
                    temp = "D";
                    break;
                case 4:
                    temp = "E";
                    break;
                case 5:
                    temp = "F";
                    break;
                case 6:
                    temp = "G";
                    break;
                case 7:
                    temp = "H";
                    break;
            }

            if (temp != "" && j > 0 && j <= 8)
                return (temp + j.ToString());
            else
                return "NA";
        }

        public void initializeBoard()
        {
            Board = new string[64, 3];

            int ind = 0;
            string temp = "";
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    switch (j)
                    {
                        case 0:
                            temp = "A";
                            break;
                        case 1:
                            temp = "B";
                            break;
                        case 2:
                            temp = "C";
                            break;
                        case 3:
                            temp = "D";
                            break;
                        case 4:
                            temp = "E";
                            break;
                        case 5:
                            temp = "F";
                            break;
                        case 6:
                            temp = "G";
                            break;
                        case 7:
                            temp = "H";
                            break;
                    }
                    Board[ind, 0] = temp + (i + 1).ToString();
                    Board[ind, 2] = (y0 + j * 60).ToString();
                    Board[ind, 1] = (x0 + i * 60).ToString();
                    ++ind;
                }
            }
        }

        public void initializePieces()
        {
            White = new Bitmap[16];
            Black = new Bitmap[16];
            WPieces = new PictureBox[16];
            BPieces = new PictureBox[16];

            string[,] PiecesFileName = {{"WP.png","WP.png","WP.png","WP.png","WP.png","WP.png","WP.png","WP.png",
                                         "WT.png","WH.png","WB.png","WQ.png","WK.png","WB.png","WH.png","WT.png"},
                                        {"BP.png","BP.png","BP.png","BP.png","BP.png","BP.png","BP.png","BP.png",
                                         "BT.png","BH.png","BB.png","BQ.png","BK.png","BB.png","BH.png","BT.png"}};

            string OrigirnFilePath = Directory.GetCurrentDirectory();
            //richTextBox1.Text = OrigirnFilePath + "\n";
            string[] Main_folders = new string[Directory.EnumerateDirectories(OrigirnFilePath).Count<string>()];
            for (int i = 0; i < Directory.EnumerateDirectories(OrigirnFilePath).Count<string>(); ++i)
            {
                Main_folders[i] = Directory.EnumerateDirectories(OrigirnFilePath).ElementAt<string>(i);
                //richTextBox1.Text += Main_folders[i] + "\n";
            }


            WPieces[0] = wp0;
            WPieces[1] = wp1;
            WPieces[2] = wp2;
            WPieces[3] = wp3;
            WPieces[4] = wp4;
            WPieces[5] = wp5;
            WPieces[6] = wp6;
            WPieces[7] = wp7;
            WPieces[8] = wt0;
            WPieces[9] = wh0;
            WPieces[10] = wb0;
            WPieces[11] = wq;
            WPieces[12] = wk;
            WPieces[13] = wb1;
            WPieces[14] = wh1;
            WPieces[15] = wt1;

            BPieces[0] = bp0;
            BPieces[1] = bp1;
            BPieces[2] = bp2;
            BPieces[3] = bp3;
            BPieces[4] = bp4;
            BPieces[5] = bp5;
            BPieces[6] = bp6;
            BPieces[7] = bp7;
            BPieces[8] = bt0;
            BPieces[9] = bh0;
            BPieces[10] = bb0;
            BPieces[11] = bq;
            BPieces[12] = bk;
            BPieces[13] = bb1;
            BPieces[14] = bh1;
            BPieces[15] = bt1;

            try
            {

                for (int i = 0; i < PiecesFileName.GetLength(0); ++i)
                {
                    for (int j = 0; j < PiecesFileName.GetLength(1); ++j)
                    {
                        if (i == 0)
                            White[j] = new Bitmap(Main_folders[0] + @"\" + PiecesFileName[i, j]);
                        else
                            Black[j] = new Bitmap(Main_folders[0] + @"\" + PiecesFileName[i, j]);
                    }
                }
            }
            catch (Exception)
            {

            }

            //put the white pieces in place
            for (int i = 0; i < 16; ++i)
                WPieces[i].BackgroundImage = White[i];
            for (int i = 0; i < 64; ++i)
            {
                for (int k = 0; k < 16; ++k)
                {
                    if (Board[i, 0] == Wpos[k])
                        AssingLocation(WPieces[k], i);
                }
            }
            //and now the black ones
            for (int i = 0; i < 16; ++i)
                BPieces[i].BackgroundImage = Black[i];
            for (int i = 0; i < 64; ++i)
            {
                for (int k = 0; k < 16; ++k)
                {
                    if (Board[i, 0] == Bpos[k])
                        AssingLocation(BPieces[k], i);
                }
            }
        }

        public void AssingColor(PictureBox box, int i)
        {
            if (i != 0)
            {
                if ((i / 8) % 2 == 0)
                {
                    if (i % 2 == 0)
                    {
                        box.BackColor = Color.White;
                    }
                    else
                        box.BackColor = Color.DarkGray;
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        box.BackColor = Color.DarkGray;
                    }
                    else
                        box.BackColor = Color.White;
                }
            }
            else
                box.BackColor = Color.White;
        }

        public void AssingLocation(PictureBox box, int i)
        {
            box.Location = new Point(Convert.ToInt32(Board[i, 2]), Convert.ToInt32(Board[i, 1]));
            AssingColor(box, i);
        }

        public void MovePiece(PictureBox _box, string location)
        {
            int index = 0;
            for (int i = 0; i < 64; ++i)
            {
                if (Board[i, 0] == location)
                {
                    index = i;
                    break;
                }
            }
            _box.Location = new Point(Convert.ToInt32(Board[index, 2]), Convert.ToInt32(Board[index, 1]));
            AssingColor(_box, index);
        }
        #endregion
        
        /*
         * LOAD FUNCTION
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            info.Text = "Press Esc to exit\nPress Enter to Restart game\nPress BackSpace to go back to main menu";
            initializeBoard();//initialize the board coordinates
            //label20.Hide();
            //richTextBox1.Text = "Begining_Wpos:\n";
            //for (int i = 0; i < Wpos.Length; ++i)
            //{
            //    if (i == Wpos.Length / 2)
            //        richTextBox1.Text += "\n";
            //    richTextBox1.Text += Begining_Wpos[i] + "  ";
            //}
            //richTextBox1.Text += "\nBegining_Bpos:\n";
            //for (int i = 0; i < Wpos.Length; ++i)
            //{
            //    if (i == Bpos.Length / 2)
            //        richTextBox1.Text += "\n";
            //    richTextBox1.Text += Begining_Bpos[i] + "  ";
            //}
            //Initialize the white pieces
            initializePieces();//initialize the images and put them in theirs places

            //richTextBox1.Text = TestMovement(true, Wpos[0], "A4").ToString();
            label14.Text = "MousePos = (" + mousePos[0].ToString() + "," + mousePos[1].ToString() + ")";
            label18.Text = "ToMove = " + ToMove[0];
            label22.Text = "PieceSelected = NONE";
            label20.Text = "Last Clicked Position = NONE";
            label21.Text = "White is in check = " + WhiteIsCap.ToString() + "\nBlack is in check = " + BlackIsCap.ToString();
            label19.Text = "";
            //MessageBox.Show(Wpos[15] + "\n" + int2Str(Str2int(Wpos[15])[0] - 2, Str2int(Wpos[15])[1]) + "\n" +
            //                Wpos[8] + "\n" + int2Str(Str2int(Wpos[8])[0] + 2, Str2int(Wpos[8])[1]));


            //if (MessageBox.Show("Do you wana play with time?", "Options", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    WithTime = true;
            //}

            CLOCK.Visible = WithTime;

            if (WithTime)
            {
                PlayersMovement.Start();
                WHITE_TIME.Text = WhiteTime[1].ToString("00") + ":" + WhiteTime[0].ToString("00");
                BLACK_TIME.Text = BlackTime[1].ToString("00") + ":" + BlackTime[0].ToString("00");
            }

            //ResetGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int aKey = e.KeyValue;
            label19.Text = e.KeyValue.ToString();
            if (aKey == 27)//esc
            {
                PlayersMovement.Stop();
                if (MessageBox.Show("Do you wana exit?", "Close form", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                    PlayersMovement.Start();
            }
            else if (aKey == 13)//enter
            {
                PlayersMovement.Stop();
                if (MessageBox.Show("Do you wana Reset game?", "Reset Game", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ResetGame();
                }
                else
                    PlayersMovement.Start();
            }
            else if (aKey == 8)//backspace
            {
                PlayersMovement.Stop();
                if (MessageBox.Show("Do you wana go back to main menu?", "Main menu", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GoBack = true;
                }
                else
                    PlayersMovement.Start();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("TROLOLO");
            Application.Exit();
        }

        /*
         * BOARD MOVEMENT FUNCTION
         */
        private void pictureBox1_Click(object sender, EventArgs e)//pieces movement
        {
            label20.Text = "Last Clicked Position = " + Mouse2Pos();
            bool ThereWasMove = false;

            if (ToMove[0] == "true")
            {
                bool ActualTurn = IsWhiteTurn;
                MovingPiecePos[0] = Str2int(ToMove[1])[1];
                MovingPiecePos[1] = Str2int(ToMove[1])[0];
                //CheckTest();
                if (IsWhiteTurn)//turn of the white pieces
                {
                    #region WhitePieces
                    if (ToMove[2] == "P")
                    {
                        if (MovingPiecePos[0] == 7 && MousePos()[0] == MovingPiecePos[0] - 2 && MovingPiecePos[1] == MousePos()[1] && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";

                        if (MousePos()[0] == MovingPiecePos[0] - 1 && MovingPiecePos[1] == MousePos()[1] && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";

                    }
                    else if (ToMove[2] == "H")
                    {
                        bool move = false;
                        if (MousePos()[0] == MovingPiecePos[0] - 1 && MousePos()[1] == MovingPiecePos[1] + 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 2 && MousePos()[1] == MovingPiecePos[1] + 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 2 && MousePos()[1] == MovingPiecePos[1] - 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 1 && MousePos()[1] == MovingPiecePos[1] - 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 1 && MousePos()[1] == MovingPiecePos[1] - 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 2 && MousePos()[1] == MovingPiecePos[1] - 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 2 && MousePos()[1] == MovingPiecePos[1] + 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 1 && MousePos()[1] == MovingPiecePos[1] + 2)
                            move = true;

                        if (move && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "T")//white tower
                    {
                        bool IsPathClear = true;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        if (MousePos()[0] == MovingPiecePos[0])
                        {
                            while (true)
                            {
                                if (x == MousePos()[1])
                                    break;
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else if (MousePos()[1] == MovingPiecePos[1])
                        {
                            while (true)
                            {
                                if (y == MousePos()[0])
                                    break;
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";


                    }
                    else if (ToMove[2] == "B")//white bishop
                    {
                        bool IsPathClear = true;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]))
                        {
                            while (true)
                            {
                                if (x == MousePos()[1] || y == MousePos()[0])
                                    break;
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";

                    }
                    else if (ToMove[2] == "K")//white king
                    {
                        bool move = false;
                        int Rocker = -1;
                        //MessageBox.Show(MousePos()[0].ToString() + "; " + MousePos()[1].ToString() + "\n" + 
                        //                MovingPiecePos[0].ToString() + "; " + MovingPiecePos[1].ToString() + "\n" +
                        //                Wpos[8] + ", " + Wpos[15] + "\n" +
                        //                int2Str(Str2int(Wpos[15])[0] - 2, Str2int(Wpos[15])[1]));


                        if (MovingPiecePos[1] == Str2int(Begining_Wpos[WhiteKingIndex])[0] && MovingPiecePos[0] == Str2int(Begining_Wpos[WhiteKingIndex])[1])
                        {
                            if (MousePos()[1] == MovingPiecePos[1] - 2 && MousePos()[0] == MovingPiecePos[0] && Wpos[8] == Begining_Wpos[8] && WPieces[8].Visible)//rock to left
                            {
                                int x = MovingPiecePos[1];
                                int y = MovingPiecePos[0];
                                bool IsPathClear = true;
                                //MessageBox.Show(MovingPiecePos[1].ToString() + "\n" + Str2int(Wpos[8])[0].ToString());
                                while (true)
                                {
                                    if (x == Str2int(Wpos[8])[0] + 1)
                                        break;
                                    --x;
                                    MessageBox.Show(int2Str(x, y));
                                    for (int i = 0; i < 16; ++i)
                                    {
                                        if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                            IsPathClear = false;
                                        else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                            IsPathClear = false;
                                        if (!IsPathClear)
                                            break;
                                    }
                                    if (!IsPathClear)
                                        break;
                                }

                                if (IsPathClear)
                                {
                                    move = true;
                                    Rocker = 8;
                                }
                            }
                            else if (MousePos()[1] == MovingPiecePos[1] + 2 && MousePos()[0] == MovingPiecePos[0] && Wpos[15] == Begining_Wpos[15] && WPieces[15].Visible)//rock to left
                            {
                                int x = MovingPiecePos[1];
                                int y = MovingPiecePos[0];
                                bool IsPathClear = true;

                                //MessageBox.Show(x.ToString() + "\n" + Str2int(Wpos[15])[1].ToString());
                                while (true)
                                {
                                    if (x == Str2int(Wpos[15])[1] - 2)
                                        break;
                                    ++x;

                                    MessageBox.Show(int2Str(x, y));
                                    for (int i = 0; i < 16; ++i)
                                    {
                                        if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                            IsPathClear = false;
                                        else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                            IsPathClear = false;
                                        if (!IsPathClear)
                                            break;
                                    }
                                    if (!IsPathClear)
                                        break;
                                }

                                if (IsPathClear)
                                {
                                    move = true;
                                    Rocker = 15;
                                }
                            }
                        }
                        if (MousePos()[1] == MovingPiecePos[1] && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0])
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0])
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else
                            ToMove[0] = "false";

                        MessageBox.Show(Rocker.ToString());
                        if (move && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                            if (Rocker != -1)
                            {
                                if (Rocker == 15)
                                {
                                    Wpos[Rocker] = int2Str(Str2int(Wpos[Rocker])[0] - 2, Str2int(Wpos[Rocker])[1]);
                                }
                                else
                                    Wpos[Rocker] = int2Str(Str2int(Wpos[Rocker])[0] + 3, Str2int(Wpos[Rocker])[1]);
                                MovePiece(WPieces[Rocker], Wpos[Rocker]);
                            }
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "Q")//white Queen
                    {
                        bool IsPathClear = true;
                        bool move = false;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        while (true)
                        {
                            if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]) || MousePos()[0] == MovingPiecePos[0])
                            {
                                if (x == MousePos()[1])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]) || MousePos()[1] == MovingPiecePos[1])
                            {
                                if (y == MousePos()[0])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (move)
                        {
                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Wpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(WPieces[Convert.ToInt16(ToMove[3])], Wpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }//end of white queen motion
                    WPieces[Convert.ToInt16(ToMove[3])].BorderStyle = BorderStyle.None;

                }
                    #endregion
                /************************************************************************
                 *                 END OF WHITE PIECES MOVEMENT                         *
                 ************************************************************************/
                else//Black Turn
                {
                    #region BlackPieces
                    if (ToMove[2] == "P")
                    {
                        if (MovingPiecePos[0] == 2 && MousePos()[0] == MovingPiecePos[0] + 2 && MovingPiecePos[1] == MousePos()[1] && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            //MousePos_str
                            Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";

                        if (MousePos()[0] == MovingPiecePos[0] + 1 && MovingPiecePos[1] == MousePos()[1] && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "H")//balck horse
                    {
                        bool move = false;

                        if (MousePos()[0] == MovingPiecePos[0] - 1 && MousePos()[1] == MovingPiecePos[1] + 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 2 && MousePos()[1] == MovingPiecePos[1] + 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 2 && MousePos()[1] == MovingPiecePos[1] - 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] - 1 && MousePos()[1] == MovingPiecePos[1] - 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 1 && MousePos()[1] == MovingPiecePos[1] - 2)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 2 && MousePos()[1] == MovingPiecePos[1] - 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 2 && MousePos()[1] == MovingPiecePos[1] + 1)
                            move = true;
                        else if (MousePos()[0] == MovingPiecePos[0] + 1 && MousePos()[1] == MovingPiecePos[1] + 2)
                            move = true;
                        else
                            ToMove[0] = "false";

                        if (move && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "T")//black tower
                    {
                        bool IsPathClear = true;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        if (MousePos()[0] == MovingPiecePos[0])
                        {
                            while (true)
                            {
                                if (x == MousePos()[1])
                                    break;
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else if (MousePos()[1] == MovingPiecePos[1])
                        {
                            while (true)
                            {
                                if (y == MousePos()[0])
                                    break;
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {
                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "B")//black bishop
                    {
                        bool IsPathClear = true;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]))
                        {
                            while (true)
                            {
                                if (x == MousePos()[1])
                                    break;
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;

                                for (int i = 0; i < 16; ++i)
                                {

                                    if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                        IsPathClear = false;
                                    else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                        IsPathClear = false;
                                    if (!IsPathClear)
                                        break;
                                }
                                if (!IsPathClear)
                                    break;
                            }

                            if (IsPathClear && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                            else
                                ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "K")//black king
                    {
                        bool move = false;
                        int Rocker = -1;

                        if (MovingPiecePos[1] == Str2int(Begining_Bpos[BlackKingIndex])[0] && MovingPiecePos[0] == Str2int(Begining_Bpos[BlackKingIndex])[1])
                        {
                            if (MousePos()[1] == MovingPiecePos[1] - 2 && MousePos()[0] == MovingPiecePos[0] && Bpos[8] == Begining_Bpos[8] && BPieces[8].Visible)//rock to left
                            {
                                int x = MovingPiecePos[1];
                                int y = MovingPiecePos[0];
                                bool IsPathClear = true;
                                //MessageBox.Show(MovingPiecePos[1].ToString() + "\n" + Str2int(Wpos[8])[0].ToString());
                                while (true)
                                {
                                    if (x == Str2int(Bpos[8])[0] + 1)
                                        break;
                                    --x;
                                    MessageBox.Show(int2Str(x, y));
                                    for (int i = 0; i < 16; ++i)
                                    {
                                        if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                            IsPathClear = false;
                                        else if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                            IsPathClear = false;
                                        if (!IsPathClear)
                                            break;
                                    }
                                    if (!IsPathClear)
                                        break;
                                }

                                if (IsPathClear)
                                {
                                    move = true;
                                    Rocker = 8;
                                }
                            }
                            else if (MousePos()[1] == MovingPiecePos[1] + 2 && MousePos()[0] == MovingPiecePos[0] && Bpos[15] == Begining_Bpos[15] && BPieces[15].Visible)//rock to left
                            {
                                int x = MovingPiecePos[1];
                                int y = MovingPiecePos[0];
                                bool IsPathClear = true;

                                while (true)
                                {
                                    if (x == Str2int(Bpos[15])[1] + 1)
                                        break;
                                    ++x;

                                    MessageBox.Show(int2Str(x, y));
                                    for (int i = 0; i < 16; ++i)
                                    {
                                        if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                            IsPathClear = false;
                                        else if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                            IsPathClear = false;
                                        if (!IsPathClear)
                                            break;
                                    }
                                    if (!IsPathClear)
                                        break;
                                }

                                if (IsPathClear)
                                {
                                    move = true;
                                    Rocker = 15;
                                }
                            }
                        }

                        if (MousePos()[1] == MovingPiecePos[1] && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0])
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] - 1 && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0] + 1)
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0])
                            move = true;
                        else if (MousePos()[1] == MovingPiecePos[1] + 1 && MousePos()[0] == MovingPiecePos[0] - 1)
                            move = true;
                        else
                            ToMove[0] = "false";


                        if (move && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                            MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                            if (Rocker != -1)
                            {
                                if (Rocker == 15)
                                {
                                    Bpos[Rocker] = int2Str(Str2int(Bpos[Rocker])[0] - 2, Str2int(Bpos[Rocker])[1]);
                                }
                                else
                                    Bpos[Rocker] = int2Str(Str2int(Bpos[Rocker])[0] + 3, Str2int(Bpos[Rocker])[1]);
                                MovePiece(BPieces[Rocker], Bpos[Rocker]);
                            }
                            ThereWasMove = true;
                            IsWhiteTurn = !IsWhiteTurn;
                            ToMove[0] = "false";
                        }
                        else
                            ToMove[0] = "false";
                    }
                    else if (ToMove[2] == "Q")//black queen
                    {
                        bool IsPathClear = true;
                        bool move = false;
                        int x = MovingPiecePos[1], y = MovingPiecePos[0];

                        while (true)
                        {
                            if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]) || MousePos()[0] == MovingPiecePos[0])
                            {
                                if (x == MousePos()[1])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (MousePos()[1] > MovingPiecePos[1])
                                    ++x;
                                else
                                    --x;
                            }
                            if (Math.Abs(MousePos()[0] - MovingPiecePos[0]) == Math.Abs(MousePos()[1] - MovingPiecePos[1]) || MousePos()[1] == MovingPiecePos[1])
                            {
                                if (y == MousePos()[0])
                                {
                                    if (IsPathClear)
                                        move = true;
                                    break;
                                }
                                if (MousePos()[0] > MovingPiecePos[0])
                                    ++y;
                                else
                                    --y;
                            }

                            for (int i = 0; i < 16; ++i)
                            {

                                if (int2Str(x, y) == Wpos[i] && WPieces[i].Visible)
                                    IsPathClear = false;
                                else if (int2Str(x, y) == Bpos[i] && BPieces[i].Visible)
                                    IsPathClear = false;
                                if (!IsPathClear)
                                    break;
                            }
                            if (!IsPathClear)
                                break;
                        }

                        if (move && !CheckTest(IsWhiteTurn, MousePos_str, int.Parse(ToMove[3])))
                        {
                            if (IsPathClear)
                            {
                                Bpos[Convert.ToInt16(ToMove[3])] = MousePos_str;
                                MovePiece(BPieces[Convert.ToInt16(ToMove[3])], Bpos[Convert.ToInt16(ToMove[3])]);
                                ThereWasMove = true;
                                IsWhiteTurn = !IsWhiteTurn;
                                ToMove[0] = "false";
                            }
                        }
                        else
                            ToMove[0] = "false";
                    }
                    BPieces[Convert.ToInt16(ToMove[3])].BorderStyle = BorderStyle.None;
                    #endregion
                }

                //richTextBox1.Text = "WPieces:\n";
                //for (int i = 0; i < Wpos.Length; ++i)
                //{
                //    richTextBox1.Text += "Visible[" + PieceChar[i] + "; " + Wpos[i] + "] = " + WPieces[i].Visible.ToString() + "\n";
                //}
                //richTextBox1.Text += "\nBPieces:\n";
                //for (int i = 0; i < Wpos.Length; ++i)
                //{
                //    richTextBox1.Text += "Visible[" + PieceChar[i] + "; " + Bpos[i] + "] = " + BPieces[i].Visible.ToString() + "\n";
                //}

                if (IsWhiteTurn)
                    turn.Text = "WHITE";
                else
                    turn.Text = "BLACK";

                CheckTest();
                if (WhiteIsCap || BlackIsCap)
                {
                    Check_Mate(IsWhiteTurn);
                }
                else
                    label19.Text = "Nothing";
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos[0] = e.X + x0;
            mousePos[1] = e.Y + y0;
            label14.Text = "MousePos = (" + mousePos[0].ToString() + "," + mousePos[1].ToString() + ")";
            label18.Text = "ToMove = " + ToMove[0];
            if (ToMove[0] == "true")
            {
                if (IsWhiteTurn)
                    label22.Text = "PieceSelected = " + PieceChar[int.Parse(ToMove[3])] + "/" + ToMove[2] + " at " + Wpos[int.Parse(ToMove[3])];
                else
                    label22.Text = "PieceSelected = " + PieceChar[int.Parse(ToMove[3])] + "/" + ToMove[2] + " at " + Bpos[int.Parse(ToMove[3])];
            }
            else
                label22.Text = "PieceSelected = NONE";
        }
        /*
         * WHITE PIECES MOVEMENT
         */
        #region White_PiecesClickFunctions
        private void wp0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[0];
                    ToMove[2] = "P";
                    ToMove[3] = "0";
                    wp0.BorderStyle = BorderStyle.Fixed3D;
                }
                //WPieces[0].BorderStyle = BorderStyle.FixedSingle;
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "0")
            //{
            //    ToMove[0] = "false";
            //    wp0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(0);
        }

        private void wp1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[1];
                    ToMove[2] = "P";
                    ToMove[3] = "1";
                    wp1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "1")
            //{
            //    ToMove[0] = "false";
            //    wp1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(1);
        }

        private void wp2_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[2];
                    ToMove[2] = "P";
                    ToMove[3] = "2";
                    wp2.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "2")
            //{
            //    ToMove[0] = "false";
            //    wp2.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(2);
        }

        private void wp3_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[3];
                    ToMove[2] = "P";
                    ToMove[3] = "3";
                    wp3.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "3")
            //{
            //    ToMove[0] = "false";
            //    wp3.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(3);
        }

        private void wp4_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[4];
                    ToMove[2] = "P";
                    ToMove[3] = "4";
                    wp4.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "4")
            //{
            //    ToMove[0] = "false";
            //    wp4.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(4);
        }

        private void wp5_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[5];
                    ToMove[2] = "P";
                    ToMove[3] = "5";
                    wp5.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "5")
            //{
            //    ToMove[0] = "false";
            //    wp5.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(5);
        }

        private void wp6_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[6];
                    ToMove[2] = "P";
                    ToMove[3] = "6";
                    wp6.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "6")
            //{
            //    ToMove[0] = "false";
            //    wp6.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(6);
        }

        private void wp7_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[7];
                    ToMove[2] = "P";
                    ToMove[3] = "7";
                    wp7.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "7")
            //{
            //    ToMove[0] = "false";
            //    wp7.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(7);
        }

        private void wh0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[9];
                    ToMove[2] = "H";
                    ToMove[3] = "9";
                    wh0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "9")
            //{
            //    ToMove[0] = "false";
            //    wh0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(9);
        }

        private void wh1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[14];
                    ToMove[2] = "H";
                    ToMove[3] = "14";
                    wh1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "14")
            //{
            //    ToMove[0] = "false";
            //    wh1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(14);
        }

        private void wt0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[8];
                    ToMove[2] = "T";
                    ToMove[3] = "8";
                    wt0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "8")
            //{
            //    ToMove[0] = "false";
            //    wt0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(8);

        }

        private void wt1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[15];
                    ToMove[2] = "T";
                    ToMove[3] = "15";
                    wt1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "15")
            //{
            //    ToMove[0] = "false";
            //    wt1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(15);
        }

        private void wb0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[10];
                    ToMove[2] = "B";
                    ToMove[3] = "10";
                    wb0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "10")
            //{
            //    ToMove[0] = "false";
            //    wb0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(10);
        }

        private void wb1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[13];
                    ToMove[2] = "B";
                    ToMove[3] = "13";
                    wb1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "13")
            //{
            //    ToMove[0] = "false";
            //    wb1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(13);
        }

        private void wk_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[12];
                    ToMove[2] = "K";
                    ToMove[3] = "12";
                    wk.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "12")
            //{
            //    ToMove[0] = "false";
            //    wk.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(12);
        }

        private void wq_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Wpos[11];
                    ToMove[2] = "Q";
                    ToMove[3] = "11";
                    wq.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (IsWhiteTurn)
            {
                for (int i = 0; i < WPieces.GetLength(0); ++i)
                    WPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "11")
            //{
            //    ToMove[0] = "false";
            //    wq.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(11);
        }
        #endregion
        /*
         * BLACK PIECES MOVEMENT
         */
        #region Black_PiecesClickFunctions
        private void bp0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[0];
                    ToMove[2] = "P";
                    ToMove[3] = "0";
                    bp0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "0")
            //{
            //    ToMove[0] = "false";
            //    bp0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(0);
        }

        private void bp1_Click(object sender, EventArgs e)
        {
            if (!IsWhiteTurn && ToMove[0] == "false")
            {
                ToMove[0] = "true";
                ToMove[1] = Bpos[1];
                ToMove[2] = "P";
                ToMove[3] = "1";
                bp1.BorderStyle = BorderStyle.Fixed3D;
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "1")
            //{
            //    ToMove[0] = "false";
            //    bp1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(1);
        }

        private void bp2_Click(object sender, EventArgs e)
        {
            if (!IsWhiteTurn && ToMove[0] == "false")
            {
                ToMove[0] = "true";
                ToMove[1] = Bpos[2];
                ToMove[2] = "P";
                ToMove[3] = "2";
                bp2.BorderStyle = BorderStyle.Fixed3D;
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "2")
            //{
            //    ToMove[0] = "false";
            //    bp2.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(2);
        }

        private void bp3_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[3];
                    ToMove[2] = "P";
                    ToMove[3] = "3";
                    bp3.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "3")
            //{
            //    ToMove[0] = "false";
            //    bp3.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(3);
        }

        private void bp4_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[4];
                    ToMove[2] = "P";
                    ToMove[3] = "4";
                    bp4.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "4")
            //{
            //    ToMove[0] = "false";
            //    bp4.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(4);
        }

        private void bp5_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[5];
                    ToMove[2] = "P";
                    ToMove[3] = "5";
                    bp5.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "5")
            //{
            //    ToMove[0] = "false";
            //    bp5.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(5);
        }

        private void bp6_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[6];
                    ToMove[2] = "P";
                    ToMove[3] = "6";
                    bp6.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "6")
            //{
            //    ToMove[0] = "false";
            //    bp6.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(6);
        }

        private void bp7_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[7];
                    ToMove[2] = "P";
                    ToMove[3] = "7";
                    bp7.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "7")
            //{
            //    ToMove[0] = "false";
            //    bp7.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(7);
        }

        private void bh0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[9];
                    ToMove[2] = "H";
                    ToMove[3] = "9";
                    bh0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "9")
            //{
            //    ToMove[0] = "false";
            //    bh0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(9);
        }

        private void bh1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[14];
                    ToMove[2] = "H";
                    ToMove[3] = "14";
                    bh1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "14")
            //{
            //    ToMove[0] = "false";
            //    bh1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(14);
        }

        private void bt0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[8];
                    ToMove[2] = "T";
                    ToMove[3] = "8";
                    bt0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "8")
            //{
            //    ToMove[0] = "false";
            //    bt0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(8);
        }

        private void bt1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)//select for movement
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[15];
                    ToMove[2] = "T";
                    ToMove[3] = "15";
                    bt1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "15")
            //{
            //    ToMove[0] = "false";
            //    bt1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(15);
        }

        private void bb0_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[10];
                    ToMove[2] = "B";
                    ToMove[3] = "10";
                    bb0.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "10")
            //{
            //    ToMove[0] = "false";
            //    bb0.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(10);
        }

        private void bb1_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[13];
                    ToMove[2] = "B";
                    ToMove[3] = "13";
                    bb1.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "13")
            //{
            //    ToMove[0] = "false";
            //    bb1.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(13);
        }

        private void bk_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[12];
                    ToMove[2] = "K";
                    ToMove[3] = "12";
                    bk.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "12")
            //{
            //    ToMove[0] = "false";
            //    bk.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(12);
        }

        private void bq_Click(object sender, EventArgs e)
        {
            if (ToMove[0] == "false")
            {
                if (!IsWhiteTurn)
                {
                    ToMove[0] = "true";
                    ToMove[1] = Bpos[11];
                    ToMove[2] = "Q";
                    ToMove[3] = "11";
                    bq.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (!IsWhiteTurn)
            {
                for (int i = 0; i < BPieces.GetLength(0); ++i)
                    BPieces[i].BorderStyle = BorderStyle.None;
                ToMove[0] = "false";
            }
            //else if (ToMove[3] == "11")
            //{
            //    ToMove[0] = "false";
            //    bq.BorderStyle = BorderStyle.None;
            //}
            else
                TestCapture(11);
        }
        #endregion

        private void PlayersMovement_Tick(object sender, EventArgs e)
        {
            if (WithTime)
            {
                if (!HasRestarded)
                {
                    if (!Lose)
                    {
                        if (IsWhiteTurn)
                        {
                            if (WhiteTime[0] == 0)
                            {
                                if (WhiteTime[1] == 0)
                                {
                                    PlayersMovement.Stop();
                                    MessageBox.Show("The White lose!");

                                    Lose = true;
                                }
                                else
                                {
                                    WhiteTime[0] = 59;
                                    --WhiteTime[1];
                                }
                            }
                            else
                                --WhiteTime[0];
                            WHITE_TIME.Text = WhiteTime[1].ToString("00") + ":" + WhiteTime[0].ToString("00");
                        }
                        else
                        {
                            if (BlackTime[0] == 0)
                            {
                                if (BlackTime[1] == 0)
                                {
                                    PlayersMovement.Stop();
                                    MessageBox.Show("The Black lose!");

                                    Lose = true;
                                }
                                else
                                {
                                    BlackTime[0] = 59;
                                    --BlackTime[1];
                                }
                            }
                            else
                                --BlackTime[0];
                            BLACK_TIME.Text = BlackTime[1].ToString("00") + ":" + BlackTime[0].ToString("00");
                        }
                    }
                }
                else
                    HasRestarded = !HasRestarded;

                if (Lose)
                {
                    //PlayersMovement.Stop();
                    if (MessageBox.Show("Do you wana play another game?", "Replay/Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ResetGame();
                        PlayersMovement.Start();
                        Lose = false;
                    }
                    else
                    {
                        Application.Exit();
                    }

                }
            }
        }

        


    }

}