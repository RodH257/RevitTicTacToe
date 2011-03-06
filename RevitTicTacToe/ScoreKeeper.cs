using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitTicTacToe
{
    /// <summary>
    /// keeps the scores in between matches
    /// </summary>
    public class ScoreKeeper
    {
        public const bool X_PLAYER = true;
        public const bool O_PLAYER = false;
        private const string X_SCORE_PRE_TEXT = "Player X Score:";
        private const string O_SCORE_PRE_TEXT = "Player O Score:";
        private TextNote _xScore;
        private TextNote _oScore;
        private Document _dbDoc;

        public ScoreKeeper(Document dbDoc)
        {
            this._dbDoc = dbDoc;
            SetupScores();
        }

        /// <summary>
        /// Sets up the scores
        /// These are simple TextNotes
        /// </summary>
        private void SetupScores()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_dbDoc);
            IEnumerable<Element> noteElements = collector.OfClass(typeof(TextNote)).ToElements();
            foreach (Element e in noteElements)
            {
                TextNote note = e as TextNote;
                if (note.Text.Contains(X_SCORE_PRE_TEXT))
                {
                    //Player X score
                    _xScore = note;

                }
                else if (note.Text.Contains(O_SCORE_PRE_TEXT))
                {
                    //player o score
                    _oScore = note;
                }
            }
        }

        /// <summary>
        /// Resets the scores 
        /// </summary>
        public void ResetScores()
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Score Reset");
            _xScore.Text = X_SCORE_PRE_TEXT + 0;
            _oScore.Text = O_SCORE_PRE_TEXT + 0;
            transaction.Commit();
        }

        /// <summary>
        /// gets the actual score from a textnote
        /// </summary>
        /// <param name="note"></param>
        /// <param name="preText"></param>
        /// <returns></returns>
        private int GetScore(TextNote note, string preText)
        {
            string scoreOnly = note.Text.Replace(preText, "");

            int score;
            if (int.TryParse(scoreOnly, out score))
            {
                return score;
            }
            else
            {
                TaskDialog.Show("Scoring error", "The score tallies are corrupt, please reset the scores");
                return 0;
            }
        }

        /// <summary>
        /// Gets X's Score
        /// </summary>
        /// <returns></returns>
        public int GetXScore()
        {
            return GetScore(_xScore, X_SCORE_PRE_TEXT);
        }

        /// <summary>
        /// Gets O's score
        /// </summary>
        /// <returns></returns>
        public int GetOScore()
        {
            return GetScore(_oScore, O_SCORE_PRE_TEXT);
        }


        /// <summary>
        /// adds 1 to X
        /// </summary>
        public void IncrementX()
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Score Update");
            _xScore.Text = X_SCORE_PRE_TEXT + (GetXScore()+1);
            transaction.Commit();
        }


        /// <summary>
        /// Increments O
        /// </summary>
        public void IncrementO()
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Score Update");
            _oScore.Text = O_SCORE_PRE_TEXT + (GetOScore() + 1);
            transaction.Commit();

        }
    }
}
